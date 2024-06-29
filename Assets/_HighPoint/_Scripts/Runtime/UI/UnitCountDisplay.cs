using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class UnitCountDisplay : MonoBehaviour
{
    TMP_Text _unitsText;

    void Awake()
    {
        _unitsText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        UpdateUnitCount();
    }

    void UpdateUnitCount()
    {
        var count = UnitManager.Instance.Units.Count();
        _unitsText.text = "Units: " + count;
    }
}