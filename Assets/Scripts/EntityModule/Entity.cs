using StateMachine;
using StateMachine.State;
using StateMachine.StateData;
using UnityEngine;

namespace EntityModule
{
    /// <summary>
    /// Entity chính — quản lý state machine và dữ liệu nhân vật.
    /// 
    /// THAY ĐỔI CHÍNH sau refactor:
    /// - Dùng SharedStateData duy nhất thay vì IdleStateData/RunStateData/JumpStateData riêng
    /// - Init data 1 lần, truyền cho TẤT CẢ states
    /// - ChangeState() đã đầy đủ tất cả cases (fix bug thiếu Jump/Fall)
    /// </summary>
    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityComponents _components;

        private EntityAnimator _entityAnimator;
        private EntityStat _stat;

        #region State

        private StateRegistry _stateRegistry;
        private StateController _stateController;

        #endregion

        /// <summary>Data dùng chung cho tất cả states — chỉ cần 1 instance.</summary>
        private SharedStateData _sharedData;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _stateRegistry = new StateRegistry();
            _entityAnimator = new EntityAnimator(_components.Animator);
            _stat = new EntityStat();

            // Init SharedStateData MỘT LẦN — tất cả states dùng chung object này
            _sharedData = new SharedStateData
            {
                animator = _entityAnimator,
                playerInput = _components.PlayerInput,
                characterController = _components.CharacterController,
                cameraTrans = _components.MainCamera.transform,
                playerTransform = transform,
                stat = _stat,
                OnStateChange = ChangeState
            };

            _stateController = new StateController(_stateRegistry.Idle, _sharedData);
        }

        private void Update()
        {
            _stateController.Update();
        }

        /// <summary>
        /// Callback chuyển state — được gọi từ bên trong states qua Data.OnStateChange.
        /// 
        /// FIX BUG: Trước đây thiếu case Jump → gọi OnStateChange(EState.Jump) 
        /// rơi vào default = quay về Idle thay vì chuyển sang JumpState.
        /// </summary>
        private void ChangeState(EState state)
        {
            EntityState targetState = state switch
            {
                EState.Run  => _stateRegistry.Run,
                EState.Jump => _stateRegistry.Jump,
                EState.Fall => _stateRegistry.Fall,
                _           => _stateRegistry.Idle
            };

            _stateController.SwitchState(targetState);
        }
    }
}