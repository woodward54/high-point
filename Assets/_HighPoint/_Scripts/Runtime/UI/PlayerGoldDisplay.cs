using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGoldDisplay : GoldDisplay
{
    void Start()
    {
        Setup(10000, Player.Instance.Gold);

        if (Blackboard.Instance.GoldStolen > 0) 
        {
           StartCoroutine(GiveGoldAfter());
        }
    }

    IEnumerator GiveGoldAfter()
    {
        // TODO play animation here

        // Hack to make the gold tick up slower after a battle
        var prevTransTime = _transitionTime;
        var slowerTransTime = 4f;

        yield return new WaitForSeconds(3f);
        _transitionTime = slowerTransTime;

        var newAmount = Player.Instance.Gold + Blackboard.Instance.GoldStolen;
        Player.Instance.SetGoldCount(newAmount);
        Blackboard.Instance.GoldStolen = 0;
        
        yield return new WaitForSeconds(slowerTransTime);
        _transitionTime = prevTransTime;
    }

    void OnEnable()
    {
        Player.OnGoldChanged += HandleGoldCountUpdated;
    }

    void OnDisable()
    {
        Player.OnGoldChanged -= HandleGoldCountUpdated;
    }

    void HandleGoldCountUpdated(int gold)
    {
        SetGoldValue(gold);
    }
}