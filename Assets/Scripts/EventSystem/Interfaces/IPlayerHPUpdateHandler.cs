public interface IPlayerHPUpdateHandler : ISubscriber
{
    public void OnPlayerHPUpdate(int hp, int maxHP);
}
