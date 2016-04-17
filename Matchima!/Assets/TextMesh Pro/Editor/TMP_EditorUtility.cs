﻿// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace TMPro.EditorUtilities
{

    public static class TMP_EditorUtility
    {
        // Static Fields Related to locating the TextMesh Pro Asset
        private static bool isTMProFolderLocated;
        private static string folderPath = "Not Found";
        
        private static EditorWindow Gameview;
        private static bool isInitialized = false;

        private static void GetGameview()
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.GameView");
            Gameview = EditorWindow.GetWindow(type);
        }


        public static void RepaintAll()
        {
            if (isInitialized == false)
            {
                GetGameview();
                isInitialized = true;
            }

            SceneView.RepaintAll();
            Gameview.Repaint();
        }


        /// <summary>
        /// Create and return a new asset in a smart location based on the current selection and then select it.
        /// </summary>
        /// <param name="name">
        /// Name of the new asset. Do not include the .asset extension.
        /// </param>
        /// <returns>
        /// The new asset.
        /// </returns>
        public static T CreateAsset<T>(string name) where T : ScriptableObject
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path.Length == 0)
            {
                // no asset selected, place in asset root
                path = "Assets/" + name + ".asset";
            }
            else if (Directory.Exists(path))
            {
                // place in currently selected directory
                path += "/" + name + ".asset";
            }
            else {
                // place in current selection's containing directory
                path = Path.GetDirectoryName(path) + "/" + name + ".asset";
            }
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }



        // Function used to find all materials which reference a font atlas so we can update all their references.
        public static Material[] FindMaterialReferences(TMP_FontAsset fontAsset)
        {
            List<Material> refs = new List<Material>();
            Material mat = fontAsset.material;
            refs.Add(mat);

            // Get materials matching the search pattern.
            string searchPattern = "t:Material" + " " + mat.name.Replace(" Material", "");
            string[] materialAssetGUIDs = AssetDatabase.FindAssets(searchPattern);

            for (int i = 0; i < materialAssetGUIDs.Length; i++)
            {
                string materialPath = AssetDatabase.GUIDToAssetPath(materialAssetGUIDs[i]);
                Material targetMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                if (targetMaterial.HasProperty(ShaderUtilities.ID_MainTex) && targetMaterial.mainTexture != null && mat.mainTexture != null && targetMaterial.mainTexture.GetInstanceID() == mat.mainTexture.GetInstanceID())
                {
                    if (!refs.Contains(targetMaterial))
                        refs.Add(targetMaterial);
                }
            }

            return refs.ToArray();
        }


        // Function used to find the Font Asset which matches the given Material Preset and Font Atlas Texture.
        public static TMP_FontAsset FindMatchingFontAsset(Material mat)
        {
            if (mat.mainTexture == null) return null;

            int fontAtlasTextureID = mat.mainTexture.GetInstanceID();

            // Get Font Assets matching the search pattern.
            string searchPattern = "t:TMP_FontAsset";
            string[] fontAssetGUIDs = AssetDatabase.FindAssets(searchPattern);

            for (int i = 0; i < fontAssetGUIDs.Length; i++)
            {
                string fontAssetPath = AssetDatabase.GUIDToAssetPath(fontAssetGUIDs[i]);
                TMP_FontAsset targetFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);

                if (targetFontAsset.material != null && targetFontAsset.material.mainTexture != null && targetFontAsset.material.mainTexture.GetInstanceID() == fontAtlasTextureID)
                    return targetFontAsset;

            }

            return null;
        }


        /// <summary>
        /// Function to find the asset folder location in case it was moved by the user.
        /// </summary>
        /// <returns></returns>
        public static string GetAssetLocation()
        {
            if (isTMProFolderLocated == false)
            {
                isTMProFolderLocated = true;
                string projectPath = Directory.GetCurrentDirectory();
                
                // Find all the directories that match "TextMesh Pro"
                string[] matchingPaths = Directory.GetDirectories(projectPath + "/Assets", "TextMesh Pro", SearchOption.AllDirectories);

                folderPath = ValidateLocation(matchingPaths);
                if (folderPath != null) return folderPath;    

                // Check alternative Asset folder name.
                matchingPaths = Directory.GetDirectories(projectPath + "/Assets", "TextMeshPro", SearchOption.AllDirectories);
                folderPath = ValidateLocation(matchingPaths);
                if (folderPath != null) return folderPath;

            }

            if (folderPath != null) return folderPath;
            else
            {
                Debug.LogWarning("Could not located the \"TextMesh Pro/GUISkins\" Folder to load the Editor Skins.");
                return null;
            }
        }


        /// <summary>
        /// Method to validate the location of the asset folder by making sure the GUISkins folder exists.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private static string ValidateLocation(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                // Check if any of the matching directories contain a GUISkins directory.
                if (Directory.Exists(paths[i] + "/GUISkins"))
                {
                    folderPath = "Assets" + paths[i].Split(new string[] { "/Assets" }, System.StringSplitOptions.None)[1];
                    return folderPath;
                }
            }

            return null;
        }

    }
}
