using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDG.SoundReactor
{
    public class VectorShape
    {
        public VertexElement[] vertexElements;

        private Vector3[] vertices;
        private Vector3[] normals;
        private Color[] mainColors;
        private Color[] restingColors;

        public VectorShape(ColorDriver sharedDriver, Transform transform, bool isLine)
        {
            vertexElements = new VertexElement[transform.childCount];

            Level level;
            VertexElementColor vertexColor;
            ColorDriver colorDriver;

            int i = 0;

            foreach (Transform levelTransform in transform)
            {
                level = levelTransform.GetComponent<Level>();

                if ((vertexColor = levelTransform.GetComponent<VertexElementColor>()) == null)
                {
                    vertexColor = levelTransform.gameObject.AddComponent<VertexElementColor>();
                }

                if ((colorDriver = levelTransform.GetComponent<ColorDriver>()) == null)
                {
                    colorDriver = levelTransform.gameObject.AddComponent<ColorDriver>();
                }

                colorDriver.sharedDriver = sharedDriver;

                if (isLine)
                {
                    vertexElements[i] = new VertexElement(level, levelTransform.localPosition, Vector3.Normalize(Vector3.up), Color.white, Color.black);
                }
                else
                {
                    vertexElements[i] = new VertexElement(level, levelTransform.localPosition, Vector3.Normalize(levelTransform.localPosition), Color.white, Color.black);
                }

                vertexColor.index = i++;
                vertexColor.vectorShape = this;
            }
        }

        private void UpdateVertexBuffer(int numVertices)
        {
            if (vertices == null || vertices.Length != numVertices)
            {
                vertices = new Vector3[numVertices];
            }

            if (normals == null || normals.Length != numVertices)
            {
                normals = new Vector3[numVertices];
            }

            if (mainColors == null || mainColors.Length != numVertices)
            {
                mainColors = new Color[numVertices];
            }

            if (restingColors == null || restingColors.Length != numVertices)
            {
                restingColors = new Color[numVertices];
            }
        }

        public void Draw(Transform transform, float layoutSize, float elementSize, float travel, bool isLine, bool anchored, float anchoredDiameter, Material material)
        {
            if (vertexElements == null || vertexElements.Length == 0)
            {
                return;
            }

            UpdateVertexBuffer(vertexElements.Length);

            if (material)
            {
                // Apply the line material
                material.SetPass(0);
            }

            for (int i = 0; i < vertexElements.Length; i++)
            {
                if (vertexElements[i].level)
                {
                    vertices[i] = vertexElements[i].vertex + vertexElements[i].normal * vertexElements[i].level.fallingLevel * travel;
                    normals[i] = vertexElements[i].normal;
                    mainColors[i] = vertexElements[i].mainColor;
                    restingColors[i] = vertexElements[i].restingColor;
                }
            }

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            if (isLine)
            {
                DrawGL.Line(vertices, mainColors, elementSize, 10, false, InterpolationMode.Curve);
            }

            if (!isLine)
            {
                DrawGL.Ring(vertices, mainColors, restingColors, layoutSize, elementSize, 10, anchored, anchoredDiameter, InterpolationMode.Curve);
            }

            GL.PopMatrix();
        }
    }
}