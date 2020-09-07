using System.Collections;
using UnityEngine;

// Used within "Singleplayer" scene
// Creates "bubble effect" for the board after every point is scored.
public class bubbleBoardEffect : MonoBehaviour {

    // Miscellaneous variables used throughout for bubble logic 
    [Header("Attributes")]
    public Shaper2D myRacket;
    public float increaseSize;
    public float waitTime;

    // Increases the board size by "increaseSize" in small intervals
    // Returns:
    //  Void
    public void bubbleEffect() {
        float minSize = myRacket.outerRadius - increaseSize;
        float orginalSize = myRacket.outerRadius;

        while (myRacket.outerRadius > minSize) {
            myRacket.outerRadius -= .01f;
        }

        StartCoroutine(waitForWhile(orginalSize));

        return;
    }

    // Helper coroutine for "bubbleEffect"
    // Waits "waitTime" seconds, then shrinks board size back to original size
    // Parameters:
    //  size - Size to shrink board size to
    IEnumerator waitForWhile(float size) {
        yield return new WaitForSeconds(waitTime);
        while (myRacket.outerRadius < size) {
            myRacket.outerRadius += .01f;
        }
    }

}