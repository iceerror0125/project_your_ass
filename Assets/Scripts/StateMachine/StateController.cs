using StateMachine.StateData;

namespace StateMachine
{
    /// <summary>
    /// Điều khiển vòng đời states: Enter → Update (mỗi frame) → Exit → Enter state mới.
    /// 
    /// Sử dụng SharedStateData duy nhất — không cần truyền data riêng cho mỗi state.
    /// SwitchState() chỉ cần nhận state mới, data đã được lưu sẵn.
    /// </summary>
    public class StateController
    {
        private EntityState _currentState;
        private readonly SharedStateData _sharedData;

        public StateController(EntityState initState, SharedStateData data)
        {
            _sharedData = data;
            SetNewState(initState);
        }

        /// <summary>
        /// Chuyển sang state mới. Exit() cũ → SetData → Enter() mới.
        /// Không cần truyền data — dùng SharedStateData đã lưu từ constructor.
        /// </summary>
        public void SwitchState(EntityState newState)
        {
            _currentState.Exit();
            SetNewState(newState);
        }

        public void Update()
        {
            _currentState.Update();
        }

        private void SetNewState(EntityState newState)
        {
            _currentState = newState;
            _currentState.SetStateData(_sharedData);
            _currentState.Enter();
        }
    }
}