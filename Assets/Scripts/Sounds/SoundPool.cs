using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPool : MonoBehaviour, ISoundPlayHandler
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Pool settings")]
    [SerializeField] private AudioPlayer _playerPrefab;
    [SerializeField, Range(1, 100)] private int _poolCapacity;

    [Header("Mixers settings")]
    [SerializeField] private List<SoundMixer> _soundMixers = new List<SoundMixer>();

    private MonoPool<AudioPlayer> _pool;
    public bool Initialized => _pool != null;

    private void OnEnable()
    {
        EventBus.Subscribe(this);

        CreatePool();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }


    public void CreatePool()
    {
        if (Initialized)
        {
            _pool.ClearPool();
        }

        Transform poolParent = new GameObject("AudioPlayerPool").transform;

        _pool = new MonoPool<AudioPlayer>(_playerPrefab, _poolCapacity, poolParent);

        foreach(var obj in _pool.Objects)
        {
            obj.Construct(_pool);
        }
    }

    public void OnSoundPlay(Sound sound)
    {
        AudioMixerGroup mixer = GetMixer(sound.Type);

        if (mixer == null)
        {
            if (_isDebug) Debug.Log("Missing mixer!");

            return;
        }

        var player = _pool.PullObject();
        player.transform.parent = transform;

        player.Init(mixer);
        player.Play(sound.Clip);
    }

    private AudioMixerGroup GetMixer(SoundTypes soundType)
    {
        foreach(var soundMixer in _soundMixers)
        {
            if (soundMixer.Type == soundType)
            {
                return soundMixer.Mixer.outputAudioMixerGroup;
            }
        }
        return null;
    }

    [System.Serializable]
    private class SoundMixer
    {
        [SerializeField] private SoundTypes _soundType;
        [SerializeField] private AudioMixer _audioMixer;

        public SoundTypes Type => _soundType;
        public AudioMixer Mixer => _audioMixer;
    }
}
