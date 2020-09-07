using System.Collections;
using UnityEngine;

// Used within "Singleplayer" and "MultiplayerLocal" scenes 
// Deals with the physics of the pong ball during gameplay
public class ball : MonoBehaviour {

    // Miscellaneous variables used throughout for ball logic 
    [Header("Attributes")]
    public float speed = 750f;
    private int frame = 0;

    // References to other scripts, called to signal collision
    [Header("Scripts")]
    public gameManager singlePlayerManager;
    public gameManagerMultLocal multiPlayerManager;


    // AudioSource variables which contain reference to specific audio files within the game
    [Header("Audio")]
    public AudioSource countDown;

    // Position/Physics related variables utilized in movement calculations
    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private Vector3 startPoint;
    private TrailRenderer trailRenderer;
    private Renderer renderer;
    private bool once = true;

    // At start of scene, before gameplay has begun, initalize physics settings
    // Returns:
    //  Void
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        renderer = GetComponent<Renderer>();
        startPoint = rb.transform.position;
    }

    // Once game has begun, apply initial downward force to ball
    // Returns:
    //  Void
    public void dropBall() {
        if (once) {
            StartCoroutine(delayTime());
            once = false;
        }
        rb.transform.position = startPoint;
        rb.velocity = Vector2.down;
        StartCoroutine(waitForWhile());
    }

    // Helper coroutine for "dropBall"
    // Waits 1 second, then play countdown audio to avoid abrupt start
    IEnumerator delayTime() {
        yield return new WaitForSeconds(1f);
        countDown.Play();
    }

    // Helper coroutine for "dropBall"
    // Waits 4 seconds, then send ball moving to avoid abrupt start
    // If singleplayer, send ball initially downwards
    // If muliplayer, send ball randomly upwards or downwards
    IEnumerator waitForWhile() {
        yield return new WaitForSeconds(4f);

        if (multiPlayerManager != null) {
            int randomInt = Random.Range(0, 2);
            if (randomInt == 0) {
                rb.velocity = Vector2.down * speed;
            } else {
                rb.velocity = Vector2.up * speed;
            }
        } else {
            rb.velocity = Vector2.down * speed;
        }
    }


    // When game has been paused, hide ball and trail renderer
    // Returns:
    //  Void
    public void hideBall() {
        renderer.enabled = false;
        trailRenderer.enabled = false;
    }

    // When game has been resumed, show ball and trail renderer
    // Returns:
    //  Void
    public void showBall() {
        renderer.enabled = true;
        trailRenderer.enabled = true;
    }

    // Called during every physics related movement during gameplay
    // Set the last velocity equal to current velocity
    // Returns:
    //  Void
    void FixedUpdate() {
        lastVelocity = rb.velocity;
    }

    // Called every frame during gameplay
    // Returns:
    //  Void
    void Update() {

        // For multiplayer local, every 10 frames check whether ball is moving upwards or downwards,
        // Used to determine win/lose sides
        if (multiPlayerManager != null) {
            if (frame % 10 == 0) {
                if (rb.velocity.y < 0) {
                    multiPlayerManager.setLastTouched(1);
                } else if (rb.velocity.y > 0) {
                    multiPlayerManager.setLastTouched(-1);
                }
            }
            frame++;
        }

    }

    // If ball collides with any racket, calculate new bounce angle and send moving in that direction while speed remains same
    // Parameters:
    //  col - Data associated with collision and incoming collider
    // Returns:
    //  Void
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Racket") {
            // Reflects the direction that the ball was hit at
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, col.GetContact(0).normal);

            // Calculate new velocity (direction + magnitude)
            rb.velocity = direction * speed;

            // Send call to update score/bounce
            if (singlePlayerManager != null) {
                singlePlayerManager.updateExistingScore();
            } else if (multiPlayerManager != null) {
                multiPlayerManager.bounceCount();
            }

        }
        return;
    }

    // Adjust the speed of the ball
    // Parameters:
    //  factor - Percentage factor to change speed of ball by 
    // Returns:
    //  Void
    public void changeSpeed(float factor) {
        this.speed = this.speed * factor;
        rb.velocity = rb.velocity * factor;
        return;
    }
}

