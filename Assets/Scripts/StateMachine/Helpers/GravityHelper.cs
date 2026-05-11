using UnityEngine;

namespace StateMachine.Helpers
{
    /// <summary>
    /// Quản lý logic trọng lực — dùng composition thay vì kế thừa.
    /// State nào cần gravity thì tạo instance helper này, state không cần thì bỏ qua.
    /// 
    /// Cách dùng trong state:
    ///   private readonly GravityHelper _gravity = new();
    ///   // Trong Update():
    ///   _gravity.ApplyGravity(Data.characterController);
    ///   _movement.Move(..., _gravity.VerticalVelocity);
    /// </summary>
    public class GravityHelper
    {
        private float _verticalVelocity;
        
        /// <summary>
        /// Lực hút nhẹ khi đứng trên mặt đất (-2f).
        /// Giữ nhân vật dính đất, tránh nảy khi đi trên dốc.
        /// </summary>
        private const float GroundedPullDown = -2f;
        
        /// <summary>
        /// Vận tốc trục Y hiện tại. Dương = bay lên, Âm = rơi xuống.
        /// Truyền giá trị này vào MovementHelper.Move() để kết hợp với di chuyển ngang.
        /// </summary>
        public float VerticalVelocity => _verticalVelocity;

        /// <summary>
        /// Tính toán gravity mỗi frame.
        /// - Chạm đất: giữ lực hút nhẹ (-2f) để không nảy trên dốc
        /// - Lơ lửng: tăng tốc rơi theo gia tốc trọng trường (Physics.gravity.y * dt)
        /// </summary>
        public void ApplyGravity(CharacterController cc)
        {
            if (cc.isGrounded)
                _verticalVelocity = GroundedPullDown;
            else
                _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        /// <summary>
        /// Set vận tốc ban đầu cho trục Y.
        /// Gọi trong JumpState.Enter() để tạo lực nhảy:
        ///   _gravity.SetInitialVelocity(Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y));
        /// </summary>
        public void SetInitialVelocity(float velocity)
        {
            _verticalVelocity = velocity;
        }

        /// <summary>Reset vận tốc về 0. Gọi khi Enter() state mới.</summary>
        public void Reset() => _verticalVelocity = 0f;
    }
}
