using UnityEngine;
using UnityEngine.UI;

public class selectedPlanePreviewManager : MonoBehaviour {

    [SerializeField] private GameObject borders;
    [SerializeField] private Image spriteDisplay;

    [HideInInspector] public int myIndex;
    [HideInInspector] public SelectionUIManager manager;

    private PlaneTypes currentlySelectedPlaneType;

    public void OnPressed() {
        manager.currentlySelectedIndexToChange = myIndex;
    }

    public void select() {
        borders.SetActive(true);
    }

    public void deselect() {
        borders.SetActive(false);
    }

    public void changeImage(PlaneTypes newPlaneType) {
        currentlySelectedPlaneType = newPlaneType;
        spriteDisplay.sprite = PrefabOrganizer.Planes[currentlySelectedPlaneType].planeSprite;
    }
}
