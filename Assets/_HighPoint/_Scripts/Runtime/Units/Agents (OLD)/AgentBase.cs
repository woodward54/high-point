// using System.Collections.Generic;
// using Pathfinding;
// using UnityEngine;

// // Agent is a moveable unit

// [RequireComponent(typeof(FollowerEntity), typeof(AIDestinationSetter))]
// public abstract class AgentBase : UnitBase<AgentStates>
// {
//     protected static readonly int SPEED_PARAMETER = Animator.StringToHash("Speed");
//     protected float SmoothSpeed = 0;

//     public FollowerEntity Pathfinder { get; protected set; }
//     public AIDestinationSetter DestSetter { get; protected set; }

//     protected override void Awake()
//     {
//         base.Awake();

//         Pathfinder = GetComponent<FollowerEntity>();
//         DestSetter = GetComponent<AIDestinationSetter>();
//     }

//     protected override void OnEnable()
//     {
//         base.OnEnable();
//     }

//     protected override void OnDisable()
//     {
//         base.OnDisable();
//     }

//     protected override void Update()
//     {
//         base.Update();

//         // TODO: does this work?
//         SmoothSpeed = Mathf.Lerp(SmoothSpeed, (Pathfinder.enabled && Pathfinder.hasPath) ? Pathfinder.velocity.magnitude : 0,
//             Time.deltaTime);
//         Animator.SetFloat(SPEED_PARAMETER, SmoothSpeed);
//     }

//     public void GetAttacked()
//     {
        
//     }

//     public override void TakeDamage(float damage, AbstractUnit attacker)
//     {
//         base.TakeDamage(damage, attacker);

//         if (CurrentHealth > 0)
//         {
//             FSM.Trigger(StateEvent.MoveIssued);
//         }
//     }
// }