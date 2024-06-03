using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelector : Singleton<UnitSelector>
{
    [SerializeField] Transform _unitButtonPrefab;
    [SerializeField] Transform _unitContent;
    [SerializeField] List<AgentConfig> _units;

    public AgentConfig SelectedUnit { get; private set; }

    readonly List<Button> _buttons = new();

    void Start()
    {
        foreach (var unit in _units)
        {
            var btnGo = Instantiate(_unitButtonPrefab, Vector3.zero, Quaternion.identity, _unitContent);
            btnGo.GetComponent<Image>().sprite = unit.UiIcon;

            var btn = btnGo.GetComponent<Button>();

            btn.onClick.AddListener(delegate { HandleClick(btn, unit); });

            _buttons.Add(btn);
        }

        DeselectAll();
    }

    void HandleClick(Button btn, AgentConfig selectedUnit)
    {
        SelectedUnit = selectedUnit;

        DeselectAll();

        btn.GetComponent<SelectableButton>().Select();
    }

    void DeselectAll()
    {
        foreach (var btn in _buttons)
        {
            btn.GetComponent<SelectableButton>().Deselect();
        }
    }
}
