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

}
