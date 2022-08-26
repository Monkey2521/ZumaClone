public interface IScoreUpdateHandler : ISubscriber
{
    public void OnScoreUpdate(int score);
}
