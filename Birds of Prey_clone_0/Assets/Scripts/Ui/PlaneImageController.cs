using UnityEngine;

public class PlaneImageController : MonoBehaviour {

    [SerializeField] private GameObject borders;
    [HideInInspector] public SelectionUIManager manager;
    [HideInInspector] public int myIndex;

    public void OnPressed() {
        manager.selectElement(myIndex);
    }

    public PlaneImageController select() {
        borders.SetActive(true);
        return this;
    }

    public void deselect() {
        borders.SetActive(false);
    }
}
