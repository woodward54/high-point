using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PurchaseUnitCardManager : MonoBehaviour
{
    [SerializeField] Transform _cardContainer;
    [SerializeField] PurchaseUnitCard _cardPrefab;

    CanvasGroup _canvas;
    bool _isWindowOpen;

    void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        transform.localScale = Vector3.zero;

        _canvas.blocksRaycasts = false;
        _canvas.interactable = false;
    }

    void Start()
    {
        foreach (var unit in Blackboard.Instance.AllUnits)
        {
            var card = Instantiate(_cardPrefab.transform, _cardContainer);
            card.GetComponent<PurchaseUnitCard>().Setup(unit);
        }
    }

    public void ToggleWindow()
    {
        _isWindowOpen = !_isWindowOpen;

        _canvas.blocksRaycasts = _isWindowOpen;
        _canvas.interactable = _isWindowOpen;

        transform.DOScale(_isWindowOpen ? Vector3.one : Vector3.zero, 0.3f)
                    .SetEase(_isWindowOpen ? Ease.OutBack : Ease.InBack);
    }
}