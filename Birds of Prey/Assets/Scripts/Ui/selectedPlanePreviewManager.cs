using UnityEngine;
using UnityEngine.UI;

public class selectedPlanePreviewManager : MonoBehaviour {

    [SerializeField] private GameObject borders;
    [SerializeField] private Image spriteDisplay;

    [HideInInspector] public int myIndex;
    [HideInInspector] public SelectionUIManager manager;
    [HideInInspector] public PlaneTypes currentlySelectedPlaneType;
    [HideInInspector] public bool hasBeenSet = false;


    public void OnPressed() {
        manager.changeSelectedSelectionPreview(myIndex);
    }

    public void select() {
        borders.SetActive(true);
    }

    public void deselect() {
        borders.SetActive(false);
    }

    public void changeImage(PlaneTypes newPlaneType) {
        hasBeenSet = true;
        currentlySelectedPlaneType = newPlaneType;
        spriteDisplay.sprite = PrefabOrganizer.Planes[currentlySelectedPlaneType].planeSprite;
    }
}
