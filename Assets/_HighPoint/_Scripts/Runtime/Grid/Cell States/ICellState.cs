public interface ICellState
{
    CellState State { get; }

    void Enter(HexCell cell);
    void Exit(HexCell cell);

    // Transition triggers
    ICellState OnSelect();
    ICellState OnDeselect();
}