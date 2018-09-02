using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    // https://docs.unity3d.com/ScriptReference/Editor.html
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PeaksProfile))]
    public class PeaksProfileEditor : Editor
    {
    
        static int[] sampleOptions =
        {
            64, 128, 256, 512, 1024, 2048, 4096, 8192
        };

        static GUIContent[] sampleOptionStrings = new GUIContent[]
        {
            new GUIContent("64"),
            new GUIContent("128"),
            new GUIContent("256"),
            new GUIContent("512"),
            new GUIContent("1024"),
            new GUIContent("2048"),
            new GUIContent("4096"),
            new GUIContent("8192")
        };

        static public readonly int[] audioChannels =
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

        static public readonly GUIContent[] audioChannelStrings = new GUIContent[]
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

        SerializedProperty windowProp;
        SerializedProperty samplesProp;
        SerializedProperty channelProp;
        SerializedProperty amplitudeProp;
        
        void OnEnable()
        {
            // Setup the SerializedProperties.
            windowProp = serializedObject.FindProperty("fftWindow");
            samplesProp = serializedObject.FindProperty("fftSamples");

            amplitudeProp = serializedObject.FindProperty("amplitudeMode");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            PeaksProfile source = (PeaksProfile)target;

            EditorGUILayout.Foldout(true, "FFT", GlobalStyles.heading);

            EditorGUILayout.PropertyField(windowProp, new GUIContent("Window", "The quality of the spectrum data. List is in desceneding order, lowest to highest."));
            EditorGUILayout.IntPopup(samplesProp, sampleOptionStrings, sampleOptions, new GUIContent("Samples", "The number of samples. Low quality results in better performance, and high quality result in lower performance."));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(amplitudeProp, new GUIContent("Amplitude", "Scales a level's magnitude. Linear is popular for processing, i.e. handling triggers, and Decible is good for visualizing."));

            EditorGUILayout.Space();

            if(GUILayout.Button("Reset"))
            {
                if (EditorUtility.DisplayDialog("Warning", "This will clear saved peaks and mark the asset as dirty. This operation cannot be undone. Continue?", "Yes", "No"))
                {
                    source.ResetPeaks();
                    EditorUtility.SetDirty(source);
                }
            }
            
            // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();

            if (source.isDirty)
            {
                EditorGUILayout.HelpBox("Peak data is dirty. Attach this asset to a SpectrumSource and hit Record Peaks in play mode.", MessageType.Warning);
            }

        }
    }
}
