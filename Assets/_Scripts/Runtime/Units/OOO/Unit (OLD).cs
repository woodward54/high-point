// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Pathfinding;
// using UnityEngine;
// using UnityHFSM;

// public enum UnitType
// {
//     BUILDING,
//     AGENT
// }

// public class Unit
// {
//     public Guid InstanceID { get; private set; }

//     public Transform UnitGO { get; private set; }
//     public Animation AnimationController { get; private set; }

//     public UnitConfigSO Data { get; private set; }
//     public Team Team { get; private set; }
//     public UnitType Type { get; private set; }

//     public int CurrentHealth { get; private set; }

//     public StateMachine Fsm { get; private set; }

//     public Transform Target { get; private set; }

//     UnitManager _manager;
//     List<IUnitSystem> _systems = new();

//     public Unit(UnitConfigSO data, Team team, UnitType type, UnitManager unitManager)
//     {
//         InstanceID = Guid.NewGuid();

//         Data = data;
//         Team = team;
//         Type = type;
//         _manager = unitManager;

//         CurrentHealth = data.Health;

//         Fsm = new StateMachine();

//         // TODO add all the states

//         Fsm.AddTransitionFromAny(new Transition(
//             "",    // From can be left empty, as it has no meaning in this context
//             "Dead",
//             t => (CurrentHealth <= 0)
//         ));

//         Fsm.Init();
//     }

//     public void Update()
//     {
//         Fsm.OnLogic();

//         foreach (var system in _systems)
//         {
//             system.Update();
//         }
//     }

//     public void Instantiate(Vector3 position, Quaternion rotation, float scale)
//     {
//         UnitGO = UnityEngine.Object.Instantiate(
//             Data.Prefab,
//             position,
//             rotation,
//             _manager.transform);

//         UnitGO.transform.localScale *= HexGrid.Instance.HexSize * scale;
//         AnimationController = UnitGO.GetComponent<Animation>();
//     }

//     public void RegisterSystem(IUnitSystem system)
//     {
//         if (_systems.Any(s => s.GetType() == system.GetType()))
//         {
//             Debug.LogWarning(system.GetType() + " is already registered for this unit. Cannot register duplicate system types");
//             return;
//         }

//         system.Setup(this);

//         // (All systems are enabled by default)

//         _systems.Add(system);
//     }

//     public T GetSystem<T>()
//     {
//         return (T)_systems.Find(s => s.GetType() == typeof(T));
//     }

//     public void DamageUnit(int amount)
//     {
//         CurrentHealth -= amount;

//         _manager.ReportUnitHealthChanged(this);

//         if (CurrentHealth <= 0)
//         {
//             _manager.ReportUnitDied(this);
//         }
//     }

//     public void Destroy()
//     {
//         UnityEngine.Object.Destroy(UnitGO.gameObject);
//     }

//     public override string ToString()
//     {
//         if (Data != null)
//         {
//             return "Unit: " + Data.name + " (" + InstanceID.ToString() + ")";
//         }

//         return base.ToString();
//     }
// }