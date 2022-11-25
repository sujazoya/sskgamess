using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimationTags
{

    public const string ZOOM_IN_ANIM = "ZoomIn";
    public const string ZOOM_OUT_ANIM = "ZoomOut";

    public const string SHOOT_TRIGGER = "Shoot";
    public const string AIM_PARAMETER = "Aim";

    public const string IDLE_PARAMETER = "IDLE";
    public const string WALK_PARAMETER = "Walk";
    public const string RUN_PARAMETER = "Run";
    public const string ATTACK_TRIGGER = "Attack";
    public const string DEAD_TRIGGER = "Dead";

}
public class EnemyAnimator : MonoBehaviour {

    private Animator anim;
    public EnemyController controller;

    void Start()
    { }
	void Awake () {
        anim = GetComponent<Animator>();
        controller = GetComponent<EnemyController>();
        if (controller)
        {
            controller.enabled = true;
        }
	}
	
    public void Walk(bool walk) {
        anim.SetBool(AnimationTags.WALK_PARAMETER, walk);
    }
    public void Idle(bool idle)
    {
        anim.SetBool(AnimationTags.WALK_PARAMETER, idle);
    }

    public void Run(bool run) {
        anim.SetBool(AnimationTags.RUN_PARAMETER, run);
    }

    public void Attack() {
        anim.SetTrigger(AnimationTags.ATTACK_TRIGGER);
    }

    public void Dead() {
        anim.SetTrigger(AnimationTags.DEAD_TRIGGER);
    }

} // class































