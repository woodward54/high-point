using System;
using UnityEngine;

public class AttackState<TStateType> : UnitBaseState<TStateType>
{
    protected float _attackRange => OwnUnit.AttackRange;
    protected float _damagePerSecond => OwnUnit.DamagePerSecond;
    protected float _secondsPerAttack => OwnUnit.SecondsPerAttack;
    protected float _damageDelay => OwnUnit.DamageDelay;

    protected float _lastAttackTime;

    protected Transform _slewTransform;
    protected bool _slew;
    protected float _slewSpeed;
    protected bool _needsLineOfSight;

    public AttackState(Unit unit, Transform slewTransform, bool slew = true, float slewSpeed = 1f, bool needsLineOfSight = false) : base(unit)
    {
        _slew = slew;
        _slewSpeed = slewSpeed;
        _needsLineOfSight = needsLineOfSight;

        _slewTransform = slewTransform;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        OwnUnit.Animator.CrossFade(AnimatorStates.IDLE, 0.2f);

        _lastAttackTime = Time.time - _secondsPerAttack;
    }

    public override void OnLogic()
    {
        base.OnLogic();

        SlewToTarget();

        if (!IsTargetInView()) return;

        if (Time.time > _lastAttackTime + _secondsPerAttack)
        {
            _lastAttackTime = Time.time;

            ExecuteAttack();
        }
    }

    protected virtual void ExecuteAttack()
    {
        OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.ATTACK, 0.2f);

        var dmg = _damagePerSecond * _secondsPerAttack;

        if (OwnUnit.Target.TryGetComponent(out Unit unit))
        {
            unit.TakeDamage(dmg, OwnUnit, _damageDelay);
        }
        else
        {
            // Noop - we should never be attacking a Transform without a Unit
        }
    }

    protected void SlewToTarget()
    {
        if (!_slew) return;

        var targetPos = OwnUnit.Target.position;
        targetPos.y = _slewTransform.position.y;

        var targetDir = targetPos - _slewTransform.position;
        var targetRotation = Quaternion.LookRotation(targetDir);

        _slewTransform.rotation = Quaternion.Slerp(_slewTransform.rotation, targetRotation, _slewSpeed * Time.deltaTime);
    }

    bool IsTargetInView()
    {
        if (!_needsLineOfSight) return true;
        if (!_slew) return true;

        var targetPos = OwnUnit.Target.position;
        targetPos.y = _slewTransform.position.y;

        var targetDir = targetPos - _slewTransform.position;
        var angleDif = Vector3.Angle(targetDir, _slewTransform.forward);

        return angleDif <= 10f;
    }
}