using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCycler : MonoBehaviour
{
    public Scene[] scenes;

    public int sceneIndex = 1;
    private int currentSceneIndex = 1;

	// Use this for initialization
	void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        bool keyPressed = false;

        currentSceneIndex = sceneIndex;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            sceneIndex--;
            keyPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            sceneIndex++;
            keyPressed = true;
        }

        if (keyPressed)
        {
            Debug.Log(sceneIndex);
            sceneIndex = Mathf.Clamp(sceneIndex, 1, SceneManager.sceneCountInBuildSettings - 1);

            if (SceneManager.sceneCount > 1)
            {
                Scene scene = SceneManager.GetSceneAt(1);

                if (scene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(currentSceneIndex);
                }
            }

            SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        }
    }
}
