// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

// The following copywrite notice is for the parts of this file that resemble the original work
// created by Keijiro in the project located here:
// https://github.com/keijiro/unity-audio-spectrum
//
// Copyright (C) 2013 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace LDG.SoundReactor
{
    // https://docs.unity3d.com/ScriptReference/Editor.html
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpectrumSource))]
    public class SpectrumSourceEditor : Editor
    {
        static int[] bandOptions =
        {
            (int)BandOption.FullRange,
            (int)BandOption.StandardRanges,
            (int)BandOption.OneOctave,
            (int)BandOption.OneHalfOctave,
            (int)BandOption.OneThirdOctave,
            (int)BandOption.OneSixthOctave,
            (int)BandOption.OneTwelvethOctave,
            (int)BandOption.A440,
        };

        static GUIContent[] bandOptionStrings = new GUIContent[]
        {
            new GUIContent("1 band (Full Range)"),
            new GUIContent("7 Bands (Standard Ranges)"),
            new GUIContent("10 Bands (One Octave)"),
            new GUIContent("20 Bands (One Half Octave)"),
            new GUIContent("30 Bands (One 3rd Octave)"),
            new GUIContent("60 Bands (One 6th Octave)"),
            new GUIContent("120 Bands (One 12th Octave)"),
            new GUIContent("120 Bands (A440)")
        };

        static int[] audioChannels =
        {
            (int)AudioChannel.FrontLeft,
            (int)AudioChannel.FrontRight,
            (int)AudioChannel.FrontCenter,
            (int)AudioChannel.Subwoofer,
            (int)AudioChannel.RearLeft,
            (int)AudioChannel.RearRight,
            (int)AudioChannel.AlternativeRearLeft,
            (int)AudioChannel.AlternativeRearRight
        };

        static GUIContent[] audioChannelStrings = new GUIContent[]
        {
            new GUIContent("Front Left (mono)"),
            new GUIContent("Front Right"),
            new GUIContent("Center"),
            new GUIContent("Subwoofer"),
            new GUIContent("Rear Left"),
            new GUIContent("Rear Right"),
            new GUIContent("Alternative Rear Left"),
            new GUIContent("Alternative Rear Right")
        };

        SerializedProperty audioSourceProp;
        SerializedProperty peaksProfileProp;

        SerializedProperty channelProp;
        SerializedProperty bandOptionProperty;
        SerializedProperty normalizeProp;

        void OnEnable()
        {
            // Setup the SerializedProperties.
            audioSourceProp = serializedObject.FindProperty("audioSource");
            peaksProfileProp = serializedObject.FindProperty("peaksProfile");

            channelProp = serializedObject.FindProperty("audioChannel");
            bandOptionProperty = serializedObject.FindProperty("bandOption");
            normalizeProp = serializedObject.FindProperty("normalizeMode");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            SpectrumSource source = (SpectrumSource)target;

            EditorGUILayout.Space();

            EditorGUILayout.Foldout(true, "Audio", GlobalStyles.heading);

            EditorGUILayout.PropertyField(audioSourceProp, new GUIContent("AudioSource"));
            EditorGUILayout.IntPopup(channelProp, audioChannelStrings, audioChannels, new GUIContent("Channel", "The audio channel to pull spectrum data from. If the channel doesn't exist, then it'll fall back to the highest supported channel during play mode."));
            EditorGUILayout.PropertyField(peaksProfileProp, new GUIContent("Peaks Profile", "Audio Spectrum Profile. Create one from the Assets menu."));

            if (!source.peaksProfile)
            {
                EditorGUILayout.HelpBox("A Peaks Profile must be attached for the spectrum to work.", MessageType.Warning);
            }

            if (Application.isPlaying && source.peaksProfile != null && source.peaksProfile.isDirty)
            {
                if (source.recordProfile)
                {
                    if (GUILayout.Button("Stop Recording"))
                    {
                        source.recordProfile = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("Record Peaks"))
                    {
                        source.RecordPeaks();
                    }
                }
            }

            GUI.enabled = !source.recordProfile;

            EditorGUILayout.Space();
            
            EditorGUILayout.Foldout(true, "Presentation", GlobalStyles.heading);
            EditorGUILayout.IntPopup(bandOptionProperty, bandOptionStrings, bandOptions, new GUIContent("Bands", "The number of times to divide the standard audio frequency."));
            EditorGUILayout.PropertyField(normalizeProp, new GUIContent("Normalize", "Scales level to be between 0 and 1 either by the highest peak value, or individually per level peak. Raw just passes the original spectrum data through."));
            
            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();

            if (source.audioSource && source.audioSource.clip)
            {
                source.audioChannel = Mathf.Clamp(source.audioChannel, 0, (source.audioSource.clip.channels - 1));
            }

            if (GUI.changed)
            {
                foreach (SpectrumSource src in targets)
                {
                    src.Refresh();
                }
            }
        }
    }
}