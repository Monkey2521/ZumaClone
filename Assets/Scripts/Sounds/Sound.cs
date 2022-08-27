using UnityEngine;

[System.Serializable]
public sealed class Sound
{
    [SerializeField] private SoundNames _name;
    [SerializeField] private SoundTypes _soundType;
    [SerializeField] private AudioClip _clip;

    public SoundNames Name => _name;
    public SoundTypes Type => _soundType;
    public AudioClip Clip => _clip;
}