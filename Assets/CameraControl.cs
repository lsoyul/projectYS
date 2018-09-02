using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public Transform targetObject;

    // Use this for initialization
    void Start()
    {
        if (targetObject != null)
        {
            this.transform.transform.position.Set(
            targetObject.transform.position.x,
            this.transform.position.y,
            targetObject.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
        {
            this.transform.position = targetObject.transform.position;
        }
    }
}
