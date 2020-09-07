using UnityEngine;

// Used within "MainMenu", "SingleplayerInstruction", and "MultiplayerInstruction" scenes 
// Deals with keeping game music loaded and playing it seamlessly between scenes
public class dontDestroy : MonoBehaviour {

    // On awake, find all gameobjects with "Music" tag
    // If multiple, delete all but one gameobjects
    // Returns:
    //  Void
    void Awake() {
        GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");

        if (musics.Length > 1) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

}
