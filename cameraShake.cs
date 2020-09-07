using UnityEngine;

// Used within "Singleplayer" and "MultiplayerLocal" scenes  
// Deals with camera shake effect when game/round has finished
public class cameraShake : MonoBehaviour {

	// Miscellaneous variables used throughout for camera shake logic 
	[Header("Attributes")]
	private Vector3 cameraInitialPosition;
	public float shakeMagnitude = 0.05f, shakeTime = 0.5f;
	public Camera mainCamera;

	// Calls helper functions to shake camera by "shakeMagnitude" for "shakeTime" seconds
	// Returns:
	//  Void
	public void ShakeIt() {
		cameraInitialPosition = mainCamera.transform.position;
		InvokeRepeating("StartCameraShaking", 0f, 0.005f);
		Invoke("StopCameraShaking", shakeTime);
	}

	// Helper function to shake camera by "shakeMagnitude"
	// Returns:
	//  Void
	void StartCameraShaking() {
		float cameraShakingOffsetX = Random.value * shakeMagnitude * 2 - shakeMagnitude;
		float cameraShakingOffsetY = Random.value * shakeMagnitude * 2 - shakeMagnitude;
		Vector3 cameraIntermadiatePosition = mainCamera.transform.position;
		cameraIntermadiatePosition.x += cameraShakingOffsetX;
		cameraIntermadiatePosition.y += cameraShakingOffsetY;
		mainCamera.transform.position = cameraIntermadiatePosition;
	}

	// Helper function to stop camera shake and return to original state after "shakeTime" seconds
	// Returns:
	//  Void
	void StopCameraShaking() {
		CancelInvoke("StartCameraShaking");
		mainCamera.transform.position = cameraInitialPosition;
	}

}
