using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script used to create and maintain the tunnel.
/// </summary>
/// 
public class TunnelManager : MonoBehaviour {

    [Header("Object Pool For Tunnel Segments")]
    //The prefab for the tunnel segment.
    public TunnelSegment tunnelSegmentPrefab;

    ///The total size of the object pool.
    public int objectPoolSize = 25;

    /// <summary>
    /// A List of objects waiting to be reused by the TunnelManager.
    /// </summary>
    public static List<TunnelSegment> Recyclables = new List<TunnelSegment>();
    private List<TunnelSegment> tempRecyclables = new List<TunnelSegment>();

    //A List of currently active tunnel segments.
    public static List<TunnelSegment> activeTunnelSegments = new List<TunnelSegment>();

    //The total number of frames that have passed.
    private int frames = 0;

    //The number of frames before an object recycling operation is initiated.
    [Tooltip("How many frames should pass between Recycling operations.")]
    public int recycleFrames = 30;

    [Header("Tunnel Segment Spacing.")]
    //The distance between tunnel segments.
    public float tunnelSegmentDistance = 2f;

    //The position of the most recently placed tunnel segment.
    private Vector3 lastTunnelSegmentPosition;

    //The total number of segments created so far.
    private int segmentNo = 0;

    [Header("Threat Generation")]
    //The maximum number of threats that can be activated per dificulty.
    [Tooltip("The maximum number of threats per tunnel ring segment")]
    public int maxThreats = 3;
    private int tempMaxThreats = 0;

    [Tooltip("The chance that a threat will be generated (0, 100)")]
    public int threatChance = 100;

    //Should threats be generated?
    public static bool generateThreats = false;

    //A list of segment numbers for threat generation.
    private List<int> segmentNumbers = new List<int>();
    private List<int> tempSegmentNumbers;

    //A list of segments to be activated on the tunnel ring segment.
    private List<int> segmentsToActivate = new List<int>();

    //Use this for initialisation
    void Awake()
    {
        Init();
    }

    /// <summary>
    ///Set up the Tunnel ready to commence play.
    /// </summary>
    void Init()
    {
        TunnelSegment tempSegment = null;

        //Instantiate the number of objects that we have declared in our objectPoolSize.
        for (int i = 0; i < objectPoolSize; i++)
        {
            TunnelSegment segment = Instantiate(tunnelSegmentPrefab, new Vector3(0, 0, lastTunnelSegmentPosition.z + tunnelSegmentDistance), Quaternion.identity) as TunnelSegment;

            Vector3 pos = segment.transform.position;
            segment.transform.parent = transform;

            //As the segments are created, add them to the activeTunnelSegment List so that we know they are in use.
            activeTunnelSegments.Add(segment);

            lastTunnelSegmentPosition = pos;

            //Increase the segmentNo so that we can keep track of the total number of segments created so far.
            segmentNo++;

            tempSegment = segment;
        }

        //Get the total number of segments to a tunnel ring section.
        for (int i = 0; i < tempSegment.segments.Count; i++)
        {
            segmentNumbers.Add(i);
        }

        //Get the maximum number of threats for the tempMaxThreats variable.
        tempMaxThreats = maxThreats;
    }

    // Update is called once per frame
    void Update()
    {
        //Add one to the frames variable every update cycle.
        frames++;

        //Check the Recyclables list every recycleFrames.
        if (frames % recycleFrames == 0)
        {
            frames = 0;
            RecycleObjects();
        }
    }

    /// <summary>
    /// Function used to recycle used tunnel segments.
    /// </summary>
    void RecycleObjects()
    {
        if (Recyclables.Count > 0)
        {
            foreach (TunnelSegment toReuse in Recyclables)
            {
                //Add this item to the temprary recyclables list.
                tempRecyclables.Add(toReuse);
            }

            foreach (TunnelSegment toReuse in tempRecyclables)
            {
                Recyclables.Remove(toReuse);

                //Move the item to be reused to the end of the tunnel.
                toReuse.transform.position = new Vector3(0, 0, lastTunnelSegmentPosition.z + tunnelSegmentDistance);

                //Set the lastTunnelSegmentPosition to the newly moved object.
                lastTunnelSegmentPosition = toReuse.transform.position;

                //As they are recycled, add the tunnel segments to the activeTunnelSegment list so that we know they are in use.
                activeTunnelSegments.Add(toReuse);

                //If generate threats is true, generate threats for this section.
                if (generateThreats)
                {
                    //Get a temporary copy of the segments List. 
                    //This allows us to remove selected segments from the list as they are chosen. Preventing duplicate selections.
                    tempSegmentNumbers = new List<int>(segmentNumbers);

                    //Get a random number of threat segments to activate on this tunnel ring segment.
                    int segments = Random.Range(0, maxThreats + 1);

                    int threatCheck = Random.Range(0, 101);

                    if (threatCheck < threatChance)
                    {
                        //Add the selected number of segments to the segmentsToActivate List.
                        for (int i = 0; i < segments; i++)
                        {
                            //Select a random number of threats.
                            int randomSegment = Random.Range(0, tempSegmentNumbers.Count);

                            //Remove the randomly selected segment so that it cannot be chosen again.
                            tempSegmentNumbers.RemoveAt(randomSegment);

                            //Add the randomly selected segment to the List of segments that need to be activated.
                            segmentsToActivate.Add(randomSegment);
                        }

                        //Tell the segment we are working on which threats it needs to activate.
                        toReuse.ActivateThreat(segmentsToActivate);
                    }

                    //If we have selected a number of threats that is greater than or equal to half the number of segments per tunnel ring
                    //then set a temporary maxThreats value so that the tunnel can not be entirely blocked off.
                    if (segments > 0 && segmentNumbers.Count / segments <= 2)
                    {
                        int remainingSegments = (segmentNumbers.Count - segments) - 1;

                        maxThreats = remainingSegments;
                    }
                    else maxThreats = tempMaxThreats;

                }

                //Clear the segments to activate list, ready for the next threat to be generated.
                segmentsToActivate.Clear();
            }

            //increase segmentNo so that we can keep track of the number of segments created so far.
            segmentNo++;

            tempRecyclables.Clear();
        }
    }

    /// <summary>
    /// Function used to clear all threats ahead of the player.
    /// </summary>
    public static void ClearRemainingThreats()
    {
        foreach (TunnelSegment segment in activeTunnelSegments)
        {
            segment.RestoreDefaults();
        }
    }
}
