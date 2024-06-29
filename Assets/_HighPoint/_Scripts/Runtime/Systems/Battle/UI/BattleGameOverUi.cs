using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class BattleGameOverUi : MonoBehaviour
{
    [SerializeField] TMP_Text _mainText;
    [SerializeField] TMP_Text _goldStolen;
    [SerializeField] TMP_Text _percent;

    // This was not working consistently
    // void OnEnable()
    // {
    //     GameManager.Instance.OnGameOverStatsReady += PopulateModal;
    // }

    // void OnDisable()
    // {
    //     GameManager.Instance.OnGameOverStatsReady -= PopulateModal;
    // }

    void Update()
    {
        PopulateModal();
    }

    void PopulateModal()
    {
        _goldStolen.text = GameManager.Instance.TotalGoldStolen.ToString("N0") + " Stolen";
        _percent.text = GameManager.Instance.CompletionPercent.ToString("N0") + "% Completion";

        var didWin = GameManager.Instance.DidWin;

        _mainText.text = didWin ? "You Won!" : "You lost...";
        _mainText.color = didWin ? Color.green : Color.red;
    }
}
