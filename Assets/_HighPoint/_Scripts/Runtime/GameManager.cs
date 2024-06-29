using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lofelt.NiceVibrations;
using Systems.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityHFSM;

public enum GameState
{
    Initializing,
    SearchingForFloor,
    BuildingTerrain,
    BuildingBase,
    Countdown,
    Battle,
    GameOver,
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] TMP_Text _timerText;
    [SerializeField] float _gameTimeSeconds = 120f;
    [SerializeField] protected string _activeState;

    public GameState State { get; private set; }
    public int StartingEnemyUnitCount { get; private set; }
    public int TotalGoldStolen { get; private set; }
    public int CompletionPercent { get; private set; }
    public bool DidWin { get; private set; }

    public Action OnGameOverStatsReady;

    protected StateMachine<GameState> FSM;

    Coroutine _timerCoroutine;

    List<GoldStore> _goldStores = new();

    protected override void OnAwake()
    {
        // TODO: find a better way to handle this, gameobjects are getting loaded to the boatloader scene
        // when the scene is unloaded
        // Is this maybe due to the coroutine?
        if (gameObject.scene.name != "Battle") 
        {
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        _gameTimeSeconds = 1200f;
#endif

        FSM = new();
        AddStates();
        AddTransitions();
        FSM.Init();

        FSM.RequestStateChange(GameState.SearchingForFloor);
    }

    void AddStates()
    {
        FSM.AddState(GameState.Initializing);
        FSM.AddState(GameState.SearchingForFloor);
        FSM.AddState(GameState.BuildingTerrain);
        FSM.AddState(GameState.BuildingBase);
        FSM.AddState(GameState.Countdown);
        FSM.AddState(GameState.Battle);
        FSM.AddState(GameState.GameOver);

        FSM.SetStartState(GameState.Initializing);
    }

    void AddTransitions()
    {
        FSM.AddTransition(new Transition<GameState>(GameState.Battle, GameState.GameOver, IsTimeUp));
        FSM.AddTransition(new Transition<GameState>(GameState.Battle, GameState.GameOver, AllEnemyUnitsDead));
        FSM.AddTransition(new Transition<GameState>(GameState.Battle, GameState.GameOver,
                (Transition<GameState> _) => { return AllPlayerUnitsDead() && PlayerOutOfUnits(); }));
    }

    bool AllEnemyUnitsDead(Transition<GameState> _) =>
        UnitManager.Instance.Units.Where(u => u.Team == Team.Enemy)
                                    .Count() == 0;

    bool AllPlayerUnitsDead() =>
        UnitManager.Instance.Units.Where(u => u.Team == Team.Player)
                                    .Count() == 0;

    bool PlayerOutOfUnits() =>
        !Player.Instance.HasAnyUnits;

    bool IsTimeUp(Transition<GameState> _) => _gameTimeSeconds <= 0;

    void Update()
    {
        FSM.OnLogic();

        if (State != FSM.ActiveStateName)
        {
            State = FSM.ActiveStateName;
            _activeState = FSM.ActiveStateName.ToString();
            Bus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(State));

            switch (State)
            {
                case GameState.SearchingForFloor:
                    ToastMessage.Instance.EnqueueMessage("Point your camera at the ground", -1f);
                    break;

                case GameState.BuildingTerrain:
                    ToastMessage.Instance.EnqueueMessage("Slowly move your camera around to build the terrain", 6f);
                    break;

                case GameState.Countdown:
                    StartingEnemyUnitCount = UnitManager.Instance.Units.Where(u => u.Team == Team.Enemy).Count();
                    break;

                case GameState.GameOver:
                    GameOver();
                    break;

                default:
                    break;
            }
        }
    }

    public void FoundFloor()
    {
        FSM.RequestStateChange(GameState.BuildingTerrain);
    }

    public void DoneScanningButton()
    {
        FSM.RequestStateChange(GameState.BuildingBase);
    }

    public void DoneBuildingBase()
    {
        FSM.RequestStateChange(GameState.Countdown);

        _timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    public void TriggerGameOver()
    {
        FSM.RequestStateChange(GameState.GameOver);
    }

    public void BackToMap()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }

        SceneLoader.Instance.LoadSceneGroup(SceneGroupIds.MAP);
    }

    public void RegisterGoldStore(GoldStore store)
    {
        _goldStores.Add(store);

        var totalAvailable = _goldStores.Sum(s => s.StartingGold);
        GoldDisplay.Instance.Setup(totalAvailable, 0, true);
        ReportGoldStolen();
    }

    public void ReportGoldStolen()
    {
        TotalGoldStolen = _goldStores.Sum(s => s.StartingGold - s.CurrentGold);

        GoldDisplay.Instance.SetGoldValue(TotalGoldStolen);
        Blackboard.Instance.GoldStolen = TotalGoldStolen;
    }

    void GameOver()
    {
        CalculateGameOverStats();

        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
    }

    void CalculateGameOverStats()
    {
        var finalEnemyCount = UnitManager.Instance.Units.Where(u => u.Team == Team.Enemy).Count();
        var percent = Mathf.CeilToInt((1f - (finalEnemyCount / (float)StartingEnemyUnitCount)) * 100);
        CompletionPercent = Mathf.Clamp(percent, 0, 100);

        // Need 50 percent to win
        DidWin = CompletionPercent >= 50;

        if (DidWin)
        {
            Player.Instance.CaptureWayspot(Blackboard.Instance.WayspotIdentifier);
        }

        Blackboard.Instance.WayspotIdentifier = "";

        OnGameOverStatsReady?.Invoke();
    }

    IEnumerator TimerCoroutine()
    {
        SetTimerText(_gameTimeSeconds);

        // Countdown
        ToastMessage.Instance.EnqueueMessage("3", 0.5f, true);
        ToastMessage.Instance.EnqueueMessage("2", 0.5f);
        ToastMessage.Instance.EnqueueMessage("1", 0.5f);
        ToastMessage.Instance.EnqueueMessage("Fight!", 2f);

        yield return new WaitForSeconds(3f);

        FSM.RequestStateChange(GameState.Battle);

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

        while (_gameTimeSeconds >= 0)
        {
            _gameTimeSeconds--;
            SetTimerText(_gameTimeSeconds);

            yield return new WaitForSeconds(1f);
        }
    }

    void SetTimerText(float timeLeft)
    {
        TimeSpan time = TimeSpan.FromSeconds(timeLeft);
        _timerText.text = time.ToString(@"mm\:ss");
    }
}