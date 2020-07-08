using DG.Tweening;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandle : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // load scene
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public virtual void InitUIValue()
    {
       
    }

    //open UI
    public virtual void OpenUIAnimation(GameObject UI)
    {
        UIAnimator temp = UI.GetComponent<UIAnimator>();
    
        UI.SetActive(true);
        temp?.OpenUIAnimation();
        
    }


    // close UI
    public virtual void CloseUIAnimation(GameObject UI)
    {
        UIAnimator temp = UI.GetComponent<UIAnimator>();
        if (temp == null)   // no animation to be played
        {
            UI.SetActive(false);
        }
        else
        {
            temp.CloseUIAnimation();
        }
    }


    //enable buttons in UI
    public virtual void EnableUIButton(GameObject UI)
    {
        Button[] UIButtons = UI.GetComponentsInChildren<Button>();

        foreach (Button button in UIButtons)
        {
            button.enabled = true;
        }
        Debug.Log("@UIHandle: "+ UI.name + " enabled");
    }


    //disable buttons in UI
    public virtual void DisableUIButton(GameObject UI)
    {
        Button[] UIButtons = UI.GetComponentsInChildren<Button>();

        foreach (Button button in UIButtons)
        {
            button.enabled = false;
        }
        Debug.Log("@UIHandle: " + UI.name + " disabled");
    }


    // display CanvaUI in ShowTime
    public virtual void ShowCanvaUI(GameObject CanvaUI, float ShowTime)
    {
        CanvaUI.SetActive(true);
        Sequence AnimationSequence = DOTween.Sequence();

        CanvasGroup CanvasGroup = CanvaUI.GetComponent<CanvasGroup>();

        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = true;


        Tweener tweener2 = CanvasGroup.DOFade(1, ShowTime);
        Tweener tweener3 = transform.DOScale(1, ShowTime);

        tweener2.SetEase(Ease.OutQuad);
        tweener3.SetEase(Ease.OutQuad);


        // 并行播放
        AnimationSequence.Insert(0f, tweener2);          //变深
        AnimationSequence.Insert(0f, tweener3);         //由小变大
    }

    // disactive CanvaUI in DisappearTime
    public virtual void CloseCanvaUI(GameObject CanvaUI, float DisappearTime)
    {
        Sequence AnimationSequence = DOTween.Sequence();

        CanvasGroup CanvasGroup = CanvaUI.GetComponent<CanvasGroup>();

        CanvasGroup.alpha = 1;
        CanvasGroup.blocksRaycasts = true;


        Tweener tweener2 = CanvasGroup.DOFade(0, DisappearTime);
        Tweener tweener3 = transform.DOScale(0, DisappearTime);

        tweener2.SetEase(Ease.OutQuad);
        tweener3.SetEase(Ease.OutQuad);


        // 并行播放
        AnimationSequence.Insert(0f, tweener2);          //变深
        AnimationSequence.Insert(0f, tweener3);         //由大变小
        // 时间到后关闭CanvaUI
        StartCoroutine(Disactive(CanvaUI, DisappearTime));
    }

    // disactive Go
    private IEnumerator Disactive(GameObject Go, float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime); //先直接返回，之后的代码等待给定的时间周期过完后执行
        Go.SetActive(false);
    }

    private IEnumerator Active(GameObject Go, float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime); //先直接返回，之后的代码等待给定的时间周期过完后执行
        Go.SetActive(true);
    }

}
