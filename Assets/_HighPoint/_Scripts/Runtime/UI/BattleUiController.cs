using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleUiController : Singleton<BattleUiController>
{
    [SerializeField] GameObject SearchingForFloorUi;
    [SerializeField] GameObject ScanningUi;
    [SerializeField] GameObject BattleUi;
    [SerializeField] GameObject GameOverUi;
    [SerializeField] GameObject DebugUi;
    [SerializeField] Button EndBattleButton;

    public bool DebugMode { get; private set; }

    EventBinding<GameStateChangedEvent> GameStateChanged;

    void OnEnable()
    {
        GameStateChanged = new EventBinding<GameStateChangedEvent>(HandleGameStateChanged);
        Bus<GameStateChangedEvent>.Register(GameStateChanged);
    }

    void OnDisable()
    {
        Bus<GameStateChangedEvent>.Unregister(GameStateChanged);
    }

    void Start()
    {
        // Force Debug mode to false to send event globally
        SetDebugMode(false);
    }

    void HandleGameStateChanged(GameStateChangedEvent @event)
    {
        DisableAll();

        switch (@event.State)
        {
            case GameState.SearchingForFloor:
                SearchingForFloorUi.SetActive(true);
                break;

            case GameState.BuildingTerrain:
                ScanningUi.SetActive(true);
                break;

            case GameState.Countdown:
                EndBattleButton.gameObject.SetActive(false);
                break;

            case GameState.Battle:
                BattleUi.SetActive(true);
                EndBattleButton.gameObject.SetActive(true);
                break;

            case GameState.GameOver:
                GameOverUi.SetActive(true);
                EndBattleButton.gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }

    void DisableAll()
    {
        SearchingForFloorUi.SetActive(false);
        ScanningUi.SetActive(false);
        BattleUi.SetActive(false);
        GameOverUi.SetActive(false);
    }

    public void ToggleDebugMode()
    {
        DebugMode = !DebugMode;

        SetDebugMode(DebugMode);
    }

    void SetDebugMode(bool value)
    {
        DebugUi.SetActive(value);

        Bus<DebugModeEvent>.Raise(new DebugModeEvent(value));
    }
}