using StateMachine.StateData;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Base class cho tất cả states trong state machine.
    /// 
    /// Mỗi state implement 3 methods:
    /// - Enter(): gọi khi state bắt đầu (play animation, init)
    /// - Update(): gọi mỗi frame (logic chính)
    /// - Exit(): gọi khi rời state (cleanup)
    /// 
    /// State truy cập SharedStateData qua property Data.
    /// SharedStateData được set bởi StateController khi switch state.
    /// </summary>
    public abstract class EntityState
    {
        private SharedStateData _data;

        /// <summary>
        /// Data dùng chung cho tất cả states.
        /// Truy cập: Data.animator, Data.playerInput, Data.characterController, v.v.
        /// </summary>
        protected SharedStateData Data
        {
            get
            {
                if (_data != null) return _data;
                Debug.LogWarning("EntityState " + this.GetType().Name + " has no state data");
                return null;
            }
        }

        /// <summary>
        /// Gắn SharedStateData cho state. Được gọi bởi StateController khi switch state.
        /// </summary>
        public void SetStateData(SharedStateData newData)
        {
            this._data = newData;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
    }
}