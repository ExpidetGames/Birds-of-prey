using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
public class SelectionUIManager : MonoBehaviour {

    [SerializeField] private GameObject planeImage;
    [SerializeField] private Image selectedPlanePreview;
    [SerializeField] private Canvas JoinCanvas;
    [SerializeField] private Canvas selectionCanvas;

    private List<PlaneImageController> allPlanes = new List<PlaneImageController>();
    private PlaneImageController currentlySelectedElement;

    private void Start() {
        Populate();
    }

    void Populate() {
        GameObject currentObj;
        PlaneImageController currentController;

        for(int i = 0; i < PrefabOrganizer.Planes.Count; i++) {
            currentObj = (GameObject)Instantiate(planeImage, transform);
            currentObj.GetComponent<Image>().sprite = PrefabOrganizer.Planes[PrefabOrganizer.Planes.Keys.ElementAt(i)].planeSprite;
            currentController = currentObj.GetComponent<PlaneImageController>();
            currentController.myIndex = i;
            currentController.manager = this;
            allPlanes.Add(currentController);
        }
        currentlySelectedElement = allPlanes[0].select();
        selectedPlanePreview.sprite = PrefabOrganizer.Planes[PrefabOrganizer.Planes.Keys.ElementAt(currentlySelectedElement.myIndex)].planeSprite;
    }

    public void selectElement(int index) {
        if(currentlySelectedElement != null) {
            currentlySelectedElement.deselect();
        }
        currentlySelectedElement = allPlanes[index].GetComponent<PlaneImageController>().select();
        selectedPlanePreview.sprite = PrefabOrganizer.Planes[PrefabOrganizer.Planes.Keys.ElementAt(index)].planeSprite;
    }

    public void backToJoinScreen() {
        JoinCanvas.GetComponent<Canvas>().enabled = true;
        JoinCanvas.GetComponent<JoinUIManager>().planeType = PrefabOrganizer.Planes.Keys.ElementAt(currentlySelectedElement.myIndex);
        selectionCanvas.GetComponent<Canvas>().enabled = false;
    }
}
