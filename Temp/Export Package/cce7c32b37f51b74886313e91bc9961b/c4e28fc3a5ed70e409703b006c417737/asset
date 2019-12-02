using UnityEngine;
using System.Collections;

public static class WMG_Util {

	public static Sprite createSprite(Texture2D tex) {
		Texture2D newTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
		newTex.filterMode = FilterMode.Bilinear;
		newTex.LoadImage(tex.EncodeToPNG());
		return Sprite.Create(newTex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
	}

}
