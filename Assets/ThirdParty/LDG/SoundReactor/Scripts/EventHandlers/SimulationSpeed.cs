using UnityEngine;
using LDG.SoundReactor;

public class SimulationSpeed : MonoBehaviour
{
    ParticleSystem ps;
   
    // Use this for initialization
    void Start ()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void OnLevel(PropertyDriver driver)
    {
        float level = driver.LevelScalar();

        if (ps)
        {
#if UNITY_4_6
			ps.playbackSpeed = 1.0f + level;
#else
            ParticleSystem.MainModule module;
            module = ps.main;

			module.simulationSpeed = 1.0f + level;
#endif
        }
    }
}
