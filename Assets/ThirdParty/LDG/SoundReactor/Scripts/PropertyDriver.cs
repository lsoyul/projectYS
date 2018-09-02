// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LDG.SoundReactor
{
    public class PropertyDriver : SerializeableObject
    {
        public PropertyDriver sharedDriver;
        public Level level;

        public Vector3 travel = Vector3.up;
        public float clipping = 1.0f;
        public float strength = 1.0f;
        public bool onBeat = false;

        // this is only a property to keep it from getting serialized
        public bool componentMissing { get; set; }

        private Vector3 scaledLevel;
        private bool updateLevelVector = true;
        private bool updateLevelScalar = true;

        private void LateUpdate()
        {
            updateLevelVector = true;
            updateLevelScalar = true;
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

            if (InheritDependencies() && !componentMissing)
            {
                DoLevel();
            }
        }

        public Vector3 LevelVector()
        {
            if (updateLevelVector)
            {
                updateLevelVector = false;

                PropertyDriver driver = (sharedDriver != null) ? sharedDriver : this;

                float s = 0.0f;

                if (driver.onBeat)
                {
                    float beatScale = (level.beatScalar == 1.0f) ? 1.0f : 0.0f;

                    s = Mathf.Clamp(level.fallingLevel * driver.strength * beatScale, 0.0f, driver.clipping);
                }
                else
                {
                    s = Mathf.Clamp(level.fallingLevel * driver.strength, 0.0f, driver.clipping);
                }

                scaledLevel = driver.travel * s;
            }

            return scaledLevel;
        }

        public float LevelScalar()
        {
            if (updateLevelScalar)
            {
                updateLevelScalar = false;

                PropertyDriver driver = (sharedDriver != null) ? sharedDriver : this;

                float s;

                if (driver.onBeat)
                {
                    float beatScale = (level.beatScalar == 1.0f) ? 1.0f : 0.0f;

                    s = Mathf.Clamp(level.fallingLevel * driver.strength * beatScale, 0.0f, driver.clipping);
                }
                else
                {
                    s = Mathf.Clamp(level.fallingLevel * driver.strength, 0.0f, driver.clipping);
                }

                scaledLevel.y = driver.travel.magnitude * s;
            }

            return scaledLevel.y;
        }

        protected virtual bool InheritDependencies()
        {
            if (level) return true;

            Transform parent = transform.parent;
            Level lvl = null;

            if (level == null)
            {
                level = GetComponent<Level>();
            }

            while (level == null)
            {
                if (parent != null)
                {
                    if ((lvl = parent.GetComponent<Level>()))
                    {
                        if (lvl.inheritable)
                        {
                            level = lvl;
                        }
                    }

                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            return (level);
        }

        protected virtual void DoLevel()
        {
            // nothing to do here
        }
    }
}
