using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Systems.SceneManagement;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeController : MonoBehaviour
{
    [SerializeField] List<NarrativeElement> _sequence;
    [SerializeField] float _fadeTime = 1.5f;

    int _sequenceIndex = 0;
    NarrativeElement _activeElement;

    // void OnEnable()
    // {
    //     TouchController.OnUiTouch += HandleTouch;
    // }

    // void OnDisable()
    // {
    //     TouchController.OnUiTouch -= HandleTouch;
    // }

    void Start()
    {
        _sequence = GetComponentsInChildren<NarrativeElement>().ToList();
        _sequence.ForEach(g => g.CanvasGroup.alpha = 0f);

        _activeElement = _sequence[0];
        _activeElement.CanvasGroup.DOFade(1f, _fadeTime);

        StartCoroutine(AutoProgressSequence());
    }

    IEnumerator AutoProgressSequence()
    {
        yield return new WaitForSeconds(_activeElement.DisplayTime);

        while (_sequenceIndex < _sequence.Count)
        {
            ProgressSequence();
            yield return new WaitForSeconds(_activeElement.DisplayTime);
        }
    }

    // void HandleTouch(Vector2 vector)
    // {
    //     ProgressSequence();
    // }

    void ProgressSequence()
    {
        // Fadeout
        _activeElement.CanvasGroup.DOFade(0f, _fadeTime);

        // Fade in next
        _sequenceIndex++;

        if (_sequenceIndex >= _sequence.Count) return;

        _activeElement = _sequence[_sequenceIndex];
        _activeElement.CanvasGroup.DOFade(1f, _fadeTime).SetDelay(_fadeTime);
    }
}