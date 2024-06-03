using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drawing;
using UnityEngine;
using UnityHFSM;

public enum UnitState
{
    Idle,
    Search,
    Move,
    Attack,
    Dead
}

public enum Team
{
    Player,
    Enemy
}

// TODO: might be inefficient for every unit to have an animator. i.e. do buildings need animators?
[RequireComponent(typeof(Animator))]
public abstract class Unit : MonoBehaviourGizmos, IDamageable
{
    public Guid InstanceID { get; private set; } = new Guid();

    public UnitConfig UnitConfig;

    public Transform Transform => transform;
    public bool IsDead => CurrentHealth <= 0 || !gameObject.activeInHierarchy;

    [field: SerializeField] public float CurrentHealth { get; protected set; }
    [field: SerializeField] public float MaxHealth { get; protected set; }
    [field: SerializeField] public Team Team { get; private set; }

    [field: Header("Current Target")]
    [field: SerializeField] public Transform Target { get; private set; }

    [field: Header("Attack Properties")]
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float SecondsPerAttack { get; private set; }
    [field: SerializeField] public float DamagePerSecond { get; private set; }
    [field: SerializeField] public float DamageDelay { get; private set; }

    // For debugging
    [SerializeField] protected string _activeState;
    [SerializeField] protected bool _isDead;

    public Animator Animator { get; private set; }
    protected StateMachine<UnitState> FSM;

    HealthBar _healthBar;

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();

        FSM = new();
        AddStates();
        AddTransitions();
        FSM.Init();
    }

    protected virtual void OnEnable()
    {
        SetupFromSO();
        Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
    }

    protected virtual void OnDisable()
    {
        Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
    }

    protected virtual void Start() { }

    protected virtual void Update()
    {
        FSM.OnLogic();

#if UNITY_EDITOR
        // Editor Debug info
        _activeState = FSM.ActiveStateName.ToString();
        _isDead = IsDead;
#endif
    }

    public virtual void TakeDamage(float damage, Unit attackingUnit, float delay = 0f)
    {
        if (IsDead) return;

        if (delay > 0)
        {
            StartCoroutine(TakeDamageAfter(damage, attackingUnit, delay));
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);

        _healthBar.SetHealthPercent(CurrentHealth / (float)MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    protected virtual void Die()
    {
        StartCoroutine(DieAfter(0f));
    }

    IEnumerator TakeDamageAfter(float damage, Unit attackingUnit, float delay)
    {
        yield return new WaitForSeconds(delay);

        TakeDamage(damage, attackingUnit);
    }

    protected virtual IEnumerator DieAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }

    protected virtual void SetupFromSO()
    {
        MaxHealth = UnitConfig.Health;
        CurrentHealth = UnitConfig.Health;

        AttackRange = UnitConfig.AttackRange;
        SecondsPerAttack = UnitConfig.SecondsPerAttack;
        DamagePerSecond = UnitConfig.DamagePerSecond;
        DamageDelay = UnitConfig.DamageDelay;

        AttackRange *= HexGrid.Instance.HexSize;
    }

    protected abstract void AddStates();

    protected abstract void AddTransitions();

    public void RegisterHealthBar(HealthBar bar)
    {
        _healthBar = bar;
    }

    // Debug Stuff
    public override void DrawGizmos()
    {
        if (GizmoContext.InSelection(this))
        {
            Draw.WireSphere(transform.position, AttackRange, Color.green);
        }
    }

    public override string ToString()
    {
        if (UnitConfig != null)
        {
            return "Unit: " + UnitConfig.name + " (" + InstanceID.ToString() + ")";
        }

        return base.ToString();
    }
}

// public void RegisterBehavior(IUnitBehavior behavior, int priority)
// {
//     if (_behaviors.Values.Any(b => b.GetType() == behavior.GetType()))
//     {
//         Debug.LogWarning(behavior.GetType() + " is already registered for this unit. Cannot register duplicate behavior types");
//         return;
//     }

//     if (_behaviors.ContainsKey(priority))
//     {
//         Debug.LogWarning("There is already a behavior listed with priority: " + priority + " for behavior" + behavior.GetType());
//         return;
//     }

//     behavior.Setup(this);

//     _behaviors.Add(priority, behavior);
// }

// public T GetBehavior<T>()
// {
//     return (T)_behaviors.Values.Where(b => b.GetType() == typeof(T));
// }

// public void RegisterSystem(IUnitSystem system)
// {
//     if (_systems.Any(s => s.GetType() == system.GetType()))
//     {
//         Debug.LogWarning(system.GetType() + " is already registered for this unit. Cannot register duplicate system types");
//         return;
//     }

//     system.Setup(this);

//     // (All systems are enabled by default)

//     _systems.Add(system);
// }

// public T GetSystem<T>()
// {
//     return (T)_systems.Find(s => s.GetType() == typeof(T));
// }

