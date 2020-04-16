using UnityEngine;
using DG.Tweening;
using System.Collections;

// 使用DoTween库生成动画
public class DoTweenUIAnimation : UIAnimator {
    private float _OpenTime = 1.0f;
    private float _CloseTime = 1.0f;


    void Start()
    {
        //OpenUIAnimation();
    }

    // 缓进方式打开UI界面
    public override void OpenUIAnimation()
    {
        // animation sequence
        Sequence AnimationSequence = DOTween.Sequence();

        CanvasGroup CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();

        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = true;

        Vector3 temp = transform.localPosition;
        temp.x = 1000;
        transform.localPosition = temp;
        transform.localScale = Vector3.zero;

        Tweener tweener1 = transform.DOLocalMoveX(0, _OpenTime);
        Tweener tweener2 = CanvasGroup.DOFade(1, _OpenTime);
        Tweener tweener3 = transform.DOScale(1, _OpenTime);

        tweener1.SetEase(Ease.OutQuad);
        tweener2.SetEase(Ease.OutQuad);
        tweener3.SetEase(Ease.OutQuad);


        // 并行播放
        AnimationSequence.Insert(0f, tweener1); //移动
        AnimationSequence.Insert(0f, tweener2);          //变深
        AnimationSequence.Insert(0f, tweener3);         //由小变大

    }

    // close UI Animation
    public override void CloseUIAnimation()
    {
        // animation sequence
        Sequence AnimationSequence = DOTween.Sequence();

        CanvasGroup CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();

        Tweener tweener1 = transform.DOLocalMoveX(-1000, _CloseTime);
        Tweener tweener2 = CanvasGroup.DOFade(0, _CloseTime);
        Tweener tweener3 = transform.DOScale(0, _CloseTime);

        //缓进函数
        tweener1.SetEase(Ease.InQuad);
        tweener2.SetEase(Ease.InQuad);
        tweener3.SetEase(Ease.InQuad);
        // 并行播放
        AnimationSequence.Insert(0f, tweener1); //移动
        AnimationSequence.Insert(0f, tweener2); //变淡
        AnimationSequence.Insert(0f, tweener3); //由大变小

        StartCoroutine(Disactive());
    }

    // set ui disactive after "_CloseTime" seconds
    private IEnumerator Disactive()
    {
        yield return new WaitForSeconds(this._CloseTime); //先直接返回，之后的代码等待给定的时间周期过完后执行
        this.gameObject.SetActive(false);
    }

}
