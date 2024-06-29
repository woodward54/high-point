using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoldStore : BuildingUnit
{
    [field: Header("Gold Store Properties")]
    [SerializeField] Transform GoldParent;

    List<Transform> _goldGOs;

    public int CurrentGold { get; private set; }
    public int StartingGold { get; private set; }
    readonly int _maxGold = 2000;

    protected override void Start()
    {
        base.Start();


        _goldGOs = GoldParent.GetComponentsInChildren<Transform>()
                                .Where(o => o.transform != GoldParent)
                                .OrderBy(o => o.localPosition.y)
                                .ToList();

        System.Random rand = new(Blackboard.Instance.Seed);
        StartingGold = Math.Min(_maxGold, Mathf.CeilToInt(((float)rand.NextDouble() + 0.2f) * _maxGold));
        CurrentGold = StartingGold;

        GameManager.Instance.RegisterGoldStore(this);

        UpdateGoldFill();
    }

    protected override void UpdateHealth(float damage)
    {
        base.UpdateHealth(damage);

        UpdateGoldFill();
    }

    void UpdateGoldFill()
    {
        var percent = CurrentHealth / (float)MaxHealth;
        var numToFill = Mathf.CeilToInt(Mathf.Lerp(0f, _goldGOs.Count, percent));

        for (int i = 0; i < _goldGOs.Count; i++)
        {
            // _goldGOs is sorted by ascending y value, _goldGOs[0] is the obj with lowest y val
            _goldGOs[i].gameObject.SetActive(i <= numToFill);
        }

        CurrentGold = Mathf.CeilToInt(Mathf.Lerp(0f, StartingGold, percent));
        GameManager.Instance.ReportGoldStolen();
    }
}