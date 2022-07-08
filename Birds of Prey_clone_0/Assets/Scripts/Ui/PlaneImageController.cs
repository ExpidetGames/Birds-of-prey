using UnityEngine;
using UnityEngine.UI;

public class PlaneImageController : MonoBehaviour {

    [SerializeField] private GameObject borders;
    [SerializeField] private Image spriteDisplay;
    [HideInInspector] public SelectionUIManager manager;
    [HideInInspector] public PlaneTypes displayedPlaneType;
    [HideInInspector] public int myIndex;
    [HideInInspector] public bool selected;

    public void OnPressed() {
        manager.selectElement(myIndex);
    }

    public void select() {
        selected = !selected;
        borders.SetActive(selected);
    }

    public void setImage(PlaneTypes planeType) {
        spriteDisplay.sprite = PrefabOrganizer.Planes[planeType].planeSprite;
        displayedPlaneType = planeType;
    }
}
