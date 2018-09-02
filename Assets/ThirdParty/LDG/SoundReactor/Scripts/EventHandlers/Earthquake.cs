using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LDG.SoundReactor;

public class Earthquake : MonoBehaviour
{
    public float radius = 1.0f;
    public Vector2 speed;

    private Vector2 sinCos;
    private Vector3 originalPos;

    // Use this for initialization
    void Start()
    {
        sinCos = Vector2.zero;

        originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = Vector2.zero;

        sinCos.x += speed.x * Time.deltaTime;
        sinCos.y += speed.y * Time.deltaTime;

        offset.x = Mathf.Sin(sinCos.x) * radius;
        offset.y = Mathf.Sin(sinCos.y) * radius;

        transform.localPosition = new Vector3(offset.x + originalPos.x, offset.y + originalPos.y, originalPos.z);
    }

    public void OnLevel(PropertyDriver driver)
    {
        Vector3 level = driver.LevelVector();
        speed.x = level.x;
        speed.y = level.y;
        radius = level.z;
    }
}
