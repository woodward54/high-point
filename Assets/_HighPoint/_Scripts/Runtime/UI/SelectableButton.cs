using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableButton : MonoBehaviour
{
    [SerializeField] GameObject _selection;

    public void Select()
    {
        _selection.SetActive(true);
    }

    public void Deselect()
    {
        _selection.SetActive(false);
    }
}