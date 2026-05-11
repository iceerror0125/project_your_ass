using StateMachine.Helpers;
using UnityEngine;

namespace StateMachine.State
{
    public class JumpState : EntityState
    {
        private readonly MovementHelper _movement = new();
        private readonly GravityHelper _gravity = new();

        /// <summary>
        /// Hệ số giảm tốc độ ngang khi bay (0.0 = không di chuyển, 1.0 = full speed).
        /// 0.8f cho cảm giác air control vừa phải.
        /// </summary>
        private const float AirControlFactor = 0.8f;

        public override void Enter()
        {
            Data.animator.Play("Jump");
            
            float jumpVelocity = Mathf.Sqrt(Data.stat.jumpForce * -2f * Physics.gravity.y);
            Debug.Log("Init velocity: " + jumpVelocity);
            _gravity.SetInitialVelocity(jumpVelocity);
        }

        public override void Exit() { }

        public override void Update()
        {
            // Di chuyển ngang khi đang bay (air control)
            Vector2 inputDir = Data.playerInput.Movement.ReadValue<Vector2>();
            Vector3 moveDir = _movement.GetMoveDirection(inputDir, Data.cameraTrans, Data.playerTransform);
            
            float airSpeed = Data.stat.speed * AirControlFactor;
            _movement.Move(Data.characterController, moveDir, airSpeed, _gravity.VerticalVelocity);
            _movement.RotateCharacter(Data.playerTransform, moveDir);

            _gravity.ApplyGravity(Data.characterController);   

            CheckLanding();
        }
        
        private void CheckLanding()
        {
            if (Data.characterController.isGrounded && _gravity.VerticalVelocity < 0)
            {
                Debug.Log("CheckLanding: " + _gravity.VerticalVelocity);
                bool isMoving = Data.playerInput.Movement.ReadValue<Vector2>().magnitude > 0.1f;
                Data.OnStateChange?.Invoke(isMoving ? EState.Run : EState.Idle);
            }
        }
    }
}