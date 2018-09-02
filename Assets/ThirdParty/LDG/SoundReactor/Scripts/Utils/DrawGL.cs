// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG
{
    public struct DrawGL
    {
        private static Vector3 vertexCache;
        private static Color colorCache;
        private static bool firstVertex = true;

        public static void Rect(Rect rect, bool filled, Color color)
        {
            int mode;

            if (filled)
            {
                mode = GL.QUADS;
            }
            else
            {
                //mode = GL.LINE_STRIP;
				mode = GL.LINES;
            }

            GL.Begin(mode);
            GL.Color(color);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(rect.width, 0, 0);
            GL.Vertex3(rect.width, rect.height, 0);
            GL.Vertex3(0, rect.height, 0);
            GL.Vertex3(0, 0, 0);
            GL.End();
        }

        public static void Line(Vector2 p1, Vector2 p2, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(p1.x, p1.y, 0.0f);
            GL.Vertex3(p2.x, p2.y, 0.0f);
            GL.End();
        }

        public static void Begin(int mode)
        {
            GL.Begin(mode);

            firstVertex = true;
        }

        public static void LineStrip(Vector3 vertex, Color color)
        {
            if (firstVertex)
            {
                vertexCache = vertex;
                colorCache = color;

                firstVertex = false;
            }
            else
            {
                GL.Color(colorCache);
                GL.Vertex(vertexCache);

                GL.Color(color);
                GL.Vertex(vertex);

                vertexCache = vertex;
                colorCache = color;
            }
        }

        public static void Curve(Vector3[] vertices, Color[] colors, int segments, bool closed, InterpolationMode mode)
        {
            int segmentCount = (vertices.Length - 1) * segments;
            float normalizedIndex;

#if UNITY_5_6_OR_NEWER
            GL.Begin(GL.LINE_STRIP);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                GL.Vertex(Spline.Tween(normalizedIndex, vertices, closed, mode));
                GL.Color(Spline.Tween(normalizedIndex, colors, closed, mode));
            }
#else
            Begin(GL.LINES);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                LineStrip(Spline.Tween(normalizedIndex, vertices, closed, mode), Spline.Tween(normalizedIndex, colors, closed, mode));
            }
#endif

            GL.End();
        }

        public static void Line(Vector3[] vertices, Color[] colors, float width, int segments, bool closed, InterpolationMode mode)
        {
            int segmentCount = (vertices.Length - 1) * segments;
            float normalizedIndex;
            Vector3 vertexCache = Vector3.zero;
            Vector3 vertex;// = Vector3.zero;
            Vector3 normal;
            Vector3 dir;
            float slice = 1.0f / (float)(segmentCount);
            float halfWidth = width * 0.5f;

            GL.Begin(GL.TRIANGLE_STRIP);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                if (i == 0)
                {
                    vertex = Spline.Tween(normalizedIndex, vertices, closed, mode);

                    vertexCache = Spline.Tween(normalizedIndex + slice, vertices, closed, mode);
                    dir = (vertexCache - vertex).normalized;
                }
                else
                {
                    vertex = Spline.Tween(normalizedIndex, vertices, closed, mode);
                    dir = (vertex - vertexCache).normalized;
                }

                if (closed && (i == 0 || i == segmentCount))
                {
                    normal = Vector3.up;
                }
                else
                {
                    normal = Vector3.Cross(dir, Vector3.forward);
                }

                GL.Color(Spline.Tween(normalizedIndex, colors, closed, mode));

                GL.Vertex(vertex + normal * halfWidth);
                GL.Vertex(vertex + normal * -halfWidth);

                vertexCache = vertex;

            }

            GL.End();
        }

        public static void Ring(Vector3[] vertices, Color[] primaryColors, Color[] secondaryColors, float radius, float width, int segments, bool anchored, float anchoredDiameter, InterpolationMode mode)
        {
            int segmentCount = (vertices.Length - 1) * segments;
            float normalizedIndex;
            float halfWidth = width * 0.5f;
            float slice = 1.0f / (float)(segmentCount);
            float anchoredRadius = anchoredDiameter * 0.5f;

            Vector3 vertex;
            Vector3 normal;
            Vector3 dir;

            GL.Begin(GL.TRIANGLE_STRIP);

            for (int i = 0; i < segmentCount + 1; i++)
            {
                normalizedIndex = (float)(i) / (float)(segmentCount);

                if (i == 0)
                {
                    vertex = Spline.Tween(normalizedIndex, vertices, true, mode);

                    vertexCache = Spline.Tween(normalizedIndex + slice, vertices, true, mode);
                    dir = (vertexCache - vertex).normalized;
                }
                else
                {
                    vertex = Spline.Tween(normalizedIndex, vertices, true, mode);
                    dir = (vertex - vertexCache).normalized;
                }

                if (i == 0 || i == segmentCount)
                {
                    normal = Vector3.up;
                }
                else
                {
                    normal = Vector3.Cross(dir, Vector3.forward);
                }

                // ring outside
                GL.Color(Spline.Tween(normalizedIndex, primaryColors, true, mode));
                GL.Vertex(vertex + normal * halfWidth);

                // ring inside
                GL.Color(Spline.Tween(normalizedIndex, secondaryColors, true, mode));

                if (anchored)
                {
                    GL.Vertex(vertex.normalized * anchoredRadius);
                }
                else
                {
                    GL.Vertex(vertex - normal * halfWidth);
                }

                vertexCache = vertex;
            }

            GL.End();
        }

        public static void Graph(float[] values, float range, float width, float height, Color color, InterpolationMode mode)
        {

            // draw spectrum
#if UNITY_5_6_OR_NEWER
            GL.Begin(GL.LINE_STRIP);

            for (int i = 0; i < width; i++)
            {
                float iLevel = Mathf.Clamp01(1.0f - Spline.Tween((float)i / (width - 1), values, false, mode) / range) * height;

                GL.Color(color);
                GL.Vertex3(i, iLevel, 0);
            }
#else
            GL.Begin(GL.LINES);

            Vector2 xy = Vector2.zero;
            Vector2 xyPrev = xy;

            for (int i = 0; i < width; i++)
            {
                xy.Set(i, Mathf.Clamp01(1.0f - Spline.Tween((float)i / (width - 1), values, false, mode) / range) * height);

                if (i > 0)
                {
                    GL.Color(color);
                    GL.Vertex3(xyPrev.x, xyPrev.y, 0);
                    GL.Vertex3(xy.x, xy.y, 0);
                }

                xyPrev = xy;
            }
#endif

            GL.End();
        }
    }
}