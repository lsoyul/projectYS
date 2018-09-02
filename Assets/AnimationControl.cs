using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour {

    public PlayerControl playerControl;
    public Animator targetAnimator;

    private void Update()
    {
        targetAnimator.SetFloat("moveSpeed", playerControl.GetCurrentSpeedFloat());
    }
}
