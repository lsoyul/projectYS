// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;
using LDG;
using LDG.SoundReactor;


[InitializeOnLoad]
public class SpectrumGUILayout
{
    static Material mat;

    static bool mouseDown = false;

    static GUIStyle statsStyle = GUIStyle.none;
    static Vector2 markerPos;
    static Vector2 mousePosition = Vector2.zero;

    SpectrumGUILayout()
    {
    }

    static public float SpectrumField(SpectrumFilter spectrumFilter, float linearizedFrequency, float levelBeat)
    {
        float peakInverse;

        EditorGUILayout.Space();
        Rect rect = GUILayoutUtility.GetRect(120, 1000, 64, 64);

        bool insideRect = rect.Contains(Event.current.mousePosition);

        if (rect.Contains(Event.current.mousePosition))
        {
            mousePosition = Event.current.mousePosition - rect.min;

            GUI.changed = true;
        }

#if UNITY_2017_2_OR_NEWER
        if (Event.current.type == EventType.MouseDown && insideRect)
#else
        if (Event.current.type == EventType.mouseDown && insideRect)
#endif
        {
            markerPos = Event.current.mousePosition - rect.min;
            linearizedFrequency = markerPos.x / rect.width;

            mouseDown = true;

            GUI.changed = true;
        }
#if UNITY_2017_2_OR_NEWER
        else if (Event.current.type == EventType.MouseUp)
#else
        else if (Event.current.type == EventType.mouseUp)
#endif
        {
            mouseDown = false;
        }

#if UNITY_2017_2_OR_NEWER
        if ((Event.current.type == EventType.MouseDrag && mouseDown))
#else
        if ((Event.current.type == EventType.mouseDrag && mouseDown))
#endif
        {
            markerPos = Event.current.mousePosition - rect.min;
            linearizedFrequency = markerPos.x / rect.width;
            GUI.changed = true;
        }

        linearizedFrequency = Mathf.Clamp01(linearizedFrequency);

        GUI.BeginGroup(rect);
#if UNITY_2017_2_OR_NEWER
        if (Event.current.type == EventType.Repaint)
#else
        if (Event.current.type == EventType.repaint)
#endif
        {
            GL.Clear(true, false, Color.grey);

            if (mat == null)
            {
                var shader = Shader.Find("Hidden/LDG/UI");
                mat = new Material(shader);
            }

            mat.SetPass(0);

            // background
            DrawGL.Rect(rect, true, new Color(0.33f, 0.33f, 0.33f));

            if (spectrumFilter)
            {
                peakInverse = 1.0f - Mathf.Clamp01(levelBeat);

                // peak line
                DrawGL.Line(new Vector2(0, peakInverse * rect.height), new Vector2(rect.width, peakInverse * rect.height), new Color(0.15f, 0.15f, 0.15f));

                // mouse x pos
                if (insideRect)
                {
                    DrawGL.Line(new Vector2(mousePosition.x, 0), new Vector2(mousePosition.x, rect.height), new Color(0.15f, 0.15f, 0.15f));
                }

                if (EditorApplication.isPlaying && Time.frameCount > 5)
                {
                    //DrawGL.Curve(spectrumFilter.spectrumSource.normalizedPeaks, 1, rect.width, rect.height, new Color(0.2f, 0.2f, 0.2f));
                    DrawGL.Graph(spectrumFilter.fallingLevels, 1, rect.width, rect.height, new Color(0.0f, 0.5f, 1.0f), spectrumFilter.interpolationMode);

                    // draw spectrum
#if UNITY_5_6_OR_NEWER
                    GL.Begin(GL.LINE_STRIP);

                    for (int i = 0; i < rect.width; i++)
                    {
                        float iLevel = spectrumFilter.GetLevel((float)i / rect.width);

                        GL.Color(new Color(0.0f, 0.75f, 0.0f));
                        GL.Vertex3(i, Mathf.Clamp01(1.0f - iLevel) * rect.height, 0);
                    }
#else
                    GL.Begin(GL.LINES);

                    Vector2 xy = Vector2.zero;
                    Vector2 xyPrev = xy;

                    for (int i = 0; i < rect.width; i++)
                    {
                        xy.Set(i, Mathf.Clamp01(1.0f - spectrumFilter.GetLevel((float)i / rect.width)) * rect.height);

                        if (i > 0)
                        {
                            GL.Color(new Color(0.0f, 0.75f, 0.0f));
                            GL.Vertex3(xyPrev.x, xyPrev.y, 0);
                            GL.Vertex3(xy.x, xy.y, 0);
                        }

                        xyPrev = xy;
                    }
#endif

                    GL.End();
                }
            }

            DrawGL.Line(new Vector2(linearizedFrequency * rect.width, 0), new Vector2(linearizedFrequency * rect.width, rect.height), new Color(0.95f, 0.2f, 0.2f));

            // background outline
            DrawGL.Rect(rect, false, new Color(0.45f, 0.45f, 0.45f));
        }

        GUI.EndGroup();

        statsStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        statsStyle.alignment = TextAnchor.UpperRight;

        float freq = Frequency.UnlinearizeFrequency(1.0f - Mathf.Clamp01(mousePosition.x / rect.width));

        //GUI.Label(rect, "Freq: " + freq.ToString("n2") + ", Peak: " + levelBeat.ToString("n2") + " ", statsStyle);
        if(insideRect)
        {
            GUI.Label(rect, "Freq: " + freq.ToString("n2") + " ", statsStyle);
        }

        return linearizedFrequency;
    }

    static public void GraphField(float[] peaks)
    {
        EditorGUILayout.Space();
        Rect rect = GUILayoutUtility.GetRect(120, 1000, 64, 64);

        GUI.BeginGroup(rect);

#if UNITY_2017_2_OR_NEWER
        if (Event.current.type == EventType.Repaint)
#else
        if (Event.current.type == EventType.repaint)
#endif
        {
            GL.Clear(true, false, Color.grey);

            if (mat == null)
            {
                var shader = Shader.Find("Hidden/LDG/UI");
                mat = new Material(shader);
            }

            mat.SetPass(0);

            // background
            DrawGL.Rect(rect, true, new Color(0.33f, 0.33f, 0.33f));

            //if (masterLevel)
            {
                if (EditorApplication.isPlaying)
                {
                    // draw spectrum
#if UNITY_5_6_OR_NEWER
                    GL.Begin(GL.LINE_STRIP);
#else
                        GL.Begin(GL.LINES);
#endif

                    for (int i = 0; i < rect.width; i++)
                    {
                        float iLevel = Spline.Tween((float)i / (rect.width - 1), peaks, false, InterpolationMode.Curve);

                        GL.Color(new Color(0.0f, 0.75f, 0.0f));
                        GL.Vertex3(i, Mathf.Clamp01(1.0f - iLevel) * rect.height, 0);
                    }

                    GL.End();
                }
            }

            // background outline
            DrawGL.Rect(rect, false, new Color(0.45f, 0.45f, 0.45f));
        }

        GUI.EndGroup();
    }
}
