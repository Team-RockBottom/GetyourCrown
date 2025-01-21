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
        /// ���� �ִϸ����Ͱ� �����ϰ��ִ� ����
        /// </summary>
        State currentState { get; }
        bool isTransitioning { get; }

        void ChangeState(State newState);
    }
}