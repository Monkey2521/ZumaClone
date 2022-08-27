using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour, IPoolable
{
    [Header("Debug settings")]
    [SerializeField] private bool _isDebug;

    [Header("Settings")]
    [SerializeField] private AudioSource _source;

    private MonoPool<AudioPlayer> _pool;

    public void Construct(MonoPool<AudioPlayer> pool)
    {
        _pool = pool;
    }

    public void Init(AudioMixerGroup mixerGroup)
    {
        _source.outputAudioMixerGroup = mixerGroup;
    }

    public void ResetObject()
    {
        _pool = null;
        _source.outputAudioMixerGroup = null;
        _source.clip = null;
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) 
        {
            _pool.ReleaseObject(this);

            return;
        }

        _source.clip = clip;

        _source.Play();

        StartCoroutine(ReleaseDelay(clip.length));
    }

    private IEnumerator ReleaseDelay(float time)
    {
        yield return new WaitForSeconds(time);

        if (_pool == null)
        {
            if (_isDebug) Debug.Log("Not initialized AudioPlayer!");
        }
        else
        {
            _source.clip = null;
            _pool.ReleaseObject(this);
        }

    }
}
