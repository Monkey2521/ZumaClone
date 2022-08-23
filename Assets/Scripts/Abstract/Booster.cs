using UnityEngine;

public abstract class Booster : ScriptableObject
{
    [SerializeField] protected int _cooldown;

    public abstract void MakeEffect();
}
