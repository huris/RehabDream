using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used to manage threats on each tunnel ring segment.
/// </summary>
public class TunnelSegment : MonoBehaviour {

    /// <summary>
    /// The List containing all segments of the attached tunnel ring prefab.
    /// </summary>
    public List<GameObject> segments = new List<GameObject>();
    private List<GameObject> tempSegments;

    /// <summary>
    /// The List containing all threat references.
    /// </summary>
    public List<GameObject> threats = new List<GameObject>();
    private List<GameObject> activeThreats = new List<GameObject>();

    /// <summary>
    /// Activate the passed threats on this runnel ring segment.
    /// </summary>
    public void ActivateThreat(List<int> activateSegments)
    {
        for (int i = 0; i < activateSegments.Count; i++)
        {
            //The segment index of the threat we need to activate.
            int index = activateSegments[i];

            threats[index].SetActive(true);
            activeThreats.Add(threats[index]);
        }
    }

    /// <summary>
    /// Restore the tunnel ring to its default state.
    /// </summary>
    public void RestoreDefaults()
    {
        for(int i = 0; i < activeThreats.Count; i++)
        {
            activeThreats[i].gameObject.SetActive(false);
        }

        activeThreats.Clear();
    }
    /// <summary>
    /// Used to detect when the player object has passed this tunnel ring segment. 
    /// </summary>
    void OnTriggerExit()
    {
        RestoreDefaults();

        //As the tunnel ring segment will no longer be visible to the player, flag it for reuse.
        TunnelManager.activeTunnelSegments.Remove(this);
        TunnelManager.Recyclables.Add(this);

        //As we exit this tunnel segment, increase the score.
        ScoreManager.UpdateScore();
    }
}
