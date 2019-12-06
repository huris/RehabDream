using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WMG_Ring_Graph : WMG_Graph_Manager {

	public bool autoRefresh;

	public bool bandMode;
	public bool animateData;
	public float animDuration;
	public Ease animEaseType;

	public bool updateFromContainer;

	public float outerRadius;
	public float innerRadius;
	public bool innerPercentageOfOuter;
	public float degrees;
	public float minValue;
	public float maxValue;
	public Color bandColor;
	public List<Color> bandColors;
	public bool autoUpdateBandAlpha;
	public Color ringColor;
	public float ringWidth;
	public float bandPadding;
	public float labelLinePadding;
	public Vector2 leftRightPadding;
	public Vector2 topBotPadding;
	public bool antiAliasing;
	public float antiAliasingStrength;

	public bool autostackLabels;
	public float autostasckLabelsDegrees;

	public List<float> values;
	public List<string> labels;
	public List<string> ringIDs;
	
	public Object ringPrefab;
	public GameObject extraRing;
	public GameObject background;
	public GameObject zeroLine;
	public GameObject zeroLineText;
	public GameObject ringsParent;
	public GameObject ringLabelsParent;
	
	public WMG_Data_Source valuesDataSource;
	public WMG_Data_Source labelsDataSource;
	public WMG_Data_Source ringIDsDataSource;

	// Original property values for use with dynamic resizing
	private float origInnerPercentageOfOuter;

	// cache
	private bool bandModeCached;
	private float outerRadiusCached;
	private float innerRadiusCached;
	private bool innerPercentageOfOuterCached;
	private float degreesCached;
	private float ringWidthCached;
	private float bandPaddingCached;
	private float labelLinePaddingCached;
	private Vector2 leftRightPaddingCached;
	private Vector2 topBotPaddingCached;
	private float minValueCached;
	private float maxValueCached;
	private Color bandColorCached;
	private List<Color> bandColorsCached = new List<Color>();
	private bool autoUpdateBandAlphaCached;
	private Color ringColorCached;
	private bool antiAliasingCached;
	private float antiAliasingStrengthCached;
	private List<float> valuesCached = new List<float>();
	private List<string> labelsCached = new List<string>();
	private List<string> ringIDsCached = new List<string>();
	private float containerWidthCached;
	private float containerHeightCached;

	// change flags
	private bool radiusChanged;
	private bool innerPercentageChanged;
	private bool bandModeChanged;
	private bool ringWidthChanged;
	private bool labelLinePaddingChanged;
	private bool paddingChanged;
	private bool degreesChanged;
	private bool minMaxChanged;
	private bool bandColorChanged;
	private bool ringColorChanged;
	private bool valuesChanged;
	private bool labelsChanged;
	private bool ringIDsChanged;
	private bool antiAliasingChanged;
	private bool containerChanged;
	private bool numRingsChanged;

	// public getter
	public List<WMG_Ring> rings { get; private set; }

	// private
	private Sprite extraRingSprite;
	private Color[] extraRingColors;
	private int ringTexSize;


	void Start() {
		extraRingSprite = WMG_Util.createSprite(getTexture(extraRing));
		ringTexSize = extraRingSprite.texture.width;
		extraRingColors = new Color[ringTexSize * ringTexSize];
		setTexture(extraRing, extraRingSprite);
		rings = new List<WMG_Ring>();
	}

	void Update() {
		if (autoRefresh) {
			Refresh();
		}
	}

	public void Refresh() {
		checkCache();
		updateFromDataSource();
		updateContainer();
		updateNumberRings();
		updateInnerPercentage();
		updateRingsAndBands();
		updateDegrees();
		updateRingColors();
		updateBandColors();
		updateOuterRadius();
		updateLabelsText();
		updateBackground();
		setCacheFlags(false);
	}

	void checkCache() {
		updateCacheAndFlag<float>(ref outerRadiusCached, outerRadius, ref radiusChanged);
		updateCacheAndFlag<float>(ref innerRadiusCached, innerRadius, ref radiusChanged);
		updateCacheAndFlag<bool>(ref innerPercentageOfOuterCached, innerPercentageOfOuter, ref innerPercentageChanged);
		updateCacheAndFlag<bool>(ref bandModeCached, bandMode, ref bandModeChanged);
		updateCacheAndFlag<float>(ref ringWidthCached, ringWidth, ref ringWidthChanged);
		updateCacheAndFlag<float>(ref bandPaddingCached, bandPadding, ref ringWidthChanged);
		updateCacheAndFlag<float>(ref labelLinePaddingCached, labelLinePadding, ref labelLinePaddingChanged);
		updateCacheAndFlag<Vector2>(ref leftRightPaddingCached, leftRightPadding, ref paddingChanged);
		updateCacheAndFlag<Vector2>(ref topBotPaddingCached, topBotPadding, ref paddingChanged);
		updateCacheAndFlag<float>(ref degreesCached, degrees, ref degreesChanged);
		updateCacheAndFlag<float>(ref minValueCached, minValue, ref minMaxChanged);
		updateCacheAndFlag<float>(ref maxValueCached, maxValue, ref minMaxChanged);
		updateCacheAndFlag<Color>(ref bandColorCached, bandColor, ref bandColorChanged);
		updateCacheAndFlagList<Color>(ref bandColorsCached, bandColors, ref bandColorChanged);
		updateCacheAndFlag<bool>(ref autoUpdateBandAlphaCached, autoUpdateBandAlpha, ref bandColorChanged);
		updateCacheAndFlag<Color>(ref ringColorCached, ringColor, ref ringColorChanged);
		updateCacheAndFlag<bool>(ref antiAliasingCached, antiAliasing, ref antiAliasingChanged);
		updateCacheAndFlag<float>(ref antiAliasingStrengthCached, antiAliasingStrength, ref antiAliasingChanged);
		if (valuesCached.Count != values.Count) numRingsChanged = true; // If num rings change, need to update textures (performance intensive)
		updateCacheAndFlagList<float>(ref valuesCached, values, ref valuesChanged);
		updateCacheAndFlagList<string>(ref labelsCached, labels, ref labelsChanged);
		updateCacheAndFlagList<string>(ref ringIDsCached, ringIDs, ref ringIDsChanged);
		updateCacheAndFlag<float>(ref containerWidthCached, getSpriteWidth(this.gameObject), ref containerChanged);
		updateCacheAndFlag<float>(ref containerHeightCached, getSpriteHeight(this.gameObject), ref containerChanged);
	}

	void setCacheFlags(bool val) {
		radiusChanged = val;
		innerPercentageChanged = val;
		bandModeChanged = val;
		ringWidthChanged = val;
		labelLinePaddingChanged = val;
		paddingChanged = val;
		degreesChanged = val;
		minMaxChanged = val;
		bandColorChanged = val;
		ringColorChanged = val;
		antiAliasingChanged = val;
		valuesChanged = val;
		labelsChanged = val;
		ringIDsChanged = val;
		containerChanged = val;
		numRingsChanged = val;
	}

	void updateFromDataSource() {
		if (valuesDataSource != null) {
			values = valuesDataSource.getData<float>();
		}
		if (labelsDataSource != null) {
			labels = labelsDataSource.getData<string>();
		}
		if (ringIDsDataSource != null) {
			ringIDs = ringIDsDataSource.getData<string>();
		}
	}

	void updateContainer() {
		if (updateFromContainer) {
			if (paddingChanged || containerChanged) {
				outerRadius = Mathf.Min((getSpriteWidth(this.gameObject) - leftRightPadding.x - leftRightPadding.y)/2, (getSpriteHeight(this.gameObject) - topBotPadding.x - topBotPadding.y)/2);
			}
		}
	}

	void updateNumberRings() {
		if (numRingsChanged) {
			// Create rings based on values data
			for (int i = 0; i < values.Count; i++) {
				if (labels.Count <= i) labels.Add("Ring " + (i + 1));
				if (bandColors.Count <= i) bandColors.Add(bandColor);
				if (rings.Count <= i) {
					GameObject obj = GameObject.Instantiate(ringPrefab) as GameObject;
					changeSpriteParent(obj, ringsParent);
					WMG_Ring ring = obj.GetComponent<WMG_Ring>();
					ring.initialize(this);
					rings.Add(ring);
				}
			}
			for (int i = rings.Count - 1; i >= 0; i--) {
				if (rings[i] != null && i >= values.Count) {
					Destroy(rings[i].label);
					Destroy(rings[i].gameObject);
					rings.RemoveAt(i);
				}
			}
		}
	}

	void updateOuterRadius() {
		if (numRingsChanged || radiusChanged || labelLinePaddingChanged) {
			int newSize = Mathf.RoundToInt(outerRadius*2);
			// extra ring
			changeSpriteSize(extraRing, newSize, newSize);
			// rings and bands
			for (int i = 0; i < rings.Count; i++) {
				changeSpriteSize(rings[i].ring, newSize, newSize);
				changeSpriteSize(rings[i].band, newSize, newSize);
				changeSpriteHeight(rings[i].label, Mathf.RoundToInt(outerRadius + labelLinePadding));
			}
			// zero line
			changeSpriteHeight(zeroLine, Mathf.RoundToInt(outerRadius + labelLinePadding));
		}
	}

	void updateLabelsText() {
		if (labelsChanged) {
			for (int i = 0; i < rings.Count; i++) {
				changeLabelText(rings[i].labelText, labels[i]);
			}
		}
	}

	void updateInnerPercentage() {
		if (radiusChanged || innerPercentageChanged) {
			if (innerPercentageOfOuter) {
				// If changed to true set the percentage
				if (innerPercentageChanged) {
					origInnerPercentageOfOuter = innerRadius / outerRadius;
				}
				// update inner radius if outer radius changed
				if (radiusChanged) {
					innerRadius = origInnerPercentageOfOuter * outerRadius;
				}
			}
		}
	}

	void updateRingsAndBands() {
		if (numRingsChanged || radiusChanged || bandModeChanged || ringWidthChanged || antiAliasingChanged) {

			// extra ring
			if (bandMode) {
				SetActive(extraRing, true);
				float ringRadius = getRingRadius(rings.Count);
				updateRingColors(ref extraRingColors, ringRadius - ringWidth, ringRadius);
				extraRingSprite.texture.SetPixels(extraRingColors);
				extraRingSprite.texture.Apply();
			}
			else {
				SetActive(extraRing, false);
			}
			// rings and bands
			for (int i = 0; i < rings.Count; i++) {
				rings[i].updateRing(i);
			}
		}
	}

	public float getRingRadius(int index) {
		int numRingsToDivide = rings.Count - 1;
		if (bandMode) numRingsToDivide++;
		if (numRingsToDivide == 0) return outerRadius; // Only happens in non-band mode with only 1 ring
		float ringInterval = (outerRadius - innerRadius) / numRingsToDivide;
		return innerRadius + index * ringInterval;
	}

	void updateDegrees() {
		if (valuesChanged || degreesChanged || minMaxChanged) {
			Vector3 baseRotation = new Vector3 (0, 0, -degrees/2);
			float newFill = (360 - degrees) / 360f;
			// extra ring
			changeRadialSpriteRotation(extraRing, baseRotation);
			changeSpriteFill(extraRing, newFill);
			// rings and bands
			for (int i = 0; i < rings.Count; i++) {
				// rings
				changeRadialSpriteRotation(rings[i].ring, baseRotation);
				changeSpriteFill(rings[i].ring, newFill);
				// bands
				float valPercent = values[i] / (maxValue - minValue);
				changeRadialSpriteRotation(rings[i].band, baseRotation);
				changeSpriteFill(rings[i].band, 0);
				if (animateData) {
					WMG_Anim.animFill(rings[i].band, animDuration, animEaseType, newFill * valPercent);
				}
				else {
					changeSpriteFill(rings[i].band, newFill * valPercent);
				}
				// labels
				rings[i].label.transform.localEulerAngles = baseRotation;
				rings[i].labelBackground.transform.localEulerAngles = -baseRotation;
				changeSpritePositionTo(rings[i].labelBackground, new Vector3 (0, -15 -getSpriteHeight(rings[i].label), 0));
				Vector3 labelRotation = new Vector3(0, 0, -valPercent * (360 - degrees));
				if (animateData) {
					if (DOTween.IsTweening(rings[i].label.transform)) { // if already animating, then don't animate relative to current rotation
						WMG_Anim.animRotation(rings[i].label, animDuration, animEaseType, labelRotation + new Vector3(0,0,360) + baseRotation, false);
						WMG_Anim.animRotationCallbackC(rings[i].labelBackground, animDuration, animEaseType, -labelRotation - baseRotation, false, ()=> labelRotationCompleted(i));
					}
					else {
						WMG_Anim.animRotation(rings[i].label, animDuration, animEaseType, labelRotation, true);
						WMG_Anim.animRotationCallbackC(rings[i].labelBackground, animDuration, animEaseType, -labelRotation, true, ()=> labelRotationCompleted(i));
					}
				}
				else {
					rings[i].label.transform.localEulerAngles = labelRotation + baseRotation;
					rings[i].labelBackground.transform.localEulerAngles = -labelRotation - baseRotation;
					if (i == rings.Count - 1) repositionLabels();
				}
			}
			// zero line
			zeroLine.transform.localEulerAngles = baseRotation;
			zeroLineText.transform.localEulerAngles = -baseRotation;
		}
	}

	void labelRotationCompleted(int ringNum) {
		if (ringNum == rings.Count - 1) {
			if (!DOTween.IsTweening(rings[ringNum].label.transform)) { // if already animating, then don't reposition labels
				repositionLabels();
			}
		}
	}

	void repositionLabels() {
		if (!autostackLabels) return;
		List<int> sortedRingIndices = getRingsSortedByValue();
		List<List<int>> overlappingGroups = new List<List<int>>();
		List<List<Vector2>> positionDifferences = new List<List<Vector2>>();
		bool createNewGroup = true;
		// Determine which ring labels are overlapping and by how much they are overlapping
		for (int i = 0; i < rings.Count - 1; i++) {
			GameObject curLabel = rings[sortedRingIndices[i]].labelBackground;
			GameObject nxtLabel = rings[sortedRingIndices[i+1]].labelBackground;
			Vector3 curLabelPos = getPositionRelativeTransform(curLabel, ringsParent);
			Vector2 curLabelSize = getSpriteSize(curLabel);
			Vector3 nxtLabelPos = getPositionRelativeTransform(nxtLabel, ringsParent);
			Vector2 nxtLabelSize = getSpriteSize(nxtLabel);
			Vector2 positionDifference = new Vector2 (nxtLabelPos.x - curLabelPos.x, nxtLabelPos.y - curLabelPos.y);
			if (Mathf.Abs(positionDifference.x) - (curLabelSize.x / 2 + nxtLabelSize.x / 2) < 0 && Mathf.Abs(positionDifference.y)  - curLabelSize.y < 0) { // Labels overlap
				if (createNewGroup) { // If create new group then add both current and next ring to the list of overlapping rings
					createNewGroup = false;
					List<int> ringGroup = new List<int>();
					ringGroup.Add(sortedRingIndices[i]);
					ringGroup.Add(sortedRingIndices[i+1]);
					overlappingGroups.Add(ringGroup);
					List<Vector2> positionDifferenceGroup = new List<Vector2>();
					positionDifferenceGroup.Add(positionDifference);
					positionDifferences.Add(positionDifferenceGroup);
				}
				else { // If we are already in an overlapping group then just add the next ring
					overlappingGroups[overlappingGroups.Count-1].Add(sortedRingIndices[i+1]);
					positionDifferences[positionDifferences.Count-1].Add(positionDifference);
				}
			}
			else { // labels did not overlap
				if (!createNewGroup) { // previous ring overlapped, but this ring did not, so create new group next time
					createNewGroup = true;
				}
			}
		}
		// Move labels so that they stack on top of eachother 
		for (int i = 0; i < overlappingGroups.Count; i++) {
			Vector2 offsetAll = Vector2.zero;
			float rotFirst = rings[overlappingGroups[i][0]].label.transform.localEulerAngles.z;
			rotFirst = Mathf.Abs(rotFirst - 180);
			float rotLast = rings[overlappingGroups[i][overlappingGroups[i].Count-1]].label.transform.localEulerAngles.z;
			rotLast = Mathf.Abs(rotLast - 180);
			// If the labels are near the top or bottom of the graph then do not vertically stack
			if (!(rotFirst > 90 - autostasckLabelsDegrees && rotFirst < 90 + autostasckLabelsDegrees && 
			      rotLast > 90 - autostasckLabelsDegrees && rotLast < 90 + autostasckLabelsDegrees)) continue;
			for (int j = 0; j < overlappingGroups[i].Count; j++) {
				GameObject curLabel = rings[overlappingGroups[i][j]].labelBackground;
				if (j < overlappingGroups[i].Count - 1) {
					Vector2 cumulativeDifference = Vector2.zero;
					int cumulativeNum = 0;
					for (int k = j; k < overlappingGroups[i].Count - 1; k++) {
						cumulativeDifference += positionDifferences[i][k];
						cumulativeNum++;
					}
					float heightOffset = cumulativeNum * getSpriteHeight(curLabel);
					if (cumulativeDifference.y > 0) heightOffset *= -1;
					if (j == 0) offsetAll = new Vector2(cumulativeDifference.x / 2,  (cumulativeDifference.y + heightOffset) / 2);
					changePositionByRelativeTransform(curLabel, ringsParent, 
					                                  new Vector2(cumulativeDifference.x, cumulativeDifference.y + heightOffset) - offsetAll);
				}
				else {
					changePositionByRelativeTransform(curLabel, ringsParent, -offsetAll);
				}
			}
		}
	}

	public List<int> getRingsSortedByValue() {
		List<float> newVals = new List<float>(values);
		newVals.Sort();
		List<int> ringIndices = new List<int>();
		for (int i = 0; i < newVals.Count; i++) {
			for (int j = 0; j < values.Count; j++) {
				if (Mathf.Approximately(values[j], newVals[i])) {
					ringIndices.Add(j);
					break;
				}
			}
		}
		return ringIndices;
	}

	void updateRingColors() {
		if (numRingsChanged || ringColorChanged) {
			changeSpriteColor(extraRing, ringColor);
			for (int i = 0; i < rings.Count; i++) {
				changeSpriteColor(rings[i].ring, ringColor);
			}
		}
	}

	void updateBandColors() {
		if (numRingsChanged || bandColorChanged) {
			for (int i = 0; i < rings.Count; i++) {
				if (autoUpdateBandAlpha) {
					bandColors[i] = new Color(bandColors[i].r, bandColors[i].g, bandColors[i].b, (i + 1f) / rings.Count);
				}
				changeSpriteColor(rings[i].band, bandColors[i]);
			}
		}
	}

	void updateBackground() {
		if (paddingChanged) {
			changeSpriteSize(background,Mathf.RoundToInt(outerRadius*2 + leftRightPadding.x + leftRightPadding.y), 
			                 			Mathf.RoundToInt(outerRadius*2 + topBotPadding.x + topBotPadding.y));
			Vector2 offset = new Vector2(-leftRightPadding.x/2f + leftRightPadding.y/2f, topBotPadding.x/2f - topBotPadding.y/2f);
			changeSpritePositionTo(background, new Vector3(offset.x, offset.y));
		}
	}



	public void updateRingColors(ref Color[] colors, float inner, float outer) {
		int size = Mathf.RoundToInt(Mathf.Sqrt(colors.Length));
		float texFactor = (outerRadius*2) / size;
		inner = inner / texFactor;
		outer = outer / texFactor;
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				int centerX = i - size / 2;
				int centerY = j - size / 2;
				float dist = Mathf.Sqrt(centerX * centerX + centerY * centerY);
				if (dist >= inner && dist < outer) {
					if (antiAliasing) {
						if (dist >= inner + antiAliasingStrength && dist < outer - antiAliasingStrength) {
							colors[i + size * j] = new Color(1, 1, 1, 1);
						}
						else {
							if (dist > inner + antiAliasingStrength) {
								colors[i + size * j] = new Color(1, 1, 1, (outer - dist) / antiAliasingStrength);
							}
							else {
								colors[i + size * j] = new Color(1, 1, 1, (dist - inner) / antiAliasingStrength);
							}
						}
					}
					else {
						colors[i + size * j] = new Color(1, 1, 1, 1);
					}
				}
				else {
					colors[i + size * j] = new Color(1, 1, 1, 0);
				}
			}
		}
	}

	public WMG_Ring getRing(string id) {
		for (int i = 0; i < ringIDs.Count; i++) {
			if (id == ringIDs[i]) return rings[i];
		}
		Debug.LogError("No ring found with id: " + id);
		return null;
	}

	public void HighlightRing(string id) {
		for (int i = 0; i < rings.Count; i++) {
			changeSpriteColor(rings[i].band, new Color(bandColor.r, bandColor.g, bandColor.b, 0));
		}
		changeSpriteColor(getRing(id).band, new Color(bandColor.r, bandColor.g, bandColor.b, 1));
	}

	public void RemoveHighlights() {
		bandColorChanged = true;
	}

}
