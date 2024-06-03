using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(ParticleSystem))]
public class ReturnParticlesToPool : MonoBehaviour
{
    public ParticleSystem System { get; private set; }
    public IObjectPool<ReturnParticlesToPool> Pool;

    public void Setup(IObjectPool<ReturnParticlesToPool> pool)
    {
        Pool = pool;

        System = GetComponent<ParticleSystem>();
        var main = System.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
        Pool.Release(this);
    }
}