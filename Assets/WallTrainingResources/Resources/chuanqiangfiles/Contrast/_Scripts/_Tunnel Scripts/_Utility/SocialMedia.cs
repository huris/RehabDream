using UnityEngine;
using System.Collections;

/// <summary>
/// Used to open a Facebook or Twitter profile page.
/// </summary>
public class SocialMedia : MonoBehaviour 
{
    [Header("Insert your Facebook Information Here.")]
	public string facebookAppID = "fb://profile/325928954230541";
	public string facebookProfile = "https://www.facebook.com/dotexeinteractive";

    [Header("Insert your Twitter Information Here")]
    public string twitterAppUserURL = "twitter:///user?screen_name=Unity3D";
    public string twitterProfile = "https://www.facebook.com/dotexeinteractive";

    /// <summary>
    /// Open a Facebook page.
    /// </summary>
	public void OpenFacebookPage()
	{
        //Get a starting time.
		float startTime;
		startTime = Time.timeSinceLevelLoad;

		//Attempt to open the Facebook app at the provided profile.
		Application.OpenURL(facebookAppID);

        //If for whatever reason the Facebook app doesn't open, open the profile in the web browser.
		if (Time.timeSinceLevelLoad - startTime <= 1f)
		{
			//fail. Open safari.
			Application.OpenURL(facebookProfile);
		}
	}

    public void OpenTwitterPage()
    {
        //Get a starting time.
        float startTime;
        startTime = Time.timeSinceLevelLoad;

        //Attempt to open the Facebook app at the provided profile.
        Application.OpenURL(twitterAppUserURL);

        //If for whatever reason the Facebook app doesn't open, open the profile in the web browser.
        if (Time.timeSinceLevelLoad - startTime <= 1f)
        {
            //fail. Open safari.
            Application.OpenURL(twitterProfile);
        }
    }
}
