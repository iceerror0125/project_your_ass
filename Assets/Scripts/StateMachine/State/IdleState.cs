using UnityEngine;

namespace StateMachine.State
{
    /// <summary>
    /// State đứng yên — KHÔNG cần MovementHelper/GravityHelper.
    /// 
    /// Đây là ưu điểm của composition: state không cần logic nào thì không gắn helper.
    /// Chỉ play animation "Idle" và lắng nghe input để chuyển state.
    /// 
    /// Transitions:
    /// - → RunState: khi nhấn phím di chuyển (magnitude > 0.1f)
    /// - → JumpState: khi nhấn Jump
    /// </summary>
    public class IdleState : EntityState
    {
        public override void Enter()
        {
            Data.animator.Play("Idle");
        }

        public override void Exit() { }

        public override void Update()
        {
            // Nhấn phím di chuyển → Run
            if (Data.playerInput.Movement.ReadValue<Vector2>().magnitude > 0.1f)
            {
                Data.OnStateChange?.Invoke(EState.Run);
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