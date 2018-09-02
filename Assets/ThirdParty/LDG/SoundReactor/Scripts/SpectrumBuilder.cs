// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LDG.SoundReactor
{
    public enum ShapeMode { Line, Circle, Rectangle, SegmentedLevels }
    public enum VectorShapeMode { Line, Circle }
    public enum SegmentMode { Object, Vector }
    public enum SpacingMode { Spaced, Divided }

    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SpectrumBuilder : SerializeableObject
    {
        public ShapeMode shape = ShapeMode.Line;
        public SegmentMode segmentMode = SegmentMode.Object;
        public int numColumns = 7;
        public int numRows = 10;
        public Texture2D texture;

        public bool spacingFoldout = true;
        public SpacingMode spacingMode;
        public bool fit = false;
        public GameObject levelInstance;
        public bool shareDriver = true;
        public Vector2 layoutSize = new Vector2(10, 10);
        public Vector3 levelSize = Vector3.one;
        public Vector2 levelSpacing = new Vector2(0.1f, 0.1f);
        public float travel = 1.0f;

        public bool bandwidthFoldout = true;
        public FrequencyRangeOption frequencyRangeOption = FrequencyRangeOption.FullRange;
        public float frequencyLower = 20.0f;
        public float frequencyUpper = 20000.0f;

        public bool transformFoldout = true;
        public float transformRepeat = 1.0f;
        public bool transformAlternate = false;
        public bool transformReverse = false;
        public bool transformFlipLevel = false;

        public VectorShape vectorShape = null;
        public bool vectorAnchored = false;
        public float vectorAnchoredDiameter = 0.0f;
        public bool closeCurve = false;
        public ColorDriver colorDriver;
        public Material vectorMaterial;

        public GameObject Instantiate(GameObject original, Transform parent)
        {
            GameObject go;

#if UNITY_EDITOR
            if((go = (GameObject)PrefabUtility.InstantiatePrefab(levelInstance)) == null)
            {
#if UNITY_4_6
				go = (GameObject)Instantiate(levelInstance);
#else
				go = Instantiate<GameObject>(levelInstance);
#endif
            }
#else
            go = Instantiate<GameObject>(levelInstance);
#endif

            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;

            return go;
        }

        void SetLevelInfo(GameObject levelObject, GameObject sharedDriver, int arraySize, int index)
        {
            float linearizedFrequency = Mathf.Repeat((float)index / (float)(arraySize), 1.0f);

            SetLevelInfo(levelObject, sharedDriver, linearizedFrequency, 0.0f);
        }

        void SetLevelInfo(GameObject levelObject, GameObject sharedDriver, float linearizedFrequency, float normalizedLevel)
        {
            Level level;

            if ((level = levelObject.GetComponent<Level>()) == null)
            {
                level = levelObject.AddComponent<Level>();
            }
            
            if (level)
            {
                level.Set(linearizedFrequency, normalizedLevel, frequencyLower, frequencyUpper, transformRepeat, transformAlternate, transformReverse, transformFlipLevel);
            }

            if (shareDriver && sharedDriver)
            {
                AttachSharedDriver(levelObject, sharedDriver);
            }
        }

        void AttachSharedDriver(GameObject levelObject, GameObject sharedObject)
        {
            Transform instanceChild;

            PropertyDriver[] sharedDriver;
            PropertyDriver[] levelDriver;

            sharedDriver = sharedObject.GetComponents<PropertyDriver>();
            levelDriver = levelObject.GetComponents<PropertyDriver>();

            for (int i = 0; i < sharedDriver.Length; i++)
            {
                levelDriver[i].sharedDriver = sharedDriver[i];
            }

            foreach (Transform segmentChild in levelObject.transform)
            {
                instanceChild = sharedObject.transform.Find(segmentChild.name);

                AttachSharedDriver(segmentChild.gameObject, instanceChild.gameObject);
            }
        }

        void DeleteChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private float CalcColumnSpacing()
        {
            float spacing = 0.0f;

            switch(spacingMode)
            {
                case SpacingMode.Spaced:
                    spacing = levelSize.x + this.levelSpacing.x;
                    break;

                case SpacingMode.Divided:

                    if (fit)
                    {
                        spacing = (layoutSize.x - levelSize.x) / (numColumns - 1);
                    }
                    else
                    {
                        spacing = (layoutSize.x) / (numColumns - 1);
                    }

                    break;
            }

            return spacing;
        }

        private float CalcRowSpacing()
        {
            float spacing = 0.0f;

            switch (spacingMode)
            {
                case SpacingMode.Spaced:
                    spacing = levelSize.y + this.levelSpacing.y;
                    break;

                case SpacingMode.Divided:

                    if (fit)
                    {
                        spacing = (layoutSize.y - levelSize.y) / (numRows - 1);
                    }
                    else
                    {
                        spacing = (layoutSize.y) / (numRows - 1);
                    }

                    break;
            }

            return spacing;
        }
        
        public void BuildLine()
        {
            Transform levelTransform;

            float halfSize;
            float spacing = CalcColumnSpacing();

            halfSize = spacing * (numColumns - 1) * 0.5f;

            for (int i = 0; i < numColumns; i++)
            {
                levelTransform = new GameObject().transform;
                levelTransform.name = "Level" + i.ToString();

                Vector3 pos = levelTransform.localPosition;
                pos.x = -halfSize + i * spacing;

                levelTransform.parent = transform;
                levelTransform.localPosition = pos;
                levelTransform.localScale = levelSize;

                levelTransform.gameObject.AddComponent<Level>();

                SetLevelInfo(levelTransform.gameObject, null, numColumns, i);
            }

            vectorShape = new VectorShape(colorDriver, transform, true);
        }

        public void BuildCircle()
        {
            int nVertices = numColumns + 1;
            Transform levelTransform;

            float spacing = CalcColumnSpacing();
            float radius;
            float arc = (Mathf.PI * 2.0f) / (float)(nVertices - 1);

            if (spacingMode == SpacingMode.Spaced)
            {
                radius = spacing * (float)nVertices / Mathf.PI * 0.5f;
            }
            else
            {
                radius = layoutSize.x * 0.5f;
            }

            if (fit && spacingMode == SpacingMode.Divided)
            {
                radius -= levelSize.y * 0.5f;
            }

            for (int i = 0; i < nVertices; i++)
            {
                levelTransform = new GameObject().transform;
                levelTransform.name = "Level" + i.ToString();

                Vector3 pos = levelTransform.localPosition;
                pos.x = Mathf.Cos(i * arc + Mathf.PI * 0.5f) * radius;
                pos.y = Mathf.Sin(i * arc + Mathf.PI * 0.5f) * radius;

                levelTransform.parent = transform;
                levelTransform.localRotation = Quaternion.AngleAxis((arc * i) * Mathf.Rad2Deg, Vector3.forward);
                levelTransform.localPosition = pos;
                levelTransform.localScale = levelSize;

                levelTransform.gameObject.AddComponent<Level>();

                SetLevelInfo(levelTransform.gameObject, null, nVertices - 1, i);
            }

            vectorShape = new VectorShape(colorDriver, transform, false);
        }

        public void BuildLinearArray()
        {
            Transform levelTransform;

            float spacing;
            float halfSize;
            
            spacing = CalcColumnSpacing();
            halfSize = numColumns * spacing / 2 - spacing / 2;

            for (int i = 0; i < numColumns; i++)
            {
                levelTransform = Instantiate(levelInstance, transform).transform;
                levelTransform.name = "Level" + i.ToString();

                Vector3 pos = levelTransform.localPosition;
                pos.x = -halfSize + i * spacing;
                //pos.x = spacing;

                levelTransform.localPosition = pos;
                levelTransform.localScale = levelSize;

                SetLevelInfo(levelTransform.gameObject, levelInstance, numColumns, i);
            }
        }

        #if UNITY_EDITOR
        private bool TextureReadOnly(Object texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
            
            return !textureImporter.isReadable;
        }
        #endif

        public void BuildRectangularArray()
        {
            Transform levelTransform;

            if (texture == null)
            {
                Debug.Log("Missing texture");
                return;
            }

#if UNITY_EDITOR
            if (TextureReadOnly(texture))
            {
                Debug.LogWarning("Read/Write must be enabled on the texture");
                return;
            }
#endif

            numColumns = texture.width;
            numRows = texture.height;
            
            float columnSpacing = CalcColumnSpacing();
            float rowSpacing = CalcRowSpacing();

            float halfWidth = columnSpacing * texture.width / 2.0f - columnSpacing * 0.5f;
            float halfHeight = rowSpacing * texture.height / 2.0f - rowSpacing * 0.5f;
            int nameIndex = 0;

            for (int x = 0; x < texture.width; x++)
            {
                for(int y = 0; y < texture.height; y++)
                {
                    Color color = texture.GetPixel(x, y);

                    if (color.a != 0.0f)
                    {
                        levelTransform = Instantiate(levelInstance, transform).transform;
                        levelTransform.name = "Level" + nameIndex.ToString();
                        nameIndex++;

                        Vector3 pos = new Vector3((float)x * columnSpacing - halfWidth, (float)y * rowSpacing - halfHeight, 0.0f);
                        levelTransform.localPosition = pos;
                        levelTransform.localScale = levelSize;

                        SetLevelInfo(levelTransform.gameObject, levelInstance, color.r, 0.0f);
                    }
                }
            }
        }
        
        public void BuildVuMeter()
        {
            Transform levelTransform;
            
            float columnSpacing = CalcColumnSpacing();
            float rowSpacing = CalcRowSpacing();

            float halfWidth = columnSpacing * numColumns / 2.0f - columnSpacing * 0.5f;
            float halfHeight = rowSpacing * numRows / 2.0f - rowSpacing * 0.5f;

            float normalizedLevelHeight = 1.0f / (float)(numRows - 1);

            int nameIndex = 0;

            for (int x = 0; x < numColumns; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    levelTransform = Instantiate(levelInstance, transform).transform;
                    levelTransform.name = "Level" + nameIndex.ToString();

                    Vector3 pos = new Vector3((float)x * columnSpacing - halfWidth, (float)y * rowSpacing - halfHeight, 0.0f);
                    levelTransform.localPosition = pos;
                    levelTransform.localScale = levelSize;

                    float linearizedFrequency = (float)x / (float)(numColumns - 1);
                    float normalizedLevel = normalizedLevelHeight * (float)y + normalizedLevelHeight;

                    SetLevelInfo(levelTransform.gameObject, levelInstance, linearizedFrequency, normalizedLevel);

                    nameIndex++;

                }
            }
        }
        
        public void BuildPolarArray()
        {
            Transform levelTransform = gameObject.transform;
            float radius;
            int nPoints = numColumns + 1;

            float spacing = CalcColumnSpacing();
            float arc = 360.0f / (float)(nPoints - 1);

            if (spacingMode == SpacingMode.Spaced)
            {
                radius = spacing * (float)(nPoints) / Mathf.PI * 0.5f;
            }
            else
            {
                radius = layoutSize.x * 0.5f;
            }

            if(fit && spacingMode == SpacingMode.Divided)
            {
                radius -= levelSize.y * 0.5f;
            }

            for (int x = 0; x < nPoints - 1; x++)
            {
                levelTransform = Instantiate(levelInstance, transform).transform;
                levelTransform.name = "Level" + x.ToString();

                levelTransform.localRotation = Quaternion.AngleAxis(arc * x, Vector3.forward);
                levelTransform.localPosition = transform.InverseTransformDirection(levelTransform.up) * radius;
                levelTransform.localScale = levelSize;

                SetLevelInfo(levelTransform.gameObject, levelInstance, nPoints - 1, x);
            }
        }

        public void Build()
        {
            if (!levelInstance && segmentMode == SegmentMode.Object) return;

            DeleteChildren();

            switch (shape)
            {
                case ShapeMode.Line:
                    {
                        if (segmentMode == SegmentMode.Vector)
                        {
                            BuildLine();
                        }
                        else
                        {
                            BuildLinearArray();
                        }
                        break;
                    }

                case ShapeMode.Circle:
                    {
                        if (segmentMode == SegmentMode.Vector)
                        {
                            BuildCircle();
                        }
                        else
                        {
                            BuildPolarArray();
                        }

                        break;
                    }

                case ShapeMode.Rectangle:
                    {
                        BuildRectangularArray();
                        break;
                    }

                case ShapeMode.SegmentedLevels:
                    {
                        BuildVuMeter();
                        break;
                    }
            }
        }

        private void Start()
        {
            vectorShape = null;
        }

        // Update is called once per frame
        void OnRenderObject()
        {
            if (segmentMode == SegmentMode.Vector)
            {
                if (vectorShape == null)
                {
                    if (shape == ShapeMode.Line)
                    {
                        vectorShape = new VectorShape(colorDriver, transform, true);
                    }
                    else if (shape == ShapeMode.Circle)
                    {
                        vectorShape = new VectorShape(colorDriver, transform, false);
                    }
                }
                else
                {
                    if (vectorMaterial)
                    {
                        if (shape == ShapeMode.Line)
                        {
                            vectorShape.Draw(transform, layoutSize.x * 0.5f, levelSize.y, travel, true, vectorAnchored, vectorAnchoredDiameter, vectorMaterial);
                        }
                        else if (shape == ShapeMode.Circle)
                        {
                            vectorShape.Draw(transform, layoutSize.x * 0.5f, levelSize.y, travel, false, vectorAnchored, vectorAnchoredDiameter, vectorMaterial);
                        }
                    }
                }
            }
        }
    }

}