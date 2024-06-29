using DG.Tweening;
using UnityEngine;

public class ScaleOnStart : MonoBehaviour
{
    void Start()
    {
        var startScale = transform.localScale;

        var delay = Random.Range(0f, 1.5f);
        transform.localScale = new Vector3(0f, 0f, 0f);
        transform.DOScale(startScale, 0.5f).SetEase(Ease.OutBack).SetDelay(delay);
    }
}