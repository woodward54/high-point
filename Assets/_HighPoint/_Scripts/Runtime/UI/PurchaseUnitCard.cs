using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseUnitCard : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] TMP_Text _countTxt;
    [SerializeField] TMP_Text _priceTxt;
    [SerializeField] Button _buyButton;

    AgentConfig _cfg;

    void OnEnable()
    {
        Player.OnUnitCountChanged += HandleUnitCountUpdated;
        Player.OnGoldChanged += HandleGoldCountUpdated;
    }

    void OnDisable()
    {
        Player.OnUnitCountChanged -= HandleUnitCountUpdated;
        Player.OnGoldChanged -= HandleGoldCountUpdated;
    }

    public void Setup(AgentConfig cfg)
    {
        _cfg = cfg;

        _image.sprite = cfg.UiIcon;
        _priceTxt.text = cfg.Price.ToString();
        _countTxt.text = Player.Instance.GetUnitCount(cfg.name).ToString();
        _buyButton.interactable = Player.Instance.Gold >= _cfg.Price;

        _buyButton.onClick.AddListener(delegate { HandleClick(); });
    }

    void HandleClick()
    {
        if (Player.Instance.Gold < _cfg.Price) return;

        Player.Instance.SetGoldCount(Player.Instance.Gold - _cfg.Price);
        Player.Instance.SetUnitCount(_cfg.name, Player.Instance.GetUnitCount(_cfg.name) + 1);
    }

    void HandleUnitCountUpdated(UnitCount unitCount)
    {
        if (unitCount.Name != _cfg.name) return;

        _countTxt.text = unitCount.Count.ToString();
    }

    void HandleGoldCountUpdated(int gold)
    {
        _buyButton.interactable = gold >= _cfg.Price;
    }
}