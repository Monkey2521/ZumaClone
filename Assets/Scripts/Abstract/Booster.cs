using UnityEngine;

public abstract class Booster : ScriptableObject
{
    [SerializeField, Range(1, 120)] protected int _cooldown;

    public float Cooldown => _cooldown;

    public abstract void MakeEffect();
}
