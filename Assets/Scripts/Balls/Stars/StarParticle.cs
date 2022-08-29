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
        _pool = null;
    }

    public void Play(int maxParticles)
    {
        _particles.maxParticles = maxParticles;
        _particles.Play();

        StartCoroutine(WaitRelease());
    }

    private IEnumerator WaitRelease()
    {
        yield return new WaitForSeconds(_particles.main.duration);

        _pool.ReleaseObject(this);
    }
}
