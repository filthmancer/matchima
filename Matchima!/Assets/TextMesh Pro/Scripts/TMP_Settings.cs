// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using System.Collections.Generic;

#pragma warning disable 0649 // Disabled warnings related to serialized fields not assigned in this script but used in the editor.

namespace TMPro
{
    [System.Serializable]
    [ExecuteInEditMode]
    public class TMP_Settings : ScriptableObject
    {
        private static TMP_Settings s_Instance;

        /// <summary>
        /// Controls if Word Wrapping will be enabled on newly created text objects by default.
        /// </summary>
        public static bool enableWordWrapping
        {
            get { return instance.m_enableWordWrapping; }
        }
        [SerializeField]
        private bool m_enableWordWrapping;

        /// <summary>
        /// Controls if Kerning is enabled on newly created text objects by default.
        /// </summary>
        public static bool enableKerning
        {
            get { return instance.m_enableKerning; }
        }
        [SerializeField]
        private bool m_enableKerning;

        /// <summary>
        /// Controls if Extra Padding is enabled on newly created text objects by default.
        /// </summary>
        public static bool enableExtraPadding
        {
            get { return instance.m_enableExtraPadding; }
        }
        [SerializeField]
        private bool m_enableExtraPadding;

        /// <summary>
        /// Controls if TintAllSprites is enabled on newly created text objects by default.
        /// </summary>
        public static bool enableTintAllSprites
        {
            get { return instance.m_enableTintAllSprites; }
        }
        [SerializeField]
        private bool m_enableTintAllSprites;

        /// <summary>
        /// Controls if Escape Characters will be parsed in the Text Input Box on newly created text objects.
        /// </summary>
        public static bool enableParseEscapeCharacters
        {
            get { return instance.m_enableParseEscapeCharacters; }
        }
        [SerializeField]
        private bool m_enableParseEscapeCharacters;

        /// <summary>
        /// Controls the display of warning message in the console.
        /// </summary>
        public static bool warningsDisabled
        {
            get { return instance.m_warningsDisabled; }
        }
        [SerializeField]
        private bool m_warningsDisabled;

        /// <summary>
        /// Returns the Default Font Asset to be used by newly created text objects.
        /// </summary>
        public static TMP_FontAsset defaultFontAsset
        {
            get { return instance.m_defaultFontAsset; }
        }
        [SerializeField]
        private TMP_FontAsset m_defaultFontAsset;

        /// <summary>
        /// Returns the list of Fallback Fonts defined in the TMP Settings file.
        /// </summary>
        public static List<TMP_FontAsset> fallbackFontAssets
        {
            get { return instance.m_fallbackFontAssets; }
        }
        [SerializeField]
        private List<TMP_FontAsset> m_fallbackFontAssets;

        /// <summary>
        /// The Default Sprite Asset to be used by default.
        /// </summary>
        public static TMP_SpriteAsset defaultSpriteAsset
        {
            get { return instance.m_defaultSpriteAsset; }
        }
        [SerializeField]
        private TMP_SpriteAsset m_defaultSpriteAsset;

        /// <summary>
        /// The Default Style Sheet used by the text objects.
        /// </summary>
        public static TMP_StyleSheet defaultStyleSheet
        {
            get { return instance.m_defaultStyleSheet; }
        }
        [SerializeField]
        private TMP_StyleSheet m_defaultStyleSheet;


        /// <summary>
        /// Get a singleton instance of the settings class.
        /// </summary>
        public static TMP_Settings instance
        {
            get
            {
                if (TMP_Settings.s_Instance == null)
                {
                    TMP_Settings.s_Instance = Resources.Load("TMP Settings") as TMP_Settings;
                }

                return TMP_Settings.s_Instance;
            }
        }


        /// <summary>
        /// Static Function to load the TMP Settings file.
        /// </summary>
        /// <returns></returns>
        public static TMP_Settings LoadDefaultSettings()
        {
            if (s_Instance == null)
            {
                // Load settings from TMP_Settings file
                TMP_Settings settings = Resources.Load("TMP Settings") as TMP_Settings;
                if (settings != null)
                    s_Instance = settings;
            }

            return s_Instance;
        }


        /// <summary>
        /// Returns the Sprite Asset defined in the TMP Settings file.
        /// </summary>
        /// <returns></returns>
        public static TMP_Settings GetSettings()
        {
            if (TMP_Settings.instance == null) return null;

            return TMP_Settings.instance;
        }


        /// <summary>
        /// Returns the Font Asset defined in the TMP Settings file.
        /// </summary>
        /// <returns></returns>
        public static TMP_FontAsset GetFontAsset()
        {
            if (TMP_Settings.instance == null) return null;

            return TMP_Settings.instance.m_defaultFontAsset;
        }


        /// <summary>
        /// Returns the Sprite Asset defined in the TMP Settings file.
        /// </summary>
        /// <returns></returns>
        public static TMP_SpriteAsset GetSpriteAsset()
        {
            if (TMP_Settings.instance == null) return null;

            return TMP_Settings.instance.m_defaultSpriteAsset;
        }


        /// <summary>
        /// Returns the Sprite Asset defined in the TMP Settings file.
        /// </summary>
        /// <returns></returns>
        public static TMP_StyleSheet GetStyleSheet()
        {
            if (TMP_Settings.instance == null) return null;

            return TMP_Settings.instance.m_defaultStyleSheet;
        }


    }
}
