using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Blackboard : SingletonPersistent<Blackboard>
{
    [field: SerializeField] public List<AgentConfig> AllUnits { get; private set; }

    [SerializeField] string _wayspotIdentifier = "Default Seed";

    public BaseConfig BaseConfig;

    public int GoldStolen;

    public string WayspotIdentifier
    {
        get { return _wayspotIdentifier; }
        set
        {
            _wayspotIdentifier = value;
            UnityEngine.Random.InitState(Seed);
        }
    }

    public int Seed => GetSimpleHash(_wayspotIdentifier);

    // Deterministic Hash
    private static int GetSimpleHash(string s)
    {
        return s.Select(a => (int)a).Sum();
    }

    protected override void OnAwake()
    {
        Application.targetFrameRate = 30;
        // Application.targetFrameRate = 60;

        DOTween.Init(true, true, LogBehaviour.Default).SetCapacity(1250, 10);
    }
}