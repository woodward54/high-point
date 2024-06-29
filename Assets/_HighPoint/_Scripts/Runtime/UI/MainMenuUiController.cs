using System.Collections.Generic;
using DG.Tweening;
using Systems.SceneManagement;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiController : MonoBehaviour
{
    [SerializeField] List<CanvasGroup> _sequence;

    int _sequenceIndex = 0;
    CanvasGroup _activeGroup;

    void Start()
    {
        _activeGroup = _sequence[0];

        _sequence.ForEach(g => g.alpha = 0f);
        _sequence.ForEach(g => g.gameObject.SetActive(false));

        // Starting state
        _activeGroup.gameObject.SetActive(true);
        _activeGroup.alpha = 1f;
    }

    public void ProgressSequence()
    {
        var fadeMeOut = _activeGroup;
        fadeMeOut.DOFade(0f, 2f).OnComplete(() => fadeMeOut.gameObject.SetActive(false));

        _sequenceIndex++;
        _activeGroup = _sequence[_sequenceIndex];

        _activeGroup.gameObject.SetActive(true);
        _activeGroup.DOFade(1f, 2f);
    }

    public void PlayGameButton()
    {
        SceneLoader.Instance.LoadSceneGroup(SceneGroupIds.MAP);
    }
}