using UnityEngine;

// Used within "Singleplayer" scene
// Deals with finger placement to in-game racket movement for single touch at a time
public class dragCircle : MonoBehaviour {

    // Position vectors used to accurately calculate angle movement
    [Header("Positions/Attributes")]
    public Transform orb;
    private Transform pivot;
    public Shaper2D myRacket;
    public GameObject particle;
    private Vector2 lastOrbVector;
    private float angle = 0.0f;

    // Debugging options, used for testing
    [Header("Options")]
    bool once = true;
    public bool testingOnPC = false;

    // At start of scene, before gameplay has begun, initalize positions
    // Returns:
    //  Void
    void Start() {
        pivot = orb.transform;
        transform.parent = pivot;
        transform.position += Vector3.up;
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
        int i = 0;
        while (i < Input.touchCount) {
            orbVector = new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, 0) - orbVector;
            Vector2 currentTouch = new Vector2(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y);

            if (once == true) {
                lastOrbVector = currentTouch;
                once = false;
            }

            // The distance from the last touch to the racket to the current touch must not be larger than 1/4 screen width
            // Converts current finger coordinates to angular distance and moves racket
            if (Vector2.Distance(lastOrbVector, currentTouch) < Screen.width / 4) {
                angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg + 90f;
                pivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                lastOrbVector = currentTouch;
            }
            i++;
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


