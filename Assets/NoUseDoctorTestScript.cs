using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class NoUseDoctorTestScript : MonoBehaviour {

	public Text Tips;
	public Sequence seq;

    private IEnumerator StartLoading_4()
    {
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync("08-WallEvaluation");
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }
        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetLoadingPercentage(displayProgress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }

    void SetLoadingPercentage(float value)
    {
        Tips.text = "场景加载\n\n" + value.ToString() + "%";
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(StartLoading_4());

    }




    // Update is called once per frame
    void Update () {
		
	}
}
