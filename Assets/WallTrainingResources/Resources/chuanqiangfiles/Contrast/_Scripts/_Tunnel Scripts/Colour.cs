using UnityEngine;
using System.Collections;

/// <summary>
/// Used to generate a random colour.
/// </summary>
public class Colour : MonoBehaviour {

    //The coloured material shared by all threats.
    public Material sharedThreatMaterial;

    //The newly generated colour.
    private Color newColour;

    //Get a new random ColorHSV.
    public void GenerateColour()
    {
        newColour = Random.ColorHSV(0, 1, 0.54f, 0.56f, 0.7f, .72f);

        sharedThreatMaterial.color = newColour;
    }
}
