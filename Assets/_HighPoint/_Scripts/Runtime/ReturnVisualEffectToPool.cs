using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Pool;

[RequireComponent(typeof(VisualEffect))]
public class ReturnVisualEffectToPool : MonoBehaviour
{
    public VisualEffect VisualEffect { get; private set; }
    
    IObjectPool<ReturnVisualEffectToPool> _pool;

    bool _hasPlayed;

    public void Setup(IObjectPool<ReturnVisualEffectToPool> pool)
    {
        _pool = pool;

        VisualEffect = GetComponent<VisualEffect>();
    }

    void Update()
    {
        if (VisualEffect.aliveParticleCount == 0 && _hasPlayed)
        {
            // Return the GameObject to the object pool.
            ReturnToPool();

            // Reset the hasPlayed flag to prepare for the next time the effect is played.
            _hasPlayed = false;
            return;
        }

        // If there are alive particles, mark the effect as having been played.
        if (VisualEffect.aliveParticleCount > 0)
        {
            _hasPlayed = true;
        }
    }

    void ReturnToPool()
    {
        _pool.Release(this);
    }
}