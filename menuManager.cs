using UnityEngine;

// Deals with updating highscore and average score on "MainMenu" scene
public class menuManager : MonoBehaviour {

    // Text/UI variables that are displayed during gameplay 
    [Header("Text/UI")]
    public updateScore highScoreBoard;
    public updateScore averageScoreBoard;

    // At start of scene, before gameplay has begun, update high and average scores
    void Start() {
        // Update highscore text
        highScoreBoard.updateText("Highscore: " + PlayerPrefs.GetInt("highScore").ToString());

        // Calculate average score
        float averageScore = 0;
        if (PlayerPrefs.GetInt("gamesPlayed") > 0) {
            averageScore = ((float)PlayerPrefs.GetInt("totalScore") / (float)PlayerPrefs.GetInt("gamesPlayed"));
            averageScore = Mathf.Round(averageScore);
        }
        // Update average score text
        averageScoreBoard.updateText("Average Score: " + averageScore);
    }
}
