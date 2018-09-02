// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;
using System.IO;

namespace LDG.SoundReactor
{
    public static class MenuItems
    {
        #region Helpers
        static void ParentAndUndo(GameObject gameObject, MenuCommand menuCommand)
        {
            GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeObject = gameObject;
        }

        static void ParentAndUndo(GameObject gameObject, GameObject parent)
        {
            GameObjectUtility.SetParentAndAlign(gameObject, parent);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
            Selection.activeObject = gameObject;
        }
        #endregion

        #region Menu Asset
        [MenuItem("Assets/Create/SoundReactor/Peaks Profile", false, 300)]
        static void PeaksProfileCreate()
        {
            PeaksProfile peaksProfile = ScriptableObject.CreateInstance<PeaksProfile>();

            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            FileAttributes attr = File.GetAttributes(assetPath);

            if (attr == FileAttributes.Directory)
            {
                ProjectWindowUtil.CreateAsset(peaksProfile, assetPath + "/New Peaks Profile.peaks.asset");
            }
            else
            {
                string path = Path.GetDirectoryName(assetPath);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);

                ProjectWindowUtil.CreateAsset(peaksProfile, path + "/" + fileName + ".peaks.asset");
            }
        }

        [MenuItem("Assets/Create/SoundReactor/Peaks Profile", true)]
        static bool ValidatePeaksProfileCreate()
        {
            return (AssetDatabase.GetAssetPath(Selection.activeObject) != "");
        }


        [MenuItem("Tools/SoundReactor/Peaks Profile", false, 111)]
        static void PeaksProfileAsset()
        {
            PeaksProfileCreate();
        }

        [MenuItem("Tools/SoundReactor/Peaks Profile", true)]
        static bool ValidatePeaksProfileAsset()
        {
            return ValidatePeaksProfileCreate();
        }
        #endregion

        #region Menu Component
        [MenuItem("Tools/SoundReactor/Component/SpectrumSource", false, 1)]
        static void SpectrumSourceComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(SpectrumSource));
        }

        [MenuItem("Tools/SoundReactor/Component/SpectrumSource", true)]
        static bool ValidateSpectrumSourceComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Component/SpectrumFilter", false, 1)]
        static void LevelSourceComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(SpectrumFilter));
        }

        [MenuItem("Tools/SoundReactor/Component/SpectrumFilter", true)]
        static bool ValidateLevelSourceComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Component/Level", false, 1)]
        static void LevelComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(Level));
        }

        [MenuItem("Tools/SoundReactor/Component/Level", true)]
        static bool ValidateLevelComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Component/EQ", false, 1)]
        static void EQComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(EQ));
        }

        [MenuItem("Tools/SoundReactor/Component/EQ", true)]
        static bool ValidateEQComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Component/Builder", false, 50)]
        static void BuilderComponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(SpectrumBuilder));
        }

        [MenuItem("Tools/SoundReactor/Component/Builder", true)]
        static bool ValidateBuilderComponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Event", false, 1)]
        static void EventDriverCmponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(EventDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Event", true)]
        static bool ValidateEventDriverCmponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Position", false, 1)]
        static void PositionDriverCmponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(PositionDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Position", true)]
        static bool ValidatePositionDriverCmponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Rotation", false, 1)]
        static void RotationDriverCmponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(RotateDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Rotation", true)]
        static bool ValidateRotationDriverCmponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Scale", false, 1)]
        static void ScaleDriverCmponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(ScaleDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Scale", true)]
        static bool ValidateScaleDriverCmponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Color", false, 1)]
        static void ColorDriverCmponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(ColorDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/Color", true)]
        static bool ValidateColorDriverCmponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/Force", false, 1)]
        static void ForceDriverCmponent()
        {
            if (Selection.activeGameObject)
            {
                Undo.AddComponent(Selection.activeGameObject, typeof(ForceDriver));
            }
        }

        [MenuItem("Tools/SoundReactor/Driver/Force", true)]
        static bool ValidateForceDriverCmponent()
        {
            return (Selection.activeGameObject != null);
        }



        [MenuItem("Tools/SoundReactor/Driver/ParticleEmitter", false, 1)]
        static void ParticleEmitterDriverCmponent()
        {
            Undo.AddComponent(Selection.activeGameObject, typeof(ParticleEmitterDriver));
        }

        [MenuItem("Tools/SoundReactor/Driver/ParticleEmitter", true)]
        static bool ValidateParticleEmitterDriverCmponent()
        {
            return (Selection.activeGameObject != null);
        }
        #endregion

        #region Menu GameObject
        [MenuItem("Tools/SoundReactor/SpectrumSource", false, 50)]
        static void SpectrumSourceGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumSource", new System.Type[] { typeof(SpectrumSource) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/SpectrumFilter", false, 50)]
        static void LevelSourceGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/SpectrumFilter (EQ)", false, 50)]
        static void EQGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter), typeof(EQ) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/Level", false, 50)]
        static void LevelGameObject()
        {
            GameObject gameObject = new GameObject("Level", new System.Type[] { typeof(Level) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }

        [MenuItem("Tools/SoundReactor/Builder", false, 100)]
        static void BuilderGameObject()
        {
            GameObject gameObject = new GameObject("SpectrumBuilder", new System.Type[] { typeof(SpectrumBuilder) });

            ParentAndUndo(gameObject, Selection.activeGameObject);
        }
        #endregion

        #region Context GameObject
        [MenuItem("GameObject/SoundReactor/SpectrumSource", false, 11)]
        static void CreateSpectrumSource(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumSource", new System.Type[] { typeof(SpectrumSource) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/SpectrumFilter", false, 11)]
        static void CreateSpectrumFilter(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/SpectrumFilter (EQ)", false, 11)]
        static void CreateEQ(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumFilter", new System.Type[] { typeof(SpectrumFilter), typeof(EQ) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/Level", false, 11)]
        static void CreateLevel(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("Level", new System.Type[] { typeof(Level) });

            ParentAndUndo(gameObject, menuCommand);
        }

        [MenuItem("GameObject/SoundReactor/Builder", false, 11)]
        static void CreateBuilder(MenuCommand menuCommand)
        {
            GameObject gameObject = new GameObject("SpectrumBuilder", new System.Type[] { typeof(SpectrumBuilder) });

            ParentAndUndo(gameObject, menuCommand);
        }

        #endregion
    }
}
