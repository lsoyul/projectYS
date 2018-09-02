using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDG.SoundReactor
{
    public class VertexElement
    {
        public VertexElement(Level level)
        {
            vertex = Vector3.zero;
            normal = Vector3.up;
            mainColor = Color.white;
            restingColor = Color.black;
            this.level = level;
        }

        public VertexElement(Level level, Vector3 vertex, Vector3 normal, Color mainColor, Color restingColor)
        {
            this.vertex = vertex;
            this.normal = normal;
            this.mainColor = mainColor;
            this.restingColor = restingColor;
            this.level = level;
        }

        public Vector3 vertex;
        public Vector3 normal;
        public Color mainColor;
        public Color restingColor;
        public Level level;
    }
}