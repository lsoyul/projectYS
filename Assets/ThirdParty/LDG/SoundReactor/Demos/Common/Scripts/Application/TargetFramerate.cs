using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetFramerate : MonoBehaviour
{
    public int targetFramerate = 60;
    
    // Use this for initialization
    void Start ()
    {
        Application.targetFrameRate = targetFramerate;
    }
}
