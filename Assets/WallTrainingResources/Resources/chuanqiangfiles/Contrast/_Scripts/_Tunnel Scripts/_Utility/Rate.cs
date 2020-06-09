using UnityEngine;
using System.Collections;

/// <summary>
/// Used to open a market store url.
/// </summary>
public class Rate : MonoBehaviour 
{
    [Header("Insert your app store URLs here.")]
	public string iosAppURL = "fb://profile/325928954230541";
	public string androidAppURL = "https://www.facebook.com/dotexeinteractive";

	void OnEnable()
	{
        //If we are not running this application on a mobile platform, we have no need for the rate button.
        if (!Application.isMobilePlatform) { GameObject.Find("Rate").SetActive(false); }
	}

    /// <summary>
    /// Open the market on the a given platform at the specified url.
    /// </summary>
	public void OpenAppPage()
	{
		string marketURL = "";

		#if UNITY_IOS
		    marketURL = iosAppURL;
		#else
			marketURL = androidAppURL;
		#endif

        //Open the specified market URL.
		Application.OpenURL(marketURL);

	}
}
