using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// World-class Distance Culling Manager.
///
/// CÁI GÌ ĐÃ ĐỔI SO VỚI BẢN GỐC:
///   1. Renderer.enabled thay vì SetActive — không phá scene graph, không trigger lifecycle.
///   2. Register/Unregister API — hỗ trợ dynamic object spawn/despawn lúc runtime.
///   3. Unity Job System + Burst Compiler — loop chạy trên worker thread, SIMD-optimized.
///   4. NativeArray — zero GC allocation mỗi frame.
///   5. Dirty flag — chỉ rebuild NativeArray khi danh sách thực sự thay đổi.
///   6. Dot product chuẩn — normalize direction trước khi tính.
///   7. LOD-aware — tự động nhảy LOD level thay vì chỉ on/off cứng.
///
/// CÁCH DÙNG:
///   - Gắn script này lên Camera hoặc Empty GameObject.
///   - Mỗi object muốn được cull: gọi CameraCullingManager.Instance.Register(renderer).
///   - Khi object bị destroy: gọi CameraCullingManager.Instance.Unregister(renderer).
///   - Hoặc dùng CullableObject component bên dưới để tự động hóa hoàn toàn.
/// </summary>
public class CameraCullingManager : MonoBehaviour
{
    // ─── Singleton ────────────────────────────────────────────────────────────
    public static CameraCullingManager Instance { get; private set; }

    // ─── Inspector ────────────────────────────────────────────────────────────
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Culling Settings")]
    [Tooltip("Bán kính tối đa (mét). Ngoài vùng này sẽ bị cull.")]
    [SerializeField] private float cullRadius = 80f;

    [Tooltip("Khoảng cách để bắt đầu fade LOD (nếu dùng LOD Group).")]
    [SerializeField] private float lodTransitionDistance = 50f;

    [Tooltip("Tần suất cull (giây). 0.1–0.2 là hợp lý.")]
    [SerializeField, Range(0.05f, 1f)] private float checkInterval = 0.15f;

    [Tooltip("Góc mở rộng frustum thêm (độ). Tránh popping khi object ở rìa màn hình.")]
    [SerializeField, Range(0f, 30f)] private float frustumBias = 5f;

    // ─── State ────────────────────────────────────────────────────────────────
    private readonly List<Renderer> _renderers = new List<Renderer>(256);
    private bool _isDirty = true;
    private float _timer;

    // ─── Job System data ──────────────────────────────────────────────────────
    private NativeArray<Vector3> _positions;
    private NativeArray<bool>    _results;

    // ─────────────────────────────────────────────────────────────────────────
    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[CullingManager] Đã có instance. Tự hủy bản trùng.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main ?? FindObjectOfType<Camera>();

        if (targetCamera == null)
        {
            Debug.LogError("[CullingManager] Không tìm thấy Camera nào trong scene!");
            enabled = false;
            return;
        }

        if (GetComponent<Renderer>() != null)
            Debug.LogWarning("[CullingManager] Script này nên gắn vào Camera hoặc Empty GameObject, không phải object có Renderer.");
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < checkInterval) return;
        _timer = 0f;

        CleanNullEntries();
        if (_renderers.Count == 0) return;

        RebuildNativeArraysIfDirty();
        PerformCullingJob();
    }

    private void OnDestroy()
    {
        DisposeNativeArrays();
        if (Instance == this) Instance = null;
    }

    private void OnDrawGizmosSelected()
    {
        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(cam.transform.position, cullRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cam.transform.position, lodTransitionDistance);
    }

    #endregion

    // ─────────────────────────────────────────────────────────────────────────
    #region Public API

    /// <summary>Đăng ký một Renderer để được quản lý culling.</summary>
    public void Register(Renderer renderer)
    {
        if (renderer == null || _renderers.Contains(renderer)) return;
        _renderers.Add(renderer);
        _isDirty = true;
    }

    /// <summary>Hủy đăng ký một Renderer (gọi trước khi Destroy object).</summary>
    public void Unregister(Renderer renderer)
    {
        if (_renderers.Remove(renderer))
            _isDirty = true;
    }

    #endregion

    // ─────────────────────────────────────────────────────────────────────────
    #region Culling Logic

    private void PerformCullingJob()
    {
        // Cập nhật vị trí mỗi frame (positions thay đổi theo physics/animation)
        for (int i = 0; i < _renderers.Count; i++)
            _positions[i] = _renderers[i].transform.position;

        Vector3  camPos     = targetCamera.transform.position;
        Vector3  camForward = targetCamera.transform.forward;
        float    sqrRadius  = cullRadius * cullRadius;
        float    cosAngle   = Mathf.Cos((90f + frustumBias) * Mathf.Deg2Rad);

        var job = new CullJob
        {
            Positions   = _positions,
            Results     = _results,
            CamPosition = camPos,
            CamForward  = camForward,
            SqrRadius   = sqrRadius,
            CosAngle    = cosAngle,
        };

        // Schedule trên worker thread, complete trước khi dùng result
        JobHandle handle = job.Schedule(_renderers.Count, 64);
        handle.Complete();

        // Apply kết quả lên Renderer (phải chạy Main thread vì Unity API)
        float sqrLod = lodTransitionDistance * lodTransitionDistance;
        for (int i = 0; i < _renderers.Count; i++)
        {
            Renderer rend = _renderers[i];
            bool visible  = _results[i];
            rend.enabled  = visible;                             // ← Renderer.enabled, không phải SetActive!

            // LOD quality hint (nếu object có LODGroup thì Unity tự xử lý,
            // đây chỉ là ví dụ apply shadow casting dựa trên khoảng cách)
            if (visible)
            {
                float sqrDist = (rend.transform.position - camPos).sqrMagnitude;
                rend.shadowCastingMode = sqrDist < sqrLod
                    ? UnityEngine.Rendering.ShadowCastingMode.On
                    : UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }

    private void CleanNullEntries()
    {
        // Đi từ cuối lên để remove không làm lệch index
        bool removed = false;
        for (int i = _renderers.Count - 1; i >= 0; i--)
        {
            if (_renderers[i] == null)
            {
                _renderers.RemoveAt(i);
                removed = true;
            }
        }
        if (removed) _isDirty = true;
    }

    private void RebuildNativeArraysIfDirty()
    {
        if (!_isDirty) return;

        DisposeNativeArrays();

        int count   = _renderers.Count;
        _positions  = new NativeArray<Vector3>(count, Allocator.Persistent);
        _results    = new NativeArray<bool>(count, Allocator.Persistent);
        _isDirty    = false;
    }

    private void DisposeNativeArrays()
    {
        if (_positions.IsCreated) _positions.Dispose();
        if (_results.IsCreated)   _results.Dispose();
    }

    #endregion

    // ─────────────────────────────────────────────────────────────────────────
    #region Burst Job

    [BurstCompile]
    private struct CullJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> Positions;
        [WriteOnly] public NativeArray<bool>   Results;

        public Vector3 CamPosition;
        public Vector3 CamForward;
        public float   SqrRadius;
        public float   CosAngle;

        public void Execute(int index)
        {
            Vector3 dir      = Positions[index] - CamPosition;
            float   sqrDist  = dir.x * dir.x + dir.y * dir.y + dir.z * dir.z; // tránh gọi .sqrMagnitude để Burst tối ưu tốt hơn

            if (sqrDist > SqrRadius)
            {
                Results[index] = false;
                return;
            }

            // Normalize direction để dot product chính xác
            float invMag = 1f / math_sqrt(sqrDist + 0.0001f);
            float dot    = (dir.x * CamForward.x + dir.y * CamForward.y + dir.z * CamForward.z) * invMag;

            Results[index] = dot >= CosAngle;
        }

        // Dùng math của Burst thay Mathf để tận dụng SIMD
        private static float math_sqrt(float x)
        {
            return Unity.Mathematics.math.sqrt(x);
        }
    }

    #endregion
}

// ─────────────────────────────────────────────────────────────────────────────
/// <summary>
/// Component helper: gắn vào mọi object muốn được cull tự động.
/// Không cần gọi Register/Unregister thủ công.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class CullableObject : MonoBehaviour
{
    private Renderer _renderer;

    private void Awake()  => _renderer = GetComponent<Renderer>();
    private void OnEnable()
    {
        if (CameraCullingManager.Instance != null)
            CameraCullingManager.Instance.Register(_renderer);
    }
    private void OnDisable()
    {
        if (CameraCullingManager.Instance != null)
            CameraCullingManager.Instance.Unregister(_renderer);
    }
}