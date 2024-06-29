using UnityEngine;

public class OutpostMarker : MapMarker, ISelectable
{
    [field: SerializeField] public SerializableGuid Identifier { get; set; } = SerializableGuid.NewGuid();
    [field: SerializeField] public BaseConfig BaseConfig { get; private set; }

    public void Select()
    {

    }

    public void Unselect()
    {

    }
}