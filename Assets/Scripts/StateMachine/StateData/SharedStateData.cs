using System;
using EntityModule;
using PlayerModule;
using UnityEngine;

namespace StateMachine.StateData
{
    /// <summary>
    /// Data class dùng chung cho TẤT CẢ states.
    /// 
    /// Thay vì tạo riêng IdleStateData, RunStateData, JumpStateData 
    /// (mỗi cái chứa duplicate fields), ta gom vào 1 class duy nhất.
    /// 
    /// - States không cần movement (IdleState) đơn giản là không dùng các field physics.
    /// - Chỉ cần init MỘT LẦN trong Entity.cs, truyền cho TẤT CẢ states.
    /// </summary>
    public class SharedStateData
    {
        // === Core — mọi state đều cần ===
        public EntityAnimator animator;
        public PlayerInput playerInput;
        public Action<EState> OnStateChange;

        // === Physics/Movement — states cần di chuyển sẽ dùng ===
        public CharacterController characterController;
        public Transform playerTransform;
        public Transform cameraTrans;
        public EntityStat stat;
    }
}
