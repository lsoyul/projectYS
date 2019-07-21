using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    //public float moveSpeed = 8f;
    public Joystick joystick;

    private float accelSpeedUnit = 0.2f;
    private float frictionForce = 0.03f;
    private float maxSpeed = 2.4f;

    private float attackCooldownDuration = 0.1f; // seconds

    private bool isAttackCooldown;
    public bool IsAttackCooldown
    {
        get { return isAttackCooldown; }
        set { isAttackCooldown = value; }
    }
    

    private bool isAttackableSpeed;
    public bool IsAttackableSpeed
    {
        get { return isAttackableSpeed; }
        set { isAttackableSpeed = value; }
    }

    public BoxCollider attackCollider;

    private Vector3 currentSpeedVector = new Vector3(GlobalVariables.zero, GlobalVariables.zero, GlobalVariables.zero);


    void Update()
    {
        isAttackableSpeed = (currentSpeedVector.magnitude > maxSpeed * 0.7f) ? true : false;
        

        if (isAttackableSpeed == true)
        {
            attackCollider.enabled = true;
        }
        else
        {
            attackCollider.enabled = false;
        }

    }

    private void FixedUpdate() {
        Vector3 accelVector = (Vector3.right * joystick.Horizontal + Vector3.forward * joystick.Vertical);

        if (accelVector != Vector3.zero)
        {
            // 가속한 방향쪽으로 향하기
            transform.rotation = Quaternion.LookRotation(accelVector);

            currentSpeedVector += (accelVector * accelSpeedUnit);

            if (currentSpeedVector.magnitude > maxSpeed)
            {
                currentSpeedVector = currentSpeedVector.normalized * maxSpeed;
            }
        }


        // Friction 적용
        if (currentSpeedVector.magnitude >= frictionForce)
        {
            currentSpeedVector += (-currentSpeedVector.normalized * frictionForce);

            
        }

        if (currentSpeedVector.magnitude < frictionForce)
        {
            currentSpeedVector.Set(GlobalVariables.zero, GlobalVariables.zero, GlobalVariables.zero);
        }

        transform.Translate(currentSpeedVector, Space.World);
    }

    public float GetCurrentSpeedFloat()
    {
        return currentSpeedVector.magnitude;
    }

    public Vector3 GetCurrentSpeedVector()
    {
        return currentSpeedVector;
    }

    IEnumerator AttackCooling()
    {
        isAttackCooldown = true;
        yield return new WaitForSecondsRealtime(attackCooldownDuration);

        isAttackCooldown = false;
    }


    #region ##### Collision #####

    public void OnCollideToPracticeCube(Collision cube)
    {
        if (isAttackCooldown == false && isAttackableSpeed == true)
        {
            // Attack to cube
            StartCoroutine(AttackCooling());

            Vector3 newCurrentVector 
            = new Vector3(this.transform.position.x - cube.transform.position.x,
                            0, this.transform.position.z - cube.transform.position.z);

            newCurrentVector.Normalize();

            currentSpeedVector = newCurrentVector * maxSpeed * 1.2f;
        }
    }

    #endregion
}
