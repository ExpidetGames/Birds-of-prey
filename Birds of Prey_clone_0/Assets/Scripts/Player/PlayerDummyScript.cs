using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDummyScript : MonoBehaviour {
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private Canvas nameTagCanvas;

    public void setName(string name) {
        nameDisplay.text = name;
    }

    public void disableNameTag() {
        nameTagCanvas.enabled = false;
    }
    public void rotateNameTowards() {
        nameDisplay.transform.LookAt(Camera.main.transform);
        nameDisplay.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
