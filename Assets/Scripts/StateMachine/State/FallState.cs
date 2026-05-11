using StateMachine.Helpers;
using UnityEngine;

namespace StateMachine.State
{
    /// <summary>
    /// State rơi tự do — khi nhân vật rời mặt đất mà không nhảy
    /// (VD: đi ra khỏi mép vực, bị đẩy ra khỏi platform).
    /// 
    /// Giống JumpState nhưng KHÔNG có lực nhảy ban đầu.
    /// AirControlFactor thấp hơn (0.6f) vì rơi tự do nên ít kiểm soát hơn.
    /// 
    /// Transitions:
    /// - → RunState: chạm đất + đang giữ phím di chuyển
    /// - → IdleState: chạm đất + không giữ phím
    /// </summary>
    public class FallState : EntityState
    {
        private readonly MovementHelper _movement = new();
        private readonly GravityHelper _gravity = new();

        private const float AirControlFactor = 0.6f;

        public override void Enter()
        {
            Data.animator.Play("Fall");
            // Không set initial velocity — nhân vật chỉ rơi tự nhiên
            _gravity.Reset();
        }

        public override void Exit() { }

        public override void Update()
        {
            Vector2 inputDir = Data.playerInput.Movement.ReadValue<Vector2>();
            Vector3 moveDir = _movement.GetMoveDirection(inputDir, Data.cameraTrans, Data.playerTransform);

            _gravity.ApplyGravity(Data.characterController);

            float airSpeed = Data.stat.speed * AirControlFactor;
            _movement.Move(Data.characterController, moveDir, airSpeed, _gravity.VerticalVelocity);
            _movement.RotateCharacter(Data.playerTransform, moveDir);

            CheckLanding();
        }

        private void CheckLanding()
        {
            if (Data.characterController.isGrounded)
            {
                bool isMoving = Data.playerInput.Movement.ReadValue<Vector2>().magnitude > 0.1f;
                Data.OnStateChange?.Invoke(isMoving ? EState.Run : EState.Idle);
            }
        }
    }
}