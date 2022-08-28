using System.Collections;
using UnityEngine;

public class StarParticle : MonoBehaviour, IPoolable
{
    [SerializeField] private ParticleSystem _particles;

    private MonoPool<StarParticle> _pool;

    public void Construct(MonoPool<StarParticle> pool)
    {
        _pool = pool;
    }

    public void ResetObject() 
    {
    }

    public void Play(int maxParticles)
    {
        Debug.Log(maxParticles + " " + (short)maxParticles);
        _particles.maxParticles = maxParticles;
        _particles.emission.SetBurst(0, new ParticleSystem.Burst(0f, 0, (short)maxParticles, 1, 0f));
        _particles.Play();

        StartCoroutine(WaitRelease());
    }

    private IEnumerator WaitRelease()
    {
        yield return new WaitForSeconds(_particles.main.duration);
        _pool.ReleaseObject(this);
    }
}
