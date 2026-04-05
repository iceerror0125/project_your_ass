using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputSystem : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    public InputActionAsset _inputAsset;
    public CharacterController characterController;
    public Animator animator;

    [Space]
    public float walkSpeed = 5f;
    public float rotationSpeed = 10f; // Tốc độ xoay mượt mà
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    
    private InputAction _movement;
    private InputAction _jump;
    private Vector3 _verticalVelocity;
    private Transform _mainCamera; // Lưu ref của camera chính

    private void Awake()
    {
        _movement = _inputAsset.FindAction("Move");
        _jump = _inputAsset.FindAction("Jump");

        // Tìm camera chính trong scene
        if (Camera.main != null)
        {
            _mainCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Camera có tag 'MainCamera'! Nhân vật sẽ di chuyển theo hướng cục bộ.");
        }
    }

    private void OnEnable()
    {
        _inputAsset.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        _inputAsset.FindActionMap("Player").Disable();
    }

    private void Update()
    {
        ApplyGravity();
        Move();
        Jump();
    }

    private void ApplyGravity()
    {
        // Reset vertical velocity when grounded
        if (characterController.isGrounded && _verticalVelocity.y < 0)
        {
            _verticalVelocity.y = -2f;
        }

        // Apply gravity
        _verticalVelocity.y += gravity * Time.deltaTime;
        
        // Final move for vertical axis
        characterController.Move(_verticalVelocity * Time.deltaTime);
    }

    private void Move()
    {
        Vector2 inputDir = _movement.ReadValue<Vector2>();

        if (inputDir.magnitude > 0.1f) // Nếu có nhấn di chuyển
        {
            animator.Play("Jog");
            
            Vector3 moveDir;
            
            if (_mainCamera != null)
            {
                // 1. Tính toán hướng di chuyển dựa trên Camera
                Vector3 camForward = _mainCamera.forward;
                Vector3 camRight = _mainCamera.right;

                // Triệt tiêu trục Y (độ cao) để nhân vật không ngửa lên/xuống
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                // Hướng di chuyển tương ứng với phím bấm và Camera
                moveDir = camForward * inputDir.y + camRight * inputDir.x;
            }
            else
            {
                // Fallback nếu không có camera: di chuyển theo hướng cục bộ
                moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
            }

            // 2. Di chuyển nhân vật
            characterController.Move(moveDir * walkSpeed * Time.deltaTime);

            // 3. Xoay mặt nhân vật về hướng di chuyển
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else if (_movement.WasReleasedThisFrame())
        {
            // Trở về Idle khi buông phím
            animator.Play("Idle");
        }
    }

    private void Jump()
    {
        if (_jump.WasPressedThisFrame() && characterController.isGrounded)
        {
            animator.Play("Jump");
            
            // Calculate jump impulse
            _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
