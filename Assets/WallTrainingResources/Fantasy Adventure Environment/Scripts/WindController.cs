// Fantasy Adventure Environment
// Copyright Staggart Creations
// staggart.xyz

using UnityEngine;
using System.Collections;

namespace FAE
{
#if UNITY_EDITOR
    using UnityEditor;
    [ExecuteInEditMode]
#endif

    /// <summary>
    /// Sets the wind properties of the FAE shaders
    /// </summary>
    public class WindController : MonoBehaviour
    {

        [Header("Vector map")]
#if UNITY_EDITOR
        public ProceduralMaterial windSubstance;
#endif
        public Texture windVectors;
        public bool visualizeVectors = false;
        /// <summary>
        /// Used to retreive the current state of the wind visualization, either on or off
        /// </summary>
        public static bool _visualizeVectors;

        [Header("Wind settings")]
        [Range(0f, 1f)]
        /// <summary>Wind direction </summary>
        /// <value>The 360 degree Y-axis direction represented as a 0-1 value</value>
        public float windDirection = 0f;
        [Range(0f, 1f)]
        /// <summary>
        /// Name property. </summary>
        /// <value>
        /// A value tag is used to describe the property value.</value>
        public float windSpeed = 0.33f;
        [Range(0f, 1f)]
        public float windStrength = 0.5f;
        [Range(0f, 32f)]
        public float windAmplitude = 14f;

        //[Header("Tree trunks")]
        [Range(0f, 150f)]
        public float trunkWindSpeed = 10f;
        [Range(0f, 30f)]
        public float trunkWindWeight = 4f;
        [Range(0f, 0.99f)]
        public float trunkWindSwinging = 0.5f;

        private float m_windAmplitude = 0f;

        //Current wind parameters to be read externally
        public static float _windStrength;
        public static float _windAmplitude;

#if UNITY_EDITOR
        [MenuItem("GameObject/3D Object/FAE Wind Controller")]
        private static void NewMenuOption()
        {
            WindController currentWindController = GameObject.FindObjectOfType<WindController>();
            if (currentWindController != null)
            {
                if (EditorUtility.DisplayDialog("FAE Wind Controller", "A WindController object already exists in your scene", "Create anyway", "Cancel"))
                {
                    CreateNewWindController();
                }
            }
            else
            {
                CreateNewWindController();
            }
        }

        private static void CreateNewWindController()
        {
            GameObject newWindController = new GameObject()
            {
                name = "WindController"
            };
            newWindController.AddComponent<WindController>();

            Undo.RegisterCreatedObjectUndo(newWindController, "Created Wind Controller");
        }
#endif

        /// <summary>
        /// Set the wind strength
        /// </summary>
        /// <param name="value"></param>
        public void SetStrength(float value)
        {
            windStrength = value;

            SetShaderParameters();
        }


        void OnEnable()
        {
            windDirection = this.transform.localEulerAngles.y / 360f;
            m_windAmplitude = windAmplitude;

#if UNITY_EDITOR
            FindSubstance();

#if UNITY_5_5_OR_NEWER
            visualizeVectors = (Shader.GetGlobalFloat("_WindDebug") == 1) ? true : false;
#endif


            if (windVectors == null)
            {
                GetSubstanceOutput();
            }
#endif

            SetShaderParameters();
        }

        public void Apply()
        {
#if UNITY_EDITOR
            this.transform.localEulerAngles = new Vector3(0f, windDirection * 360f, 0f);

            //Sync the static var to the local var
            visualizeVectors = _visualizeVectors;
            VisualizeVectors(visualizeVectors);

            SetShaderParameters();

            SetSubstanceParameters();
#endif
        }

        private void SetShaderParameters()
        {
            Shader.SetGlobalTexture("_WindVectors", windVectors);
            Shader.SetGlobalFloat("_WindSpeed", windSpeed);
            Shader.SetGlobalFloat("_WindStrength", windStrength);
            Shader.SetGlobalVector("_WindDirection", this.transform.rotation * Vector3.back);

            Shader.SetGlobalFloat("_TrunkWindSpeed", trunkWindSpeed);
            Shader.SetGlobalFloat("_TrunkWindWeight", trunkWindWeight);
            Shader.SetGlobalFloat("_TrunkWindSwinging", trunkWindSwinging);

            //Set static var
            WindController._windStrength = windStrength;
            WindController._windAmplitude = windAmplitude;

        }

#if UNITY_EDITOR

        private void OnDisable()
        {
            VisualizeVectors(false);
        }

        //Avoid constantly rebuilding the Substance by checking if the values have changed
        private void SetSubstanceParameters()
        {


            //Wind amplitude
            if (m_windAmplitude != windAmplitude)
            {
                windSubstance.SetProceduralFloat("windAmplitude", windAmplitude);
                if (!windSubstance.isProcessing) windSubstance.RebuildTexturesImmediately();
            }
            m_windAmplitude = windAmplitude;

        }

        //Looks for the FAE_WindVectors substance in the project
        public void FindSubstance()
        {
            //Substance already assigned, no need to find it
            if (windSubstance) return;

            string[] assets = AssetDatabase.FindAssets("t:ProceduralMaterial FAE_WindVectors");
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

            SubstanceImporter si = AssetImporter.GetAtPath(assetPath) as SubstanceImporter; //Substance .sbsar container
            ProceduralMaterial[] substanceContainer = si.GetMaterials();

            //Look for the substance instance matching the material name we're looking for
            foreach (ProceduralMaterial substanceInstance in substanceContainer)
            {
                if (substanceInstance.name == "FAE_WindVectors")
                {
                    windSubstance = substanceInstance; //Gotcha

                    GetSubstanceOutput();
                }
            }

            //Debug.Log("Found substance: " + windSubstance.name);
        }
#endif

        /// <summary>
        /// Toggles the visualization of the wind vectors on all shaders that feature wind animations
        /// </summary>
        /// <param name="state">sadsdasda</param>
        public static void VisualizeVectors(bool state)
        {
            _visualizeVectors = state;
            Shader.SetGlobalFloat("_WindDebug", state ? 1f : 0f);
        }

#if UNITY_EDITOR
        private void GetSubstanceOutput()
        {

            if (!windSubstance)
            {
                Debug.Log(this.name + ": FAE_WindVectors Substance material is not set!");
                this.enabled = false;
                return;
            }

            windSubstance.RebuildTexturesImmediately();
            Texture[] substanceOutputs = windSubstance.GetGeneratedTextures();
            windVectors = substanceOutputs[0];

        }
#endif


    }
}