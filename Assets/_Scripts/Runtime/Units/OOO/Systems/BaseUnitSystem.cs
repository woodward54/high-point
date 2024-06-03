using UnityEngine;

[RequireComponent(typeof(Unit))]
public abstract class BaseUnitSystem : MonoBehaviour
{
    public bool IsEnabled { get; private set; } = true;

    protected Unit _ownUnit;

    protected virtual void Awake()
    {
        _ownUnit = GetComponent<Unit>();
    }
}