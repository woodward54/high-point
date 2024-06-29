using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelector : Singleton<UnitSelector>
{
    [SerializeField] Transform _unitButtonPrefab;
    [SerializeField] Transform _unitContent;

    public AgentConfig SelectedUnit { get; private set; }

    readonly List<Button> _buttons = new();

    void Start()
    {
        foreach (var unit in Blackboard.Instance.AllUnits)
        {
            var btnGo = Instantiate(_unitButtonPrefab, Vector3.zero, Quaternion.identity, _unitContent);
            btnGo.GetComponent<Image>().sprite = unit.UiIcon;

            var btn = btnGo.GetComponent<Button>();

            btn.name = unit.name;

            btn.onClick.AddListener(delegate { HandleClick(btn, unit); });

            _buttons.Add(btn);

            UnitInventory.Instance.RegisterUnitType(unit);
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

    public void UpdateUnitButtonCount(AgentConfig unit, int count)
    {
        var btn = _buttons.Where(b => b.name == unit.name).First();
        btn.GetComponent<UnitCountButton>().SetCount(count);

        if (count <= 0)
        {
            SelectedUnit = null;
            btn.GetComponent<SelectableButton>().Deselect();
        }

        btn.interactable = count > 0;
    }
}
