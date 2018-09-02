// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Level))]
    public class LevelEditor : Editor
    {
        SerializedProperty spectrumFilterProp;
        SerializedProperty linearizedFrequencyProp;
        SerializedProperty inheritableProp;

        private void OnEnable()
        {
            spectrumFilterProp = serializedObject.FindProperty("spectrumFilter");
            linearizedFrequencyProp = serializedObject.FindProperty("linearizedFrequency");
            inheritableProp = serializedObject.FindProperty("inheritable");
        }

        public override void OnInspectorGUI()
        {
            Level level = (Level)target;

            float linearizedFrequency = level.linearizedFrequency;

            linearizedFrequency = SpectrumGUILayout.SpectrumField(level.spectrumFilter, level.linearizedFrequency, level.levelBeat);

            foreach (Level lvl in targets)
            {
                lvl.linearizedFrequency = Frequency.UnlinearizeFrequency(linearizedFrequency, Frequency.LowestAudibleFrequency, Frequency.HighestAudibleFrequency);
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(spectrumFilterProp, new GUIContent("SpectrumFilter", "SpectrumFilter to grab spectrum data from. If this is set to None, then it'll try to find a SpectrumFilter at runtime by looking up through the hierarchy."));
            EditorGUILayout.PropertyField(linearizedFrequencyProp, new GUIContent("Frequency (Hz)", "Frequency to track. Set the frequency directly here, or click in the frequency window above."));
            EditorGUILayout.PropertyField(inheritableProp, new GUIContent("Inheritable", "Tells a PropertyDriver if it can inherit this Level or not, unless the PropertyDriver is sharing the same GameObject as the Level, in which the Level will automatically be inherited."));

            serializedObject.ApplyModifiedProperties();
            
            foreach (Level lvl in targets)
            {
                lvl.linearizedFrequency = Mathf.Clamp(lvl.linearizedFrequency, Frequency.LowestAudibleFrequency, Frequency.HighestAudibleFrequency);
                lvl.linearizedFrequency = Frequency.LinearizeFrequency(lvl.linearizedFrequency);

                Undo.RecordObject(lvl, "Level");
            }
            
            if (Application.isPlaying || GUI.changed)
            {
                Repaint();
            }
        }
    }
}