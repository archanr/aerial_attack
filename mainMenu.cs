using UnityEngine;

// Handles all scene transition buttons throughout the game
public class mainMenu : MonoBehaviour {

    // Variables used throughout
    [Header("Attributes")]
    public Color loadToColor = Color.black;
    private float sceneTransitionSpeed = 2f;
    private bool singleModeSelected = false;
    private bool multiModeSelected = false;

    // AudioSource variables which contain reference to specific audio files within the game
    [Header("Audio")]
    public AudioSource select;
    public AudioSource selectPlay;


    // At start of scene, before gameplay has begun, set selected to singleplayer
    void Start() {
        singleModeSelected = true;
    }

    // Triggered when "Singleplayer" button is pressed in "MainMenu" scene
    // Set singleplayer as selected and multiplayer as unselected
    // Returns:
    //  Void
    public void setSingleplayer() {
        select.Play();

        singleModeSelected = true;
        multiModeSelected = false;
    }

    // Triggered when "Multiplayer Local" button is pressed in "MainMenu" scene
    // Set singleplayer as unselected and multiplayer as selected
    // Returns:
    //  Void
    public void setMultiplayerLocal() {
        select.Play();

        singleModeSelected = false;
        multiModeSelected = true;
    }

    // Triggered when "Play Button" graphic is pressed in "MainMenu" scene
    //      If "singleModeSelected" = true - Load "Singleplayer" scene
    //      If "multiModeSelected"  = true - Load "MultiplayerLocal" scene
    // Returns:
    //  Void
    public void playButton() {
        selectPlay.Play();
        if (singleModeSelected == true) {
            Initiate.Fade("Singleplayer", loadToColor, sceneTransitionSpeed);
        } else if (multiModeSelected == true) {
            Initiate.Fade("MultiplayerLocal", loadToColor, sceneTransitionSpeed);
        }
    }

    // Triggered when "Instructions" button is pressed in "MainMenu" scene
    // Or when "Back" button graphic is pressed in "MultiplayerInstruction" scene
    // Loads "SingleplayerInstruction" scene
    // Returns:
    //  Void
    public void gotoSingleInstruction() {
        select.Play();
        Initiate.Fade("SingleplayerInstruction", loadToColor, sceneTransitionSpeed);
    }

    // Triggered when "Next" button graphic is pressed in "SingleplayerInstruction" scene
    // Loads "MultiplayerInstruction" scene
    // Returns:
    //  Void
    public void gotoMultInstruction() {
        select.Play();
        Initiate.Fade("MultiplayerInstruction", loadToColor, sceneTransitionSpeed);
    }

    // Triggered when "Home" button graphic is pressed within various scenes
    // Loads "MainMenu" scene, if reached from paused state, resume normal game time speed
    // Returns:
    //  Void
    public void returnToMainMenu() {
        if (Time.timeScale < 1f) {
            Time.timeScale = 1f;
        }
        selectPlay.Play();
        Initiate.Fade("Menu", loadToColor, sceneTransitionSpeed);
    }

    // Triggered when "Replay" button graphic is pressed in "Singleplayer" scene
    // Loads "Singleplayer" scene again, if reached from paused state, resume normal game time speed
    // Returns:
    //  Void
    public void singleplayerReplay() {
        if (Time.timeScale < 1f) {
            Time.timeScale = 1f;
        }
        selectPlay.Play();
        Initiate.Fade("Singleplayer", loadToColor, sceneTransitionSpeed);
    }

    // Triggered when "Replay" button graphic is pressed in "MultiplayerLocal" scene
    // Loads "MultiplayerLocal" scene again, if reached from paused state, resume normal game time speed
    // Returns:
    //  Void
    public void multiplayerLocalReplay() {
        if (Time.timeScale < 1f) {
            Time.timeScale = 1f;
        }
        selectPlay.Play();
        Initiate.Fade("MultiplayerLocal", loadToColor, sceneTransitionSpeed);
    }

}
