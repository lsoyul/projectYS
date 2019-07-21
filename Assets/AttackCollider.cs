using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class AttackCollider : MonoBehaviour
{
    public PlayerControl playerControl;

    private void OnCollisionEnter(Collision other) {
        Debug.Log("OnCollisionEnter : " + other.gameObject.tag);
    }

    /// <summary>
    /// OnCollisionStay is called once per frame for every collider/rigidbody
    /// that is touching rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "PracticeCube")
        {
            playerControl.OnCollideToPracticeCube(other);
        }
        Debug.Log("OnCollisionStay : " + other.gameObject.tag);
    }

    private void OnCollisionExit(Collision other) {
        Debug.Log("OnCollisionExit : " + other.gameObject.tag);
    }
}
