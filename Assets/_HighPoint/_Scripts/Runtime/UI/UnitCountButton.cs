using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCountButton : MonoBehaviour
{
    [SerializeField] TMP_Text _countTxt;

    void Awake()
    {
        _countTxt.text = "";
    }

    public void SetCount(int count)
    {
        _countTxt.text = count.ToString();
    }
}