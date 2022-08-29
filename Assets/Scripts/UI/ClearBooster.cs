using UnityEngine;

[CreateAssetMenu(menuName = "Boosters/Clear booster", fileName = "New clear booster")]
public class ClearBooster : Booster
{ 
    public override void MakeEffect()
    {
        EventBus.Publish<IClearPathHandler>(handler => handler.OnClearPath());
    }
}
