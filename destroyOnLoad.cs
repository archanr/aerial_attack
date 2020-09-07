using UnityEngine;

// Used within "Singleplayer" and "MultiplayerLocal" scenes 
// Deals with destroying "MainMenu" game music when loaded into game scenes
public class destroyOnLoad : MonoBehaviour {

    // On awake, destroy game object with "Music" tag
    // Returns:
    //  Void
    void Awake() {
        GameObject music = GameObject.FindGameObjectWithTag("Music");
        Destroy(music);
    }

}
