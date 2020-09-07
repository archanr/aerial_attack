using System.Collections;
using UnityEngine;

// Used within "Singleplayer" and "MultiplayerLocal" scenes 
// Deals with the initial powerup motion and interactivity with racket
public class powerUpMotion : MonoBehaviour {

    // Miscellaneous variables used throughout for powerup logic 
    [Header("Attributes")]
    public GameObject myObject;
    public string type;
    public float speed = 25f;
    private Rigidbody2D rb;

    // At start of scene, before gameplay has begun, initalize physics settings
    // Returns:
    //  Void
    void Start() {
        myObject = GameObject.FindWithTag("Manager");

        // Apply initial force to powerup when starting game
        rb = GetComponent<Rigidbody2D>();
        // Set direction to random then change velocity
        Vector2 direction = Random.insideUnitCircle.normalized;
        rb.velocity = direction * speed;
        StartCoroutine(DestroyObject());
    }

    // Coroutine during lifetime of object
    // If object alive for longer than 10 seconds, destroy itself to save memory overhead
    IEnumerator DestroyObject() {
        yield return new WaitForSeconds(10f);
        if (gameObject != null) {
            Destroy(gameObject);
        }
    }

    // Called every fixed frame during gameplay
    // Handles object rotation during lifetime
    // Returns:
    //  Void
    void FixedUpdate() {
        transform.Rotate(0, 0, 50 * Time.deltaTime);
    }

    // Invoked if powerup object comes into contact with a racket
    // Sends call to start powerup effect in scene's respective "GameManager" script
    // Parameters:
    //  other - Data associated with trigger and incoming collider
    // Returns:
    //  Void
    void OnTriggerEnter2D(Collider2D other) {
        string racketType = other.gameObject.name;

        // If singleplayer mode racket
        if (racketType == "Racket" || racketType == "DoubleRacket") {
            myObject.GetComponent<gameManager>().powerUp(type);
            Destroy(gameObject);
        }

        // If local multiplayer mode racket
        if (racketType == "TopRacket" || racketType == "BottomRacket") {
            if (type == "enlargeRacket") {
                if (racketType == "TopRacket") {
                    myObject.GetComponent<gameManagerMultLocal>().powerUp("enlargeTopRacket");
                } else {
                    myObject.GetComponent<gameManagerMultLocal>().powerUp("enlargeBottomRacket");
                }
            } else {
                myObject.GetComponent<gameManagerMultLocal>().powerUp(type);
            }
            Destroy(gameObject);
        }
    }

}
