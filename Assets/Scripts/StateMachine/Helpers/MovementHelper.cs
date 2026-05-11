using UnityEngine;

namespace StateMachine.Helpers
{
    /// <summary>
    /// Quản lý logic di chuyển — tính hướng theo camera, move, rotate.
    /// State nào cần movement thì tạo instance helper này.
    /// 
    /// Cách dùng trong state:
    ///   private readonly MovementHelper _movement = new();
    ///   // Trong Update():
    ///   Vector3 moveDir = _movement.GetMoveDirection(inputDir, cameraTrans, playerTransform);
    ///   _movement.Move(cc, moveDir, speed, gravity.VerticalVelocity);
    ///   _movement.RotateCharacter(playerTransform, moveDir);
    /// </summary>
    public class MovementHelper
    {
        /// <summary>
        /// Tính hướng di chuyển tương đối với camera.
        /// - Có camera: input × hướng camera (triệt tiêu trục Y)
        /// - Không camera: fallback dùng hướng local nhân vật
        /// </summary>
        public Vector3 GetMoveDirection(Vector2 inputDir, Transform cameraTrans, Transform playerTransform)
        {
            if (cameraTrans != null)
            {
                Vector3 camForward = cameraTrans.forward;
                Vector3 camRight = cameraTrans.right;

                // Triệt tiêu trục Y để nhân vật chỉ di chuyển trên mặt phẳng XZ
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                return camForward * inputDir.y + camRight * inputDir.x;
            }

            // Fallback nếu không có camera
            return playerTransform.forward * inputDir.y + playerTransform.right * inputDir.x;
        }

        /// <summary>
        /// Di chuyển nhân vật: kết hợp ngang (XZ) + gravity (Y) vào 1 lần Move().
        /// QUAN TRỌNG: Chỉ gọi CharacterController.Move() MỘT LẦN mỗi frame.
        /// </summary>
        public void Move(CharacterController cc, Vector3 moveDir, float speed, float verticalVelocity)
        {
            Vector3 motion = moveDir * (speed * Time.deltaTime);
            motion.y = verticalVelocity * Time.deltaTime;
            cc.Move(motion);
        }

        /// <summary>
        /// Xoay nhân vật mượt mà về hướng di chuyển (Slerp).
        /// Chỉ xoay khi moveDir != zero để tránh lỗi LookRotation.
        /// </summary>
        public void RotateCharacter(Transform transform, Vector3 moveDir, float rotationSpeed = 10f)
        {
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
