using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityHFSM;

public enum GameState
{
    Initializing,
    SearchingForFloor,
    BuildingTerrain,
    Battle,
    GameOver,
}

public class GameManager : Singleton<GameManager>
{
    public GameState State { get; private set; }

    [SerializeField] protected string _activeState;

    protected StateMachine<GameState> FSM;

    protected override void Awake()
    {
        FSM = new();
        AddStates();
        AddTransitions();
        FSM.Init();
    }

    protected void Start()
    {
        FSM.RequestStateChange(GameState.SearchingForFloor);
    }

    protected void AddStates()
    {
        FSM.AddState(GameState.Initializing);
        FSM.AddState(GameState.SearchingForFloor);
        FSM.AddState(GameState.BuildingTerrain);
        FSM.AddState(GameState.Battle);
        FSM.AddState(GameState.GameOver);

        FSM.SetStartState(GameState.Initializing);
    }

    protected void AddTransitions()
    {

    }

    void Update()
    {
        if (State != FSM.ActiveStateName)
        {
            State = FSM.ActiveStateName;
            _activeState = FSM.ActiveStateName.ToString();
            Bus<GameStateChangedEvent>.Raise(new GameStateChangedEvent(State));
        }
    }

    public void FoundFloor()
    {
        FSM.RequestStateChange(GameState.BuildingTerrain);
    }

    public void DoneScanningButton()
    {
        FSM.RequestStateChange(GameState.Battle);

        HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
    }

    // TODO: figure out game over state
    public void GameOver()
    {
        FSM.RequestStateChange(GameState.GameOver);
    }
}