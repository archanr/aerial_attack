using UnityEngine;

// Used within "MultiplayerLocal" scene
// Deals with finger placement to in-game racket movement for multiple, simulataneous touches
public class dragCircleMult : MonoBehaviour {

    // Position vectors used to accurately calculate angle movement
    [Header("Positions/Attributes")]
    public Transform orb;
    public Shaper2D myRacket;
    public GameObject particle;
    private Transform pivot;
    private Vector2 lastOrbVector;
    private float angle = 0.0f;

    // Debugging options, used for testing
    [Header("Options")]
    public bool testingOnPC = false;
    public bool bottomPlayer;

    // Flags used for initializing first racket/touch interaction
    private bool onceTop = true;
    private bool onceBottom = true;

    // At start of scene, before gameplay has begun, initalize positions and settings
    // Returns:
    //  Void
    void Start() {
        pivot = orb.transform;
        transform.parent = pivot;
        transform.position += Vector3.up;
        Input.multiTouchEnabled = true;
    }

    // Called every frame during gameplay
    // Handles touch to racket movement for both players simulataneously
    // Returns:
    //  Void
    void Update() {
        // Initialize pivot point
        Vector3 orbVector = Camera.main.WorldToScreenPoint(orb.position);
        if (testingOnPC) {
            orbVector = Input.mousePosition - orbVector;
            angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg + 90f;
            pivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // If touch detected on screen
        if (Input.touchCount > 0) {

            foreach (Touch touch in Input.touches) {
                // If touch detected in top half of screen, must be top player and alter top racket only
                if (touch.position.y > (Screen.height / 2) && bottomPlayer == false) {
                    orbVector = new Vector3(touch.position.x, touch.position.y, 0) - orbVector;
                    Vector2 currentTouch = new Vector2(touch.position.x, touch.position.y);

                    if (onceTop == true) {
                        lastOrbVector = currentTouch;
                        onceTop = false;
                    }

                    // The distance from the last touch to the racket to the current touch must not be larger than 1/4 screen width
                    // Converts current finger coordinates to angular distance and moves racket
                    if (Vector2.Distance(lastOrbVector, currentTouch) < Screen.width / 4) {
                        angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg - 90f;
                        pivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        lastOrbVector = currentTouch;
                    }

                    // If touch detected in bottom half of screen, must be bottom player and alter bottom racket only
                } else if (touch.position.y < (Screen.height / 2) && bottomPlayer == true) {
                    orbVector = new Vector3(touch.position.x, touch.position.y, 0) - orbVector;
                    Vector2 currentTouch = new Vector2(touch.position.x, touch.position.y);

                    if (onceBottom == true) {
                        lastOrbVector = currentTouch;
                        onceBottom = false;
                    }

                    // The distance from the last touch to the racket to the current touch must not be larger than 1/4 screen width
                    // Converts current finger coordinates to angular distance and moves racket
                    if (Vector2.Distance(lastOrbVector, currentTouch) < Screen.width / 4) {
                        angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg + 90f;
                        pivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                        lastOrbVector = currentTouch;
                    }
                }
            }
        }

    }

    // Instantiate a particle effect where ball collides with racket
    // Parameters:
    //  col - Data associated with collision and incoming collider
    // Returns:
    //  Void
    void OnCollisionEnter2D(Collision2D col) {
        string colName = col.gameObject.name;
        if (colName == "PongBall") {
            Instantiate(particle, new Vector3(col.GetContact(0).point.x, col.GetContact(0).point.y, 0), Quaternion.identity);
        }
    }

    // Used for powerup altercations
    // Changes racket size
    // Parameters:
    //  size - The new size of the racket
    // Returns:
    //  Void
    public void changeSize(float size) {
        myRacket.arcDegrees = size;
    }

}
