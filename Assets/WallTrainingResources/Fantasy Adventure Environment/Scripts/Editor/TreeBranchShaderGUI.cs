// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace FAE
{
    public class TreeBranchShaderGUI : ShaderGUI
    {

        MaterialProperty _MaskClipValue;

        //Main maps
        MaterialProperty _MainTex;
        MaterialProperty _BumpMap;

        //Color
        MaterialProperty _VariationColor;
        MaterialProperty _AmbientOcclusion;
        MaterialProperty _TransmissionColor;
        MaterialProperty _GradientBrightness;
        MaterialProperty _Smoothness;
        MaterialProperty _FlatLighting;

        //Animation
        MaterialProperty _MaxWindStrength;
        MaterialProperty _WindAmplitudeMultiplier;

        MaterialEditor m_MaterialEditor;

        //Meta
        bool showHelp;
        bool showHelpColor;
        bool showHelpAnimation;

        bool hasWindController;
        WindController windController;

        GUIContent mainTexName = new GUIContent("Diffuse", "Diffuse (RGB) and Transparency (A)");
        GUIContent normalMapName = new GUIContent("Normal Map");
        private bool visualizeVectors;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            if (windController == null) LocateWindController();
            this.FindProperties(props);

            //Receive
            visualizeVectors = WindController._visualizeVectors;

            this.m_MaterialEditor = materialEditor;

            //Style similar to Standard shader
            m_MaterialEditor.SetDefaultGUIWidths();
            m_MaterialEditor.UseDefaultMargins();
            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();

            //Draw fields
            DoHeader();

            DoMapsArea();
            DoColorArea();
            DoAnimationArea();

            if (EditorGUI.EndChangeCheck())
            {
                //Send
                WindController.VisualizeVectors(visualizeVectors);
            }

            GUIHelper.DrawExtraFields(m_MaterialEditor);
            GUIHelper.DrawFooter();

        }

        void DoHeader()
        {
            EditorGUILayout.BeginHorizontal();
            showHelp = GUILayout.Toggle(showHelp, "Toggle help", "Button");
            GUILayout.Label("FAE Tree Branch Shader", GUIHelper.Header);
            EditorGUILayout.EndHorizontal();
            if (showHelp) EditorGUILayout.HelpBox("Please bear in mind, when using custom meshes, that most shader feature require the ambient occlusion to be baked into the vertex colors.", MessageType.Warning);
        }

        void DoMapsArea()
        {
            GUILayout.Label("Main maps", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(_MaskClipValue.displayName);
            _MaskClipValue.floatValue = EditorGUILayout.Slider(_MaskClipValue.floatValue, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            this.m_MaterialEditor.TexturePropertySingleLine(mainTexName, this._MainTex);
            this.m_MaterialEditor.TexturePropertySingleLine(normalMapName, this._BumpMap);

            EditorGUILayout.Space();
        }

        void DoColorArea()
        {
            EditorGUILayout.BeginHorizontal();
            showHelpColor = GUILayout.Toggle(showHelpColor, "?", "Button", GUILayout.Width(25f)); GUILayout.Label("Color", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            m_MaterialEditor.ShaderProperty(_VariationColor, _VariationColor.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Uses the object's world-space position to add a color variation. This effect is controlled through the alpha channel.\n\n Note: Does not work with meshes that are batched or combined", MessageType.None);
            m_MaterialEditor.ShaderProperty(_AmbientOcclusion, _AmbientOcclusion.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Darkens the areas of the mesh where red vertex colors are applied", MessageType.None);
            m_MaterialEditor.ShaderProperty(_TransmissionColor, _TransmissionColor.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Simulates light passing through the material. This effect is controlled through the alpha channel.", MessageType.None);
            m_MaterialEditor.ShaderProperty(_GradientBrightness, _GradientBrightness.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Adds a gradient to the branch mesh from bottom to top. This information is stored in the blue vertex color channel.\n\nWithout this information, the parameter will have no effect.", MessageType.None);
            m_MaterialEditor.ShaderProperty(_Smoothness, _Smoothness.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("Determines the shininess of the material", MessageType.None);
            m_MaterialEditor.ShaderProperty(_FlatLighting, _FlatLighting.displayName);
            if (showHelpColor) EditorGUILayout.HelpBox("A value of 1 makes the mesh normals point upwards. For some trees this is necessary to achieve the best visual result.", MessageType.None);

            EditorGUILayout.Space();
        }

        void DoAnimationArea()
        {
            EditorGUILayout.BeginHorizontal();
            showHelpAnimation = GUILayout.Toggle(showHelpAnimation, "?", "Button", GUILayout.Width(25f)); GUILayout.Label("Wind animation", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();


            if (!hasWindController)
            {
                EditorGUILayout.HelpBox("No \"WindController\" component was found in your scene. Please add this script to an empty GameObject\n\nA prefab can be found in the Prefabs/Effects folder.", MessageType.Warning);
                EditorGUI.BeginDisabledGroup(true);

            }

            visualizeVectors = EditorGUILayout.Toggle("Visualize wind", visualizeVectors);
            if (showHelpAnimation) EditorGUILayout.HelpBox("Toggle a visualisation of the wind vectors on all the objects that use FAE shaders featuring wind.\n\nThis allows you to more clearly see the effects of the settings.", MessageType.None);

            m_MaterialEditor.ShaderProperty(_MaxWindStrength, new GUIContent("Max wind strength"));
            if (showHelpAnimation) EditorGUILayout.HelpBox("Determines how much influence the wind has on the branches", MessageType.None);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Wind Amplitude Multiplier"));
            _WindAmplitudeMultiplier.floatValue = EditorGUILayout.FloatField(_WindAmplitudeMultiplier.floatValue, GUILayout.Width(65f));
            EditorGUILayout.EndHorizontal();
            //m_MaterialEditor.ShaderProperty(_WindAmplitudeMultiplier, _WindAmplitudeMultiplier.displayName);
            if (showHelpAnimation) EditorGUILayout.HelpBox("Multiply the wind amplitude for this material. Essentally this is the size of the wind waves.", MessageType.None);

            if (hasWindController && showHelpAnimation)
            {
                GUIHelper.DrawWindInfo();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
        }

        void LocateWindController()
        {
            //Debug.Log("Searching scene for WindController script");
            windController = GameObject.FindObjectOfType<WindController>();
            hasWindController = (windController) ? true : false;
        }

        public void FindProperties(MaterialProperty[] props)
        {
            //Rendering
            _MaskClipValue = FindProperty("_Cutoff", props);

            //Main maps
            _MainTex = FindProperty("_MainTex", props);
            _BumpMap = FindProperty("_BumpMap", props);

            //Color
            _VariationColor = FindProperty("_VariationColor", props);
            _AmbientOcclusion = FindProperty("_AmbientOcclusion", props);
            _TransmissionColor = FindProperty("_TransmissionColor", props);
            _GradientBrightness = FindProperty("_GradientBrightness", props);
            _Smoothness = FindProperty("_Smoothness", props);
            _FlatLighting = FindProperty("_FlatLighting", props);

            //Animation
            _MaxWindStrength = FindProperty("_MaxWindStrength", props);
            _WindAmplitudeMultiplier = FindProperty("_WindAmplitudeMultiplier", props);

        }

    }
}
