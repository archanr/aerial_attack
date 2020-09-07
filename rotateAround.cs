using UnityEngine;

// Used within "MainMenu" scene
// Deals with auto rotating racket in main menu screen
public class rotateAround : MonoBehaviour {

    // Variables used throughout for rotating logic 
    [Header("Attributes")]
    public float speed;
    public Transform target;
    private Vector3 zAxis = new Vector3(0, 0, 1);

    // At start of scene, before gameplay has begun, initalize direction of rotation
    // Returns:
    //  Void
    void Start() {
        int randomInt = Random.Range(0, 2);

        if (randomInt == 0) {
            speed = 1f;
        } else if (randomInt == 1) {
            speed = -1f;
        }
    }

    // During lifetime, keep rotating around target pivot point
    // Returns:
    //  Void
    void FixedUpdate() {
        this.transform.RotateAround(target.position, zAxis, speed);
    }

}
