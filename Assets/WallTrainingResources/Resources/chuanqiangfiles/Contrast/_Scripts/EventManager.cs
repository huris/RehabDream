using UnityEngine;
using System.Collections;

/// <summary>
/// Used for all common game and UI events.
/// </summary>
public class EventManager : MonoBehaviour {

    [HideInInspector]
    //The PlayerController script.
    public PlayerController playerController;

    [HideInInspector]
    public ScoreManager scoreManager;

    //The speed at which UI items will be faded in and out.
    private float fadeSpeed = 0.6f;

    //The various UI Canvas Groups.
    private CanvasGroup mainMenuCanvasGroup;
    private CanvasGroup inGameCanvasGroup;
    private CanvasGroup gameOverFlash;

    //The script that changes the colour of the threats.
    Colour colour;

    //The total number of games played so far.
    public int gameNumber = 0;

    void Awake()
    {
        //Cache the CanvasGroups so that they can be faded in and out.
        mainMenuCanvasGroup = GameObject.Find("Main Menu UI").GetComponent<CanvasGroup>();
        inGameCanvasGroup = GameObject.Find("In Game UI").GetComponent<CanvasGroup>();
        gameOverFlash = GameObject.Find("Game Over Flash").GetComponent<CanvasGroup>();

        //Cache the colour script.
        colour = GameObject.FindObjectOfType<Colour>();

        ShowMainMenu();
    }

    /// <summary>
    /// Called when play is ready to commence.
    /// </summary>
    public void StartGame()
    {
        //Begin the creation of threats.
        TunnelManager.generateThreats = true;

        //Begin to calculate score.
        ScoreManager.calculateScore = true;

        //Generate a new threat colour for this game.
        colour.GenerateColour();

        //Hide the main menu.
        HideMainMenu();
        ShowInGameUI();

        //Get the best and last scores from the Score Manager.
        scoreManager.GetScores();

        //Accept input on the playerController.
        playerController.acceptInput = true;
    }

    /// <summary>
    /// Called when the game is ready to be ended.
    /// </summary>
    public void GameOver()
    {
        //Cease the creation of threats.
        TunnelManager.generateThreats = false;
        ScoreManager.calculateScore = false;

        //Do not accept input on the player controller.
        playerController.acceptInput = false;

        //Flash the Game Over Canvas Group so that the tunnel can be reset discretely.
        //The tunnel will be reset at the mid section of the flash from within the GameOverFlash Coroutine.
        StartCoroutine(GameOverCleanUp(gameOverFlash, fadeSpeed/2));
    }

    /// <summary>
    /// Hide the main menu once a game has started.
    /// </summary>
    void HideMainMenu()
    {
        StartCoroutine(FadeOut(mainMenuCanvasGroup));
    }

    ///<summary/>
    ///Show the main menu.
    ///</summary>
    void ShowMainMenu()
    {
        StartCoroutine(FadeIn(mainMenuCanvasGroup, fadeSpeed));
    }

    /// <summary>
    /// Hide the in game UI.
    /// </summary>
    void HideInGameUI()
    {
        StartCoroutine(FadeOut(inGameCanvasGroup));
    }

    /// <summary>
    /// Show the in game UI.
    /// </summary>
    void ShowInGameUI()
    {
        StartCoroutine(FadeIn(inGameCanvasGroup, fadeSpeed));
    }

    /// <summary>
    /// Fade out the provided CanvasGroup after delay(optional).
    /// </summary>
    IEnumerator FadeOut(CanvasGroup group, float delay = 0f)
    {
        //We are hiding the canvas group, so there is no need for it to be interactable.
        group.interactable = false;

        //If we have provided a delay, wait.
        if (delay != 0) { yield return new WaitForSeconds(delay); }

            float time = fadeSpeed;
            while (group.alpha > 0)
            {
                group.alpha -= Time.deltaTime / time;
                yield return null;
            }
    }

    /// <summary>
    /// Fade in the provided CanvasGroup after delay(optional).
    /// </summary>
    IEnumerator FadeIn(CanvasGroup group, float delay = 0f)
    {
        //If we have provided a delay, wait.
        if (delay != 0) { yield return new WaitForSeconds(delay); }

        float time = fadeSpeed;
        while (group.alpha < 1)
        {
            group.alpha += Time.deltaTime / time;
            yield return null;
        }

        //Once the Canvas Group has faded back in, enable interaction for it.
        group.interactable = true;
    }

    /// <summary>
    ///Clean up after the game has ended and get it ready to be started again.
    /// </summary>
    IEnumerator GameOverCleanUp(CanvasGroup group, float delay = 0f)
    {
        //If we have provided a delay, wait.
        if (delay != 0) { yield return new WaitForSeconds(delay); }

        //Begin hiding the in game UI.
        HideInGameUI();

        //Fade in the CanvasGroup.
        float time = fadeSpeed;
        while (group.alpha < 1)
        {
            group.alpha += Time.deltaTime / time;
            yield return null;
        }

        //Clear the remaining threats ahead of the player.
        TunnelManager.ClearRemainingThreats();

        //Check for a new high score and reset the current score to 0.
        scoreManager.FinaliseScores();

        //Fade back out.
        while (group.alpha > 0)
        {
            group.alpha -= Time.deltaTime / time;
            yield return null;
        }

        //Show the main menu.
        ShowMainMenu();

        //Increase the game number by 1.
        gameNumber++;
    }
}
