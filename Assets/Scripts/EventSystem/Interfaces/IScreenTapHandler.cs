using UnityEngine;

public interface IScreenTapHandler : ISubscriber
{
    public void OnScreenTap(Vector3 point);
}
