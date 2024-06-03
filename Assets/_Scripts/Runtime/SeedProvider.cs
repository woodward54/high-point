using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SeedProvider : Singleton<SeedProvider>
{
    [Tooltip("The seed used globally.")]
    [field: SerializeField] public int Seed { get; private set; } = 0;

    [SerializeField] bool _randomizeSeed = true;

    protected override void OnAwake()
    {
        CycleSeed();
    }

    public int CycleSeed()
    {
        if (_randomizeSeed)
            Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        return Seed;
    }
}