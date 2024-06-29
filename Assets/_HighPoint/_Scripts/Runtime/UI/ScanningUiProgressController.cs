using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScanningUiProgressController : Singleton<ScanningUiProgressController>
{
    // TODO: change this to minimum tiles
    [SerializeField][Range(0, 1)] float _scanPercentageNeeded = 0.5f;
    [SerializeField] Transform _doneScanningButton;
    [SerializeField] Image _scanningProgressImg;
    [SerializeField] TMP_Text _scanText;

    bool _scanningRequirementMet = false;

    void Start()
    {
        _scanningProgressImg.fillAmount = 0f;
        _doneScanningButton.gameObject.SetActive(false);
    }

    void Update()
    {
        var numOfCells = HexGrid.Instance.InstantiatedCellCount;
        var totalCells = HexGrid.Instance.Depth * HexGrid.Instance.Width;
        var progress = Mathf.Clamp(numOfCells / (float)totalCells, 0f, 1f);

        if (progress > _scanningProgressImg.fillAmount)
        {
            _scanningProgressImg.DOFillAmount(progress, 1f);
            var progressInt = Mathf.RoundToInt(progress * 100f);
            _scanText.text = "Scan Progress (" + progressInt + "%)";
        }

        if (_scanningRequirementMet) return;

        if (progress >= _scanPercentageNeeded)
        {
            _doneScanningButton.gameObject.SetActive(true);
            _scanningRequirementMet = true;
        }
    }
}