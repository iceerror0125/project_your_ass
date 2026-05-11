namespace StateMachine.State
{
    /// <summary>
    /// Registry giữ singleton instances của tất cả states.
    /// Tránh tạo mới state object mỗi lần switch — tái sử dụng.
    /// </summary>
    public class StateRegistry
    {
        public IdleState Idle { get; } = new();
        public RunState Run { get; } = new();
        public JumpState Jump { get; } = new();
        public FallState Fall { get; } = new();
    }
}