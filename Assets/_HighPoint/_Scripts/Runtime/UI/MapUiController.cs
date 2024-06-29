using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Persistence;
using System.Threading.Tasks;
using Systems.SceneManagement;

public class MapUiController : Singleton<MapUiController>
{
    [SerializeField] CanvasGroup SelectedMarkerUi;

    readonly List<CanvasGroup> _allCanvases = new();

    EventBinding<MapMarkerSelected> MapMarkerSelected;

    void OnEnable()
    {
        MapMarkerSelected = new EventBinding<MapMarkerSelected>(HandleMapMarkerSelected);
        Bus<MapMarkerSelected>.Register(MapMarkerSelected);
    }

    void OnDisable()
    {
        Bus<MapMarkerSelected>.Unregister(MapMarkerSelected);
    }

    void Start()
    {
        _allCanvases.Add(SelectedMarkerUi);

        DisableAll(0f);
    }

    void HandleMapMarkerSelected(MapMarkerSelected @event)
    {
        var allButMarkerUi = _allCanvases.Where(c => c != SelectedMarkerUi).ToList();
        allButMarkerUi.ForEach(c => Disable(c));

        if (@event.Selected != null)
        {
            Enable(SelectedMarkerUi);
        }
        else
        {
            Disable(SelectedMarkerUi, 0f);
        }
    }

    void DisableAll(float fadeTime = 0.5f)
    {
        _allCanvases.ForEach(c => Disable(c, fadeTime));
    }

    void Disable(CanvasGroup canvas, float fadeTime = 0.5f)
    {
        canvas.DOFade(0f, fadeTime).OnComplete(() =>
        {
            SelectedMarkerUi.interactable = false;
            SelectedMarkerUi.blocksRaycasts = false;
        });
    }

    void Enable(CanvasGroup canvas, float fadeTime = 0.5f)
    {
        canvas.DOFade(1f, fadeTime).OnComplete(() =>
        {
            SelectedMarkerUi.interactable = true;
            SelectedMarkerUi.blocksRaycasts = true;
        });
    }

    public async void DeleteAccountButton()
    {
        SaveLoadSystem.Instance.DeleteGame();
        SaveLoadSystem.Instance.LoadGame();

        ToastMessage.Instance.EnqueueMessage("Restarting game...", 2f, true);

        await Task.Delay(2400);

        SceneLoader.Instance.LoadSceneGroup(SceneGroupIds.MAIN_MENU);
    }
}