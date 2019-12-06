using UnityEngine;
using System;
using DG.Tweening;
using DG.Tweening.Core;

// Contains GUI system dependent functions

public static class WMG_Anim {

	public static void animFill(GameObject obj, float duration, Ease easeType, float animTo) {
		UIBasicSprite comp = obj.GetComponent<UIBasicSprite>();
		DOTween.To(()=> comp.fillAmount, x=> comp.fillAmount = x, animTo, duration).SetEase(easeType).SetUpdate(false);
	}
	
	public static void animRotation(GameObject obj, float duration, Ease easeType, Vector3 animTo, bool relative) {
		obj.transform.DOLocalRotate(animTo, duration, RotateMode.FastBeyond360).SetEase(easeType).SetUpdate(false).SetRelative(relative);
	}
	
	public static void animRotationCallbackC(GameObject obj, float duration, Ease easeType, Vector3 animTo, bool relative, TweenCallback onComp) {
		obj.transform.DOLocalRotate(animTo, duration, RotateMode.FastBeyond360).SetEase(easeType).SetUpdate(false).SetRelative(relative)
			.OnComplete(onComp);
	}
	
	public static void animPositionCallbackC(GameObject obj, float duration, Ease easeType, Vector3 animTo, TweenCallback onComp) { 
		DOTween.To(()=> obj.transform.localPosition, x=> obj.transform.localPosition = x, animTo, duration).SetEase(easeType).SetUpdate(false)
			.OnComplete(onComp);
	}
	
	public static void animPosition(GameObject obj, float duration, Ease easeType, Vector3 animTo) {
		DOTween.To(()=> obj.transform.localPosition, x=> obj.transform.localPosition = x, animTo, duration).SetEase(easeType).SetUpdate(false);
	}

	public static void animSize(GameObject obj, float duration, Ease easeType, Vector2 animTo) {
		UIWidget comp = obj.GetComponent<UIWidget>();
		DOTween.To(()=> comp.width, x=> comp.width = x, Mathf.RoundToInt(animTo.x), duration).SetEase(easeType).SetUpdate(false);
		DOTween.To(()=> comp.height, x=> comp.height = x, Mathf.RoundToInt(animTo.y), duration).SetEase(easeType).SetUpdate(false);
	}

	public static void animPositionCallbacks(GameObject obj, float duration, Ease easeType, Vector3 animTo, TweenCallback onUpd, TweenCallback onComp) {
		DOTween.To(()=> obj.transform.localPosition, x=> obj.transform.localPosition = x, animTo, duration).SetEase(easeType).SetUpdate(false)
			.OnUpdate(onUpd).OnComplete(onComp);
	}
	
	public static void animScale(GameObject obj, float duration, Ease easeType, Vector3 animTo, float delay) {
		DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration).SetEase(easeType).SetUpdate(false).SetDelay(delay);
	}
	
	public static void animScaleCallbackC(GameObject obj, float duration, Ease easeType, Vector3 animTo, TweenCallback onComp) {
		DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration).SetEase(easeType).SetUpdate(false)
			.OnComplete(onComp);
	}
	
	public static void animScaleSeqInsert(ref Sequence seq, float insTime, GameObject obj, float duration, Ease easeType, Vector3 animTo, float delay) {
		seq.Insert(insTime,
		           DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration + delay).SetEase(easeType).SetUpdate(false).SetDelay(delay)
		           );
	}
	
	public static void animScaleSeqAppend(ref Sequence seq, GameObject obj, float duration, Ease easeType, Vector3 animTo, float delay) {
		seq.Append(
			DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration + delay).SetEase(easeType).SetUpdate(false).SetDelay(delay)
			);
	}
	
	public static void animInt(DOGetter<int> getter, DOSetter<int> setter, float duration, int animTo) {
		DOTween.To(getter, setter, animTo, duration).SetUpdate(false);
	}
	
	public static void animVec2(DOGetter<Vector2> getter, DOSetter<Vector2> setter, float duration, Vector2 animTo) {
		DOTween.To(getter, setter, animTo, duration).SetUpdate(false);
	}
}
