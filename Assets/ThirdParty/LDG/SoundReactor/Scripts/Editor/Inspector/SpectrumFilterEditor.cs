// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpectrumFilter))]
    public class SpectrumFilterEditor : Editor
    {
        SerializedProperty spectrumSource;
        SerializedProperty eq;

        SerializedProperty interpolationType;
        SerializedProperty scale;
        SerializedProperty fallSpeed;
        SerializedProperty beatSensitivity;

        private void OnEnable()
        {
            spectrumSource = serializedObject.FindProperty("spectrumSource");
            eq = serializedObject.FindProperty("eq");

            interpolationType = serializedObject.FindProperty("interpolationMode");
            scale = serializedObject.FindProperty("scale");
            fallSpeed = serializedObject.FindProperty("fallSpeed");
            beatSensitivity = serializedObject.FindProperty("beatSensitivity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpectrumFilter spectrumFilter = (SpectrumFilter)target;

            SpectrumGUILayout.SpectrumField(spectrumFilter, 2.0f, -1.0f);

            EditorGUILayout.PropertyField(spectrumSource, new GUIContent("SpectrumSource", "The SpectrumSource to filter. If this is set to None, then it'll try to find a SpectrumSource at runtime by looking up through the hierarchy."));
            EditorGUILayout.PropertyField(eq, new GUIContent("EQ", "Allows the user to make finer adjustments to the spectrum."));
            EditorGUILayout.PropertyField(interpolationType, new GUIContent("Interpolation", "Sets whether the values in between levels ease in and out or are straight lines."));
            EditorGUILayout.PropertyField(scale, new GUIContent("Scale", "Scales all magnitudes equally for the given SpectrumSource"));
            EditorGUILayout.PropertyField(fallSpeed, new GUIContent("Fall Speed", "The time in seconds a level at maximum height will reach its minimum height."));
            EditorGUILayout.PropertyField(beatSensitivity, new GUIContent("Beat Sensitivity", "The percentage threshold that a level has to move to trigger a beat. 1 equals 100%"));

            // Update frequently while it's playing.
            if (EditorApplication.isPlaying || GUI.changed)
            {
                Repaint();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}