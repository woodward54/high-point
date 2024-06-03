using UnityEngine;

public static class AnimatorStates
{
    public static readonly int IDLE = Animator.StringToHash("Idle");
    public static readonly int MOVE = Animator.StringToHash("Move");
    public static readonly int CLIMB = Animator.StringToHash("Climb");
    public static readonly int ATTACK = Animator.StringToHash("Attack");
    public static readonly int DEAD = Animator.StringToHash("Dead");
    public static readonly int WORKER_BUILD_STANDING = Animator.StringToHash("Build Standing");
    public static readonly int WORKER_BUILD_KNEELING = Animator.StringToHash("Build Kneeling");
}