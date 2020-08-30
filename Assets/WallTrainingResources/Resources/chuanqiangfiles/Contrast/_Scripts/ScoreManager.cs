using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Score, best score and last score management.
/// </summary>
public class ScoreManager : MonoBehaviour {

    //Should the score be calculated?
    public static bool calculateScore;

    //The most recently set score.
    int lastScore = 0;

    //The best score that has been achieved.
    int bestScore = 0;

    //The score achieved so far.
    private static int score = 0;
    int scoreForUI;

    //The text objects responsible for displaying scores.
    private Text scoreText;
    private Text lastText;
    private Text bestText;

    void Awake()
    {
        //Get the Text objects to display scores.
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        lastText = GameObject.Find("LastScoreText").GetComponent<Text>();
        bestText = GameObject.Find("BestScoreText").GetComponent<Text>();

        //Get the best score.
        bestScore = PlayerPrefs.GetInt("BestScore");

        //Populate the Best and last score text objects.
        lastText.text = "Last: " + lastScore.ToString();
        bestText.text = "Best: " + bestScore.ToString();
    }

    /// <summary>
    /// Update the score.
    /// </summary>
    public static void UpdateScore()
    {
        if (calculateScore) {

            //Increase the score.
            score++;
        }
    }

    void Update()
    {
        //If the score does not match the score we are displaying on the UI, Update it.
        if (score != scoreForUI)
        {
            scoreText.text = score.ToString();
            scoreForUI++;
        }
    }

    /// <summary>
    /// Populate the best and last score Text objects.
    /// </summary>
    public void GetScores()
    {
        bestScore = PlayerPrefs.GetInt("BestScore");

        lastText.text = "Last: " + lastScore.ToString();
        bestText.text = "Best: " + bestScore.ToString();
    }

    /// <summary>
    /// Finalise and clean up scores upon game over.
    /// </summary>
    public void FinaliseScores()
    {
        //Set the lastScore variable.
        lastScore = score;
        
        //Check to see if we have a new best score.
        if (lastScore > bestScore)
        {
            //if so, set it in the PlayerPrefs.
            PlayerPrefs.SetInt("BestScore", lastScore);
        }

        //Reset the score ready for the next game.
        score = 0;

        //Reset the score text.
        scoreText.text = "0";
        
    }
}
