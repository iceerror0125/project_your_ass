using StateMachine.Helpers;
using UnityEngine;

namespace StateMachine.State
{
    /// <summary>
    /// State chạy/di chuyển trên mặt đất.
    /// 
    /// Sử dụng MovementHelper + GravityHelper (composition) thay vì kế thừa GroundState.
    /// Logic move/gravity được delegate hoàn toàn cho helpers → không duplicate code.
    /// 
    /// Transitions:
    /// - → IdleState: khi thả phím di chuyển
    /// - → JumpState: khi nhấn Jump
    /// </summary>
    public class RunState : EntityState
    {
        private readonly MovementHelper _movement = new();
        private readonly GravityHelper _gravity = new();

        public override void Enter()
        {
            Data.animator.Play("Jog");
            _gravity.Reset();
        }

        public override void Exit() { }

        public override void Update()
        {
            ApplyMovement();
            CheckChangeState();
        }

        /// <summary>
        /// Đọc input → tính hướng theo camera → áp gravity → move + rotate.
        /// Tất cả được delegate cho helpers, state chỉ orchestrate.
        /// </summary>
        private void ApplyMovement()
        {
            Vector2 inputDir = Data.playerInput.Movement.ReadValue<Vector2>();
            Vector3 moveDir = _movement.GetMoveDirection(inputDir, Data.cameraTrans, Data.playerTransform);

            _gravity.ApplyGravity(Data.characterController);
            _movement.Move(Data.characterController, moveDir, Data.stat.speed, _gravity.VerticalVelocity);
            _movement.RotateCharacter(Data.playerTransform, moveDir);
        }

        private void CheckChangeState()
        {
            // Thả phím → Idle
            if (Data.playerInput.Movement.ReadValue<Vector2>().magnitude < 0.1f)
            {
                Data.OnStateChange?.Invoke(EState.Idle);
                return;
            }

            // Nhấn Jump → Jump
            if (Data.playerInput.Jump.WasPressedThisFrame())
            {
                Data.OnStateChange?.Invoke(EState.Jump);
            }
        }
    }
}