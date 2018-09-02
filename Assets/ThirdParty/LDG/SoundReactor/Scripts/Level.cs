// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LDG.SoundReactor
{
    [DisallowMultipleComponent]
    public class Level : SerializeableObject
    {
        public SpectrumFilter spectrumFilter;
        public float linearizedFrequency = 0.0f;
        public bool inheritable = true;

        private bool falling = false;
        private float levelPrev;
        private float levelValley = 0.0f;
        private float levelDir;

        [SerializeField]
        private float _normalizedLevel = 0.0f;
        public float normalizedLevel
        {
            get { return _normalizedLevel; }
            set { _normalizedLevel = value; }
        }

        private float _levelBeat = 0.0f;
        public float levelBeat
        {
            get
            {
                return _levelBeat;
            }
        }

        private float _fallingLevel = 0.0f;
        public float fallingLevel
        {
            get
            {
                return _fallingLevel * _levelScalar;
            }
        }

        private float _levelScalar = 1.0f;
        public float levelScalar
        {
            get
            {
                return _levelScalar;
            }
        }

        private float _beatScalar = 1.0f;
        public float beatScalar
        {
            get
            {
                return _beatScalar;
            }
        }

        public void Set(float linearizedFrequency, float normalizedLevel, float lowerFrequency, float upperFrequency, float repeat, bool alternate, bool reverse, bool flipLevel)
        {
            if (flipLevel)
            {
                normalizedLevel = 1.0f - normalizedLevel;
            }

            linearizedFrequency = Frequency.TransformLinearFrequency(linearizedFrequency, repeat, alternate, reverse);

            this.linearizedFrequency = Frequency.RemapLinear(linearizedFrequency, lowerFrequency, upperFrequency);
            this.normalizedLevel = normalizedLevel;
        }

        public float GetLevel ()
        {
            float level = 0.0f;

            if(spectrumFilter)
            {
                level = spectrumFilter.GetLevel(linearizedFrequency);
            }

            return level;
        }

        void LateUpdate()
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
                float level;

                _beatScalar = 0.0f;

                level = spectrumFilter.GetLevel(linearizedFrequency);

                levelDir = level - levelPrev;
                levelPrev = level;

                _fallingLevel -= Time.deltaTime * spectrumFilter.fallSpeed;
                _fallingLevel = Mathf.Max(level, _fallingLevel);

                if (levelDir > 0.0f && falling)
                {
                    levelValley = level;

                    falling = false;
                }

                if (levelDir < 0.0f && !falling)
                {
                    _levelBeat = level;

                    falling = true;

                    if (Mathf.Abs(_levelBeat - levelValley) > spectrumFilter.beatSensitivity)
                    {
                        _beatScalar = 1.0f;
                    }
                }

                _levelScalar = (_fallingLevel >= normalizedLevel) ? 1.0f : 0.0f;
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.19f, 0.65f, 0.86f, 0.9f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
#endif
        bool InheritDependencies()
        {
            if (spectrumFilter) return true;

            Transform parent = transform.parent;

            if(spectrumFilter == null)
            {
                spectrumFilter = GetComponent<SpectrumFilter>();
            }
            
            while (spectrumFilter == null)
            {
                if(parent != null)
                {
                    spectrumFilter = parent.GetComponent<SpectrumFilter>();

                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            return (spectrumFilter);
        }
    }
}
