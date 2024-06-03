public abstract class BaseCellState : ICellState
{
    public abstract CellState State { get; }

    public abstract void Enter(HexCell cell);

    public abstract void Exit(HexCell cell);

    public virtual ICellState OnDeselect()
    {
        return this;
    }

    public virtual ICellState OnSelect()
    {
        return this;
    }
}