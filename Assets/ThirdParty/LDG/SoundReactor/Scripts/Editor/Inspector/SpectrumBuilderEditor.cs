// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CustomEditor(typeof(SpectrumBuilder))]
    public class SpectrumBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            float lower = 20;
            float upper = 20000;

            SpectrumBuilder builder = (SpectrumBuilder)target;

            EditorGUILayout.Space();

            EditorGUILayout.Foldout(true, "Level", GlobalStyles.heading);
     
            builder.segmentMode = (SegmentMode)EditorGUILayout.EnumPopup(new GUIContent("Mode", "Creates a shape that is made up of segments or a vector."), builder.segmentMode);

            if (builder.segmentMode == SegmentMode.Vector)
            {
                builder.shape = (ShapeMode)Mathf.Min((int)builder.shape, (int)VectorShapeMode.Circle);
            }

            builder.levelSize = EditorGUILayout.Vector3Field("Size", builder.levelSize);

            if (builder.segmentMode == SegmentMode.Object)
            {
                builder.levelInstance = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Level", "The GameObject containing PropertyDriver(s)."), builder.levelInstance, typeof(GameObject), true);
                builder.shareDriver = EditorGUILayout.Toggle(new GUIContent("Share Driver", "Tells the builder to attach the PropertyDrivers from the Level to the instanced Levels."), builder.shareDriver);
            }
            else
            {
                if (builder.vectorShape != null)
                {
                    builder.colorDriver = (ColorDriver)EditorGUILayout.ObjectField(new GUIContent("Color Driver", ""), builder.colorDriver, typeof(ColorDriver), true);

                    if (!builder.colorDriver)
                    {
                        EditorGUILayout.HelpBox("A ColorDriver must be attached.", MessageType.Warning);
                    }

                    builder.vectorMaterial = (Material)EditorGUILayout.ObjectField("Material", builder.vectorMaterial, typeof(Material), true);

                    if (!builder.vectorMaterial)
                    {
                        EditorGUILayout.HelpBox("A Material must be attached.", MessageType.Warning);
                    }

                    builder.travel = EditorGUILayout.FloatField(new GUIContent("Travel", ""), builder.travel);
                    //builder.closeCurve = EditorGUILayout.Toggle(new GUIContent("Close Curve", ""), builder.closeCurve);
                    builder.vectorAnchored = EditorGUILayout.Toggle(new GUIContent("Anchored", "Anchor the bottom or inside edge of the shape."), builder.vectorAnchored);

                    if(builder.vectorAnchored)
                    {
                        builder.vectorAnchoredDiameter = EditorGUILayout.FloatField(new GUIContent("Anchored Diam.", "Diameter of the inside edge of the vector."), builder.vectorAnchoredDiameter);
                    }
                }
            }

            EditorGUILayout.Foldout(true, "Layout", GlobalStyles.heading);

            if (builder.segmentMode == SegmentMode.Object)
            {
                builder.shape = (ShapeMode)EditorGUILayout.EnumPopup(new GUIContent("Shape", "The shape to arrange the Levels into."), builder.shape);
            }
            else
            {
                builder.shape = (ShapeMode)EditorGUILayout.EnumPopup(new GUIContent("Shape", "The shape to arrange the Levels into."), (VectorShapeMode)builder.shape);
            }

            if (builder.spacingFoldout)
            {
                builder.spacingMode = (SpacingMode)EditorGUILayout.EnumPopup(new GUIContent("Spacing Mode", "How the Levels should be spaced. Levels can either be spaced evenly apart from each other, or they can be divided evenly based on the layout size."), builder.spacingMode);

                if (builder.spacingMode == SpacingMode.Divided)
                {
                    builder.fit = EditorGUILayout.Toggle(new GUIContent("Fit Inside", "Tells the segments to fit inside the layout size. Otherwise the segments are centered."), builder.fit);
                }

                switch (builder.shape)
                {
                    case ShapeMode.SegmentedLevels:
                        builder.numColumns = Mathf.Max(2, EditorGUILayout.IntField("Columns", builder.numColumns));
                        builder.numRows = Mathf.Max(2, EditorGUILayout.IntField("Rows", builder.numRows));

                        if (builder.spacingMode == SpacingMode.Divided)
                        {
                            builder.layoutSize = EditorGUILayout.Vector2Field("Layout Size", builder.layoutSize);
                        }

                        if (builder.spacingMode == SpacingMode.Spaced)
                        {
                            builder.levelSpacing = EditorGUILayout.Vector2Field("Spacing", builder.levelSpacing);
                        }

                        break;
                    case ShapeMode.Rectangle:
                        builder.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", builder.texture, typeof(Texture2D), true);

                        if (builder.spacingMode == SpacingMode.Divided)
                        {
                            builder.layoutSize = EditorGUILayout.Vector2Field("Layout Size", builder.layoutSize);
                        }

                        if (builder.spacingMode == SpacingMode.Spaced)
                        {
                            builder.levelSpacing = EditorGUILayout.Vector2Field("Spacing", builder.levelSpacing);
                        }

                        break;

                    default:
                        builder.numColumns = Mathf.Max(2, EditorGUILayout.IntField("Levels", builder.numColumns));

                        if (builder.spacingMode == SpacingMode.Divided)
                        {
                            builder.layoutSize.x = EditorGUILayout.FloatField("Layout Size", builder.layoutSize.x);
                        }

                        if (builder.spacingMode == SpacingMode.Spaced)
                        {
                            builder.levelSpacing.x = EditorGUILayout.FloatField("Spacing", builder.levelSpacing.x);
                        }
                        break;
                }
            }

            EditorGUILayout.Foldout(true, "Frequency Range", GlobalStyles.heading);

            if (builder.bandwidthFoldout)
            {
                builder.frequencyRangeOption = (FrequencyRangeOption)EditorGUILayout.EnumPopup("Preset", builder.frequencyRangeOption);

                if (builder.frequencyRangeOption == FrequencyRangeOption.Custom)
                {
                    lower = builder.frequencyLower;
                    upper = builder.frequencyUpper;
                }
                else
                {
                    Frequency.GetRangePreset(ref lower, ref upper, builder.frequencyRangeOption);
                }

                EditorGUI.BeginChangeCheck();

                builder.frequencyLower = Mathf.Min(EditorGUILayout.Slider("Lower (Hz)", lower, 20.0f, 20000.0f), upper);
                builder.frequencyUpper = Mathf.Max(EditorGUILayout.Slider("Upper (Hz)", upper, 20.0f, 20000.0f), lower);

                if (EditorGUI.EndChangeCheck())
                {
                    builder.frequencyRangeOption = FrequencyRangeOption.Custom;
                }
            }

            EditorGUILayout.Foldout(true, "Transformation", GlobalStyles.heading);

            if (builder.transformFoldout)
            {
                builder.transformRepeat = EditorGUILayout.FloatField(new GUIContent("Repeat", "The number of times to repeat the frequency range along the shape."), builder.transformRepeat);
                builder.transformAlternate = EditorGUILayout.Toggle(new GUIContent("Alternate", "Tells the frequency to reverse the frequency every time it repeats."), builder.transformAlternate);
                builder.transformReverse = EditorGUILayout.Toggle(new GUIContent("Reverse", "Causes the levels to be assigned frequencies starting with the highest frequency first."), builder.transformReverse);

                if (builder.shape == ShapeMode.SegmentedLevels)
                {
                    builder.transformFlipLevel = EditorGUILayout.Toggle(new GUIContent("Flip Level", "Causes the levels to display upside down. Only works with segmented levels."), builder.transformFlipLevel);
                }
            }

            if (GUI.changed || GUILayout.Button("Build"))
            {
                builder.Build();
                EditorUtility.SetDirty(target);
            }

            Undo.RecordObject(builder, "Spectrum Builder");
        }
    }
}