using UnityEngine;
using TMPro;

// Used within all scenes 
// Deals with updating text UI elements
public class updateScore : MonoBehaviour {
    public TextMeshProUGUI textmeshPro;

    // At start of scene, before gameplay has begun, initalize text source
    // Returns:
    //  Void
    void Start() {
        textmeshPro = GetComponent<TextMeshProUGUI>();
    }

    // Updates text UI component with "displayText" contents
    // Parameters:
    //  displayText - string of text to display on text UI component
    // Returns:
    //  Void
    public void updateText(string displayText) {
        textmeshPro.text = displayText;
    }

}
