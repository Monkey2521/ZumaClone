using UnityEngine;

[System.Serializable]
public sealed class Sound
{
    [SerializeField] private SoundTypes _soundType;
    [SerializeField] private AudioClip _clip;

    public SoundTypes Type => _soundType;
    public AudioClip Clip => _clip;
}

public enum SoundTypes
{
    UI,
    GamePhase,
    Ball,
    Castle,
    Booster
}