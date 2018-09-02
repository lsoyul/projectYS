// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

#if UNITY_EDITOR
#endif

namespace LDG.SoundReactor
{
    [DisallowMultipleComponent]
    public class SpectrumFilter : SerializeableObject
    {
        public SpectrumSource spectrumSource;
        public EQ eq;

        public InterpolationMode interpolationMode = InterpolationMode.Linear;
        public float scale = 1.0f;
        public float fallSpeed = 2.0f;
        public float beatSensitivity = 0.2f;

#if UNITY_EDITOR
        [System.NonSerialized]
        public float[] fallingLevels = null;

        public void UpdateFallingLevels()
        {
            if (fallingLevels == null || fallingLevels.Length != spectrumSource.bands)
            {
                fallingLevels = new float[spectrumSource.bands];

                for (int i = 0; i < spectrumSource.bands; i++)
                {
                    fallingLevels[i] = 0.001f;
                }
            }

            for(int i = 0; i < spectrumSource.bands; i++)
            {
                fallingLevels[i] -= Time.deltaTime * fallSpeed;
                fallingLevels[i] = Mathf.Max(fallingLevels[i], GetLevel((float)i / ((float)spectrumSource.bands - 1.0f)));
            }
        }
#endif

        public float GetLevel(float linearizedFrequency)
        {
            float level = 0.0f;
            float eqValue = 1.0f;
            float amplitudeScale = 1.0f;

            if(spectrumSource)
            {
                level = spectrumSource.GetLevel(linearizedFrequency, interpolationMode);
                amplitudeScale = spectrumSource.amplitudeScale;
            }

            if(eq)
            {
                eqValue = eq.GetLevel(linearizedFrequency, interpolationMode);
            }

            return level * eqValue * scale * amplitudeScale;
        }

        void Update()
        {
#if UNITY_EDITOR
            // we need to remember object reference states before we change them at run time. to do this we
            // need to give EditorApplication a chance to post the PlayModeStateChange.EnteredPlayMode event.
            // by default that event seems to post after the monobehaviour events are called.
            // since I can't change when that event is posted, I have to delay when the dependencies are aquired.
            if (Time.frameCount <= 2) return;
#endif

            if (InheritDependencies())
            {
#if UNITY_EDITOR
                UpdateFallingLevels();
#endif
            }
        }

        bool InheritDependencies()
        {
            //if (spectrumSource) return true;

            Transform parent = transform.parent;

            if(spectrumSource == null)
            {
                spectrumSource = GetComponent<SpectrumSource>();
            }

            if(eq == null)
            {
                eq = GetComponent<EQ>();
            }

            while (spectrumSource == null || eq == null)
            {
                if (parent != null)
                {
                    if (spectrumSource == null)
                    {
                        spectrumSource = parent.GetComponent<SpectrumSource>();
                    }

                    if (eq == null)
                    {
                        eq = parent.GetComponent<EQ>();
                    }

                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            return (spectrumSource);
        }
    }
}