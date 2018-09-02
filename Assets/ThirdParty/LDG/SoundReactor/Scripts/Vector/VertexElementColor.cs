using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LDG.SoundReactor
{
    public class VertexElementColor : MonoBehaviour
    {
        public int index;
        public Color mainColor = Color.white;
        public Color restingColor = Color.black;

        public VectorShape vectorShape;

        private void Update()
        {
            if (vectorShape != null)
            {
                vectorShape.vertexElements[index].mainColor = mainColor;
                vectorShape.vertexElements[index].restingColor = restingColor;
            }
        }
    }
}
