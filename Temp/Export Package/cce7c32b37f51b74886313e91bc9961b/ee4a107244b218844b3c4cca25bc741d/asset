using UnityEngine;
using System.Collections;

public class WMG_Ring : WMG_GUI_Functions {
	
	public GameObject ring;
	public GameObject band;
	public GameObject label;
	public GameObject labelText;
	public GameObject labelPoint;
	public GameObject labelBackground;

	private Sprite ringSprite;
	private Sprite bandSprite;
	private Color[] ringColors;
	private Color[] bandColors;
	private WMG_Ring_Graph graph;
	private int ringTexSize;
	private int bandTexSize;

	public void initialize(WMG_Ring_Graph graph) {
		ringSprite = WMG_Util.createSprite(getTexture(ring));
		bandSprite = WMG_Util.createSprite(getTexture(band));
		ringTexSize = ringSprite.texture.width;
		bandTexSize = bandSprite.texture.width;
		ringColors = new Color[ringTexSize * ringTexSize];
		bandColors = new Color[bandTexSize * bandTexSize];
		setTexture(ring, ringSprite);
		setTexture(band, bandSprite);
		this.graph = graph;
		changeSpriteParent(label, graph.ringLabelsParent);
	}

	public void updateRing(int ringNum) {
		float ringRadius = graph.getRingRadius(ringNum);
		// label points
		changeSpritePositionToY(labelPoint, -(ringRadius - graph.ringWidth / 2));
		// rings
		graph.updateRingColors(ref ringColors, ringRadius - graph.ringWidth, ringRadius);
		ringSprite.texture.SetPixels(ringColors);
		ringSprite.texture.Apply();
		// bands
		if (graph.bandMode) {
			SetActive(band, true);
			graph.updateRingColors(ref bandColors, ringRadius + graph.bandPadding, graph.getRingRadius(ringNum + 1) - graph.ringWidth - graph.bandPadding);
			bandSprite.texture.SetPixels(bandColors);
			bandSprite.texture.Apply();
		}
		else {
			SetActive(band, false);
		}

	}

}
