using System.Collections;
using UnityEngine;

// Serves as the game manager for "MultiplayerLocal" scene
// Handles scoring, interactions with other gameobjects/scripts, sounds, and game logic
public class gameManagerMultLocal : MonoBehaviour {

    const int BOTTOM = -1;
    const int BOTH = 0;
    const int TOP = 1;

    // Miscellaneous variables used throughout for game logic 
    [Header("Attributes")]
    private float multTimes = 0; 
    private int numberOfBounces = 0;
    private float powerUpLength = 5f;
    private bool startingGameOver = false;
    private bool initialBallDrop = false;
    private int lastTouched = 0;     // Determines which racket hit the ball last, later used to calculate losing and winning screen to appropriate sides
                                     // 0 neither last touched, -1 bottom last touched, 1 top last touched
    private int scoreToWin = 3;
    private int bottomPlayerScore = 0;
    private int topPlayerScore = 0;

    // References to other scripts, used for powerup effects 
    [Header("Scripts")]
    public ball ballScript;
    public dragCircleMult topRacket;
    public dragCircleMult bottomRacket;

    // Text/UI variables that are displayed during gameplay 
    [Header("Text/UI")]
    public GameObject gameOverUI;
    public GameObject pauseMenuUI;
    public Healthbar bottomPowerUpBar; // Bottom screen powerup bar slider
    public Healthbar topPowerUpBar;    // Top screen powerup bar slider
    public updateScore bottomOutcome;  // Bottom screen text object within "gameOverUI" that displays "Win" or "Lose" result
    public updateScore topOutcome;     // Top screen text object within "gameOverUI" that displays "Win" or "Lose" result
    public cameraShake shakeEffect;
    public GameObject rackets;
    public GameObject UICanvas;
    public updateScore topPlayerScoreBoard;
    public updateScore bottomPlayerScoreBoard;
    public GameObject pauseButton;

    // Prefab variables which contain powerup gameobjects
    [Header("Prefabs")]
    public GameObject slowBallPrefab;
    public GameObject enlargeRacketPrefab;


    // AudioSource variables which contain reference to specific audio files within the game
    [Header("Audio")]
    public AudioSource[] sounds;
    public AudioSource pongHit1;
    public AudioSource pongHit2;
    public AudioSource gameOver;
    public AudioSource slowBall;
    public AudioSource enlargeRacket;
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
        countDown = sounds[5];
        select = sounds[6];
        gamePlayMusic = sounds[7];
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

    // Handles game rounds every time ball fall out of bounds
    // Either displays game over screen (top or bottom player score reaches 3), or sets up next round
    // Returns:
    //  Void
    public void replayOrOver() {
        gameOver.Play();
        shakeEffect.ShakeIt();

        if (lastTouched == TOP) {
            topPlayerScore++;
            topPlayerScoreBoard.updateText(topPlayerScore.ToString());
        } else if (lastTouched == BOTTOM) {
            bottomPlayerScore++;
            bottomPlayerScoreBoard.updateText(bottomPlayerScore.ToString());
        }

        // If top or bottom player has reached the score to win, initiate game over screen
        if (topPlayerScore == scoreToWin || bottomPlayerScore == scoreToWin) {
            startingGameOver = true;
            gamePlayMusic.Pause();
            initiateGameOver();
        } else {
            // Else, setup for next round
            // Drop ball again, reset powerups, ball speed, and destory all power ups
            ballScript.dropBall();
            turnOffPowerUpBar(BOTH);
            ballScript.changeSpeed(1 - (multTimes / 1f));
            multTimes = 0f;
            removeAllPowerups();
        }

        return;
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

    // When game over initated, call function to decide win/lose screens and show results
    // Returns:
    //  Void
    public void initiateGameOver() {
        decideWinLoseSides();
        StartCoroutine(waitForWhile());
    }

    // Determines which player (top or bottom) has won, and updates corresponding "Win/Lose" text elements to respective sides
    // Returns:
    //  Void
    public void decideWinLoseSides() {
        if (bottomPlayerScore == scoreToWin) {
            topOutcome.updateText("Win");
            bottomOutcome.updateText("Lose");
        } else if (topPlayerScore == scoreToWin) {
            topOutcome.updateText("Lose");
            bottomOutcome.updateText("Win");
        } else {
            bottomOutcome.updateText("Draw");
            topOutcome.updateText("Draw");
        }
    }

    // Helper coroutine for "initiateGameOver"
    // Sets 1.5 second delay before showing "gameOverUI" and hiding irrelevant gameobjects
    IEnumerator waitForWhile() {
        yield return new WaitForSeconds(1.5f);
        turnOffPowerUpBar(0);
        gameOverUI.SetActive(true);
        rackets.SetActive(false);
        UICanvas.SetActive(false);
    }

    // Every tenth bounce it will call to spawn a random powerup and increase ball speed by 10%
    // Returns:
    //  Void
    public void bounceCount() {
        // Alternate ping pong sound effects
        if (numberOfBounces % 2 == 0) {
            pongHit1.Play();
        } else {
            pongHit2.Play();
        }
        numberOfBounces++;

        // Instantiate a powerup every 10 bounces and increase ball speed by 10%
        if (numberOfBounces % 10 == 0 && numberOfBounces != 0) {
            spawnPowerUp();
            ballScript.changeSpeed(1.1f);
            multTimes += .1f;
        }
        return;
    }

    // Setter for last touched variable (bottom or top)
    // Parameters:
    //  lastTouched - The racket identifier that had last touched the ball
    // Returns:
    //  Void
    public void setLastTouched(int lastTouched) {
        this.lastTouched = lastTouched;
    }

    // Pick random powerup to create, then spawn it onto gameboard
    // Returns:
    //  Void
    public void spawnPowerUp() {
        GameObject powerUpObject = null;
        int randomInt = Random.Range(0, 2);

        if (randomInt == 0) {
            powerUpObject = Instantiate(slowBallPrefab) as GameObject;
        } else if (randomInt == 1) {
            powerUpObject = Instantiate(enlargeRacketPrefab) as GameObject;
        }

        powerUpObject.transform.position = this.transform.position;
    }

    // Enables various powerups within the game
    // Parameters:
    //  powerUpType - type of powerup
    //      slowBall            -  Slows down ball speed by 50%
    //      enlargeTopRacket    -  Top racket size increases only
    //      enlargeBottomRacket -  Bottom racket size increases only
    // Returns:
    //  Void
    public void powerUp(string powerUpType) {
        if (startingGameOver == false) {
            if (powerUpType == "slowBall") {
                slowBall.Play();
                ballScript.changeSpeed(0.5f);
                startSlidePowerUpBar(BOTH);
            } else if (powerUpType == "enlargeTopRacket") {
                topRacket.changeSize(50f);
                enlargeRacket.Play();
                startSlidePowerUpBar(TOP);
            } else if (powerUpType == "enlargeBottomRacket") {
                bottomRacket.changeSize(50f);
                enlargeRacket.Play();
                startSlidePowerUpBar(BOTTOM);
            }

            StartCoroutine(powerDown(powerUpType));
        }
        return;
    }

    // Helper coroutine for "powerUp"
    // Waits "powerUpLength" time, then reverts powerup effects that were set in "powerUp" function
    public IEnumerator powerDown(string powerUpType) {
        yield return new WaitForSeconds(powerUpLength);
        if (powerUpType == "slowBall") {
            ballScript.changeSpeed(2f);
            turnOffPowerUpBar(BOTH);
        } else if (powerUpType == "enlargeTopRacket") {
            topRacket.changeSize(35f);
            turnOffPowerUpBar(TOP);
        } else if (powerUpType == "enlargeBottomRacket") {
            bottomRacket.changeSize(35f);
            turnOffPowerUpBar(BOTTOM);
        }
    }


    // When powerup has been activated, enable powerup bar slider and begin countdown
    // Parameters:
    //  section - section to enable powerup bar slider
    //      TOP    - top side only
    //      BOTTOM - bottom side only
    // Returns:
    //  Void
    public void startSlidePowerUpBar(int section) {
        if (section == BOTTOM) {
            turnOnPowerUpBar(section);
            bottomPowerUpBar.SetHealth(100f);
            bottomPowerUpBar.healthPerSecond = 20f;
        } else if (section == TOP) {
            turnOnPowerUpBar(section);
            topPowerUpBar.SetHealth(100f);
            topPowerUpBar.healthPerSecond = 20f;
        } else {
            startSlidePowerUpBar(BOTTOM);
            startSlidePowerUpBar(TOP);
        }
        return;
    }


    // Turns power up bar visibility on if it has been turned off
    // Parameters:
    //  section - section to turn on powerup bar
    //      TOP    - top powerup bar only
    //      BOTTOM - bottom powerup bar only
    // Returns:
    //  Void
    public void turnOnPowerUpBar(int type) {
        if (type == BOTTOM) {
            if (bottomPowerUpBar.gameObject.activeSelf == false) {
                bottomPowerUpBar.gameObject.SetActive(true);
            }
        } else if (type == TOP) {
            if (topPowerUpBar.gameObject.activeSelf == false) {
                topPowerUpBar.gameObject.SetActive(true);
            }
        } else {
            turnOnPowerUpBar(BOTTOM);
            turnOnPowerUpBar(TOP);
        }
    }

    // Turns power up bar visibility off if it has been turned on
    // Parameters:
    //  section - section to turn off powerup bar
    //      TOP    - top powerup bar only
    //      BOTTOM - bottom powerup bar only
    // Returns:
    //  Void
    public void turnOffPowerUpBar(int type) {
        if (type == BOTTOM) {
            if (bottomPowerUpBar.gameObject.activeSelf == true) {
                bottomPowerUpBar.gameObject.SetActive(false);
            }
        } else if (type == TOP) {
            if (topPowerUpBar.gameObject.activeSelf == true) {
                topPowerUpBar.gameObject.SetActive(false);
            }
        } else {
            turnOffPowerUpBar(BOTTOM);
            turnOffPowerUpBar(TOP);
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
        (topRacket.GetComponent("dragCircleMult") as MonoBehaviour).enabled = false;
        (bottomRacket.GetComponent("dragCircleMult") as MonoBehaviour).enabled = false;
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
        (topRacket.GetComponent("dragCircleMult") as MonoBehaviour).enabled = true;
        (bottomRacket.GetComponent("dragCircleMult") as MonoBehaviour).enabled = true;
        gamePlayMusic.Play();
    }

}
