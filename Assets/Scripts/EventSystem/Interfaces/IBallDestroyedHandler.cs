using UnityEngine;

public interface IBallDestroyedHandler : ISubscriber
{
    public void OnBallDestroyed(Vector3 position, int ScorePerBall);
}
