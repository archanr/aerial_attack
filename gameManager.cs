using System.Collections;
using UnityEngine;

// Serves as the game manager for "Singleplayer" scene
// Handles scoring, interactions with other gameobjects/scripts, sounds, and game logic
public class gameManager : MonoBehaviour {

    // Miscellaneous variables used throughout for game logic 
    [Header("Attributes")]
    private int gameScore = 0;
    private int numberOfBounces = 0;
    private float powerUpLength = 5f;
    private bool doublePointsActive = false;
    private bool initialBallDrop = false;

    // References to other scripts, used for powerup effects 
    [Header("Scripts")]
    public ball ballScript;
    public dragCircle racketScript;

    // Text/UI variables that are displayed during gameplay 
    [Header("Text/UI")]
    public updateScore scoreBoard;
    public updateScore finalBoard;
    public GameObject gameOverUI;
    public GameObject pauseMenuUI;
    public Healthbar powerUpBarUI;
    public cameraShake shakeEffect;
    public GameObject board;
    public GameObject doubleRacket;
    public bubbleBoardEffect bubble;
    public GameObject pauseButton;

    // Prefab variables which contain powerup gameobjects
    [Header("Prefabs")]
    public GameObject slowBallPrefab;
    public GameObject enlargeRacketPrefab;
    public GameObject doublePointsPrefab;
    public GameObject doubleRacketPrefab;

    // AudioSource variables which contain reference to specific audio files within the game
    [Header("Audio")]
    public AudioSource[] sounds;
    public AudioSource pongHit1;
    public AudioSource pongHit2;
    public AudioSource gameOver;
    public AudioSource slowBall;
    public AudioSource enlargeRacket;
    public AudioSource doublePoints;
    public AudioSource doubleRackets;
    public AudioSource powerOver;
    public AudioSource countDown;
    public AudioSource select;
    public AudioSource gamePlayMusic;

    // At start of scene, before gameplay has begun, initalize all audio sources
    // Returns:
    //  Void
    void Start() {
        sounds = GetComponents<AudioSource>();
        pongHit1 = sounds[0];
        pongHit2 = sounds[1];
        gameOver = sounds[2];
        slowBall = sounds[3];
        enlargeRacket = sounds[4];
        doublePoints = sounds[5];
        doubleRackets = sounds[6];
        powerOver = sounds[7];
        countDown = sounds[8];
        select = sounds[9];
        gamePlayMusic = sounds[10];
    }

    // Once gameplay has begun, invoke ball drop function from ball script
    // Returns:
    //  Void
    void Update() {
        if (initialBallDrop == false) {
            ballScript.dropBall();
            initialBallDrop = true;
        }
    }

    // Handles game over when ball falls out of bounds
    // Clears powerups from gameboard, saves new game statistics, and shows "gameOverUI"
    // Returns:
    //  Void
    public void initiateGameOver() {
        gamePlayMusic.Pause();
        gameOver.Play();
        shakeEffect.ShakeIt();
        removeAllPowerups();
        
        StartCoroutine(waitForWhile());

        // Update 'final score' board then display game over screen
        finalBoard.updateText("Score: " + gameScore.ToString());

        // If a new highscore, set and save to device
        if (gameScore > PlayerPrefs.GetInt("highScore")) {
            PlayerPrefs.SetInt("highScore", gameScore);
        }

        // Set and save total score to device
        PlayerPrefs.SetInt("totalScore", PlayerPrefs.GetInt("totalScore") + gameScore);

        // Set and save total game count to device
        PlayerPrefs.SetInt("gamesPlayed", PlayerPrefs.GetInt("gamesPlayed") + 1); 

        return;
    }

    // Helper coroutine for "initiateGameOver"
    // Sets 1.5 second delay before showing "gameOverUI" and hiding irrelevant gameobjects
    IEnumerator waitForWhile() {
        yield return new WaitForSeconds(1.5f);
        turnOffPowerUpBar();
        gameOverUI.SetActive(true);
        board.SetActive(false);
    }

    // Remove all power up gameobjects instatiated on board
    // Returns:
    //  Void
    public void removeAllPowerups() {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (var item in powerups) {
            Destroy(item);
        }
    }

    // Increases the gamescore text element on board
    // Every tenth bounce it will call to spawn a random powerup and increase ball speed by 5%
    // Returns:
    //  Void
    public void updateExistingScore() {
        // Alternate ping pong sound effects
        if (numberOfBounces % 2 == 0) {
            pongHit1.Play();
        } else {
            pongHit2.Play();
        }
        numberOfBounces++;

        // If double points powerup active, add extra point
        if (doublePointsActive) {
            gameScore += 2;
        } else {
            gameScore++;
        }

        // Start bubble effect for board
        bubble.bubbleEffect();
        scoreBoard.updateText(gameScore.ToString());

        // Instantiate a powerup every 10 bounces and increase ball speed by 5%
        if (numberOfBounces % 10 == 0 && numberOfBounces != 0) {
            spawnPowerUp();
            ballScript.changeSpeed(1.05f);
        }
        return;
    }

    // Pick random powerup to create, then spawn it onto gameboard
    // Returns:
    //  Void
    public void spawnPowerUp() {
        GameObject powerUpObject = null;
        int randomInt = Random.Range(0, 4);

        if (randomInt == 0) {
            powerUpObject = Instantiate(slowBallPrefab) as GameObject;
        } else if (randomInt == 1) {
            powerUpObject = Instantiate(enlargeRacketPrefab) as GameObject;
        } else if (randomInt == 2) {
            powerUpObject = Instantiate(doublePointsPrefab) as GameObject;
        } else if (randomInt == 3) {
            powerUpObject = Instantiate(doubleRacketPrefab) as GameObject;
        }

        powerUpObject.transform.position = this.transform.position;
    }

    // Enables various powerups within the game
    // Parameters:
    //  powerUpType - type of powerup
    //      slowBall            -  Slows down ball speed by 50%
    //      enlargeRacket       -  Racket size increases
    //      doublePoints        -  Each bounce gives extra point
    //      doubleRacket        -  Activate racket on opposite side
    // Returns:
    //  Void
    public void powerUp(string powerUpType) {
        startSlidePowerUpBar();
        if (powerUpType == "slowBall") {
            ballScript.changeSpeed(0.5f);
            slowBall.Play();
        } else if (powerUpType == "enlargeRacket") {
            racketScript.changeSize(50f);
            enlargeRacket.Play();
        } else if (powerUpType == "doublePoints") {
            doublePointsActive = true;
            doublePoints.Play();
        } else if (powerUpType == "doubleRacket") {
            doubleRacket.SetActive(true);
            doubleRackets.Play();
        }

        StartCoroutine(powerDown(powerUpType));
        return;
    }

    // Helper coroutine for "powerUp"
    // Waits "powerUpLength" time, then reverts powerup effects that were set in "powerUp" function
    public IEnumerator powerDown(string powerUpType) {
        yield return new WaitForSeconds(powerUpLength);
        if (powerUpType == "slowBall") {
            ballScript.changeSpeed(2f);
        } else if (powerUpType == "enlargeRacket") {
            racketScript.changeSize(35f);
        } else if (powerUpType == "doublePoints") {
            doublePointsActive = false;
        } else if (powerUpType == "doubleRacket") {
            doubleRacket.SetActive(false);
        }

        turnOffPowerUpBar();
    }

    // When powerup has been activated, enable powerup bar slider and begin countdown
    // Returns:
    //  Void
    public void startSlidePowerUpBar() {
        turnOnPowerUpBar();
        powerUpBarUI.SetHealth(100f);
        powerUpBarUI.healthPerSecond = 20f;
    }

    // Turns power up bar visibility on if it has been turned off
    // Returns:
    //  Void
    public void turnOnPowerUpBar() {
        if (powerUpBarUI.gameObject.activeSelf == false) {
            powerUpBarUI.gameObject.SetActive(true);
        }
    }

    // Turns power up bar visibility off if it has been turned on
    // Returns:
    //  Void
    public void turnOffPowerUpBar() {
        if (powerUpBarUI.gameObject.activeSelf == true) {
            powerUpBarUI.gameObject.SetActive(false);
        }
    }

    // When pause button triggered, freezes the game and displays "pauseMenuUI"
    // Returns:
    //  Void
    public void pauseGame() {
        gamePlayMusic.Pause();
        select.Play();
        pauseButton.SetActive(false);
        pauseMenuUI.SetActive(true);
        ballScript.hideBall();
        (racketScript.GetComponent("dragCircle") as MonoBehaviour).enabled = false;
        Time.timeScale = .0001f;
    }

    // When resume button triggered, unfreezes the game and hides "pauseMenuUI" to show gameboard once again
    // Returns:
    //  Void
    public void resumeGame() {
        countDown.Play();
        pauseButton.SetActive(true);
        pauseMenuUI.SetActive(false);
        ballScript.showBall();
        StartCoroutine(waitThenResume());
    }

    // Helper coroutine for "resumeGame"
    // Waits 3 seconds, then continues game to avoid abrupt restart
    IEnumerator waitThenResume() {
        yield return new WaitForSeconds(3f * Time.timeScale);
        Time.timeScale = 1f;
        (racketScript.GetComponent("dragCircle") as MonoBehaviour).enabled = true;
        gamePlayMusic.Play();
    }

}
