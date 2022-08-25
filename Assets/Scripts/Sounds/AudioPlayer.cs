using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
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

    public void Init(AudioMixerGroup mixer)
    {
        _source.outputAudioMixerGroup = mixer;
    }

    public void Play(AudioClip clip)
    {
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
