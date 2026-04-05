using Unity.Cinemachine; 
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineOrbitalFollow))]
public class CameraRotation : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionAsset _inputAsset;
    [SerializeField] private string _lookActionName = "Look";
    
    [Header("Sensitivity")]
    [SerializeField] private float _horizontalSensitivity = 0.1f;
    [SerializeField] private float _verticalSensitivity = 0.1f;
    
    private CinemachineOrbitalFollow _orbital;
    private InputAction _lookAction;

    private void Awake()
    {
        _orbital = GetComponent<CinemachineOrbitalFollow>();
        
        if (_inputAsset != null)
        {
            _lookAction = _inputAsset.FindAction(_lookActionName);
        }
    }

    private void OnEnable()
    {
        if (_lookAction != null)
        {
            _lookAction.Enable();
            _lookAction.performed += OnLookPerformed;
        }
    }

    private void OnDisable()
    {
        if (_lookAction != null)
        {
            _lookAction.performed -= OnLookPerformed;
            _lookAction.Disable();
        }
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        if (_orbital == null) return;
        
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        
        float x = mouseDelta.x * _horizontalSensitivity;
        float y = -mouseDelta.y * _verticalSensitivity;
        

        _orbital.HorizontalAxis.Value += x;
        _orbital.VerticalAxis.Value += y;
    }
}