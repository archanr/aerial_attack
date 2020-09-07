using UnityEngine;

// Used within "Singleplayer" and "MultiplayerLocal" scenes  
// Deals with the invisible boundary that the ball must cross over to intitiate a "game-over" sequence
public class outOfBounds : MonoBehaviour {

    // Miscellaneous variables used throughout for powerup logic 
    [Header("Attributes")]
    public gameManager singlePlayerManager;
    public gameManagerMultLocal multiPlayerManager;

    // If anything enters "invisible" out of bounds area, initiate game/round over
    // Parameters:
    //  other - Data associated with trigger and incoming collider
    // Returns:
    //  Void
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "PongBall") {
            if (singlePlayerManager != null) {
                singlePlayerManager.initiateGameOver();
            } else if (multiPlayerManager != null) {
                multiPlayerManager.replayOrOver();
            }
        }
    }

}
