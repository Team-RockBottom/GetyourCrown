namespace GetyourCrown.CharacterContorller
{
    public enum State
    {
        Move,
        Jump,
        Attack,
        Pick,
        Kick,
        Hit,
        Stun,
    }

    public interface IAnimationController
    {
        /// <summary>
        /// 현재 애니메이터가 실행하고있는 상태
        /// </summary>
        State currentState { get; }
        bool isTransitioning { get; }

        void ChangeState(State newState);
    }
}