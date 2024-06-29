using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldDisplay : Singleton<GoldDisplay>
{
    [SerializeField] Image _fill;
    [SerializeField] TMP_Text _count;

    protected int _maxGold = 0;
    protected int _currGold = 0;
    protected float _transitionTime = 0.5f;

    int _tweenValue = 0;
    bool _showMaxValue;

    protected override void OnAwake()
    {
        UpdateUi();
    }

    public void Setup(int maxGold, int currentGold = 0, bool showMaxValue = false)
    {
        _maxGold = maxGold;
        _showMaxValue = showMaxValue;
        _currGold = currentGold;
        UpdateUi();
    }

    public void SetGoldValue(int gold)
    {
        _currGold = gold;
        UpdateUi(_transitionTime);
    }

    protected void UpdateUi(float duration = 0f)
    {
        var ease = Ease.Linear;

        var percent = _maxGold == 0 ? 0f : _currGold / (float)_maxGold;
        Mathf.Clamp(percent, 0f, 1);

        _fill.DOFillAmount(percent, duration).SetEase(ease);

        DOTween.To(() => _tweenValue, x => _tweenValue = x, _currGold, duration)
                .SetEase(ease)
                .OnUpdate(() =>
                {
                    _count.text = _tweenValue.ToString("N0");
                    _count.text += _showMaxValue ? "/" + _maxGold.ToString("N0") : "";
                });
    }
}