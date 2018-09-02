using UnityEngine;

public class Player3DExample : MonoBehaviour {

    //public float moveSpeed = 8f;
    public Joystick joystick;

    public float accelSpeedUnit = 0.4f;
    public float frictionForce = 0.2f;
    public float maxSpeed = 8f;
    
    private Vector3 currentSpeedVector = new Vector3(0f, 0f, 0f);

    void Update () 
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
        if (currentSpeedVector.magnitude > 0.1f)
        {
            currentSpeedVector += (-currentSpeedVector.normalized * frictionForce);

            if (currentSpeedVector.magnitude < 0.1f)
            {
                currentSpeedVector.Set(0f, 0f, 0f);
            }
        }
        transform.Translate(currentSpeedVector * Time.deltaTime, Space.World);


    }
}