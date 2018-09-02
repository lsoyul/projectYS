using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    //public float moveSpeed = 8f;
    public Joystick joystick;

    public float accelSpeedUnit = 1.0f;
    public float frictionForce = 0.4f;
    public float maxSpeed = 8f;

    private Vector3 currentSpeedVector = new Vector3(GlobalVariables.zero, GlobalVariables.zero, GlobalVariables.zero);

    void Update()
    {
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

        transform.Translate(currentSpeedVector * Time.deltaTime, Space.World);


    }

    public float GetCurrentSpeedFloat()
    {
        return currentSpeedVector.magnitude;
    }

    public Vector3 GetCurrentSpeedVector()
    {
        return currentSpeedVector;
    }
}
