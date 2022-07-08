using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class SelectionUIManager : MonoBehaviour {

    [HideInInspector] public int currentlySelectedIndexToChange = 0;

    [SerializeField] private GameObject contentHolder;
    [SerializeField] private GameObject selectedPlanePreview;
    [SerializeField] private GameObject planePreviewImagePrefab;
    [SerializeField] private GameObject selectedPlanesHolder;

    private List<PlaneImageController> allPreviewControllers;
    private selectedPlanePreviewManager[] allSelectionImageControllers;


    private void Start() {
        allPreviewControllers = new List<PlaneImageController>();
        allSelectionImageControllers = selectedPlanesHolder.GetComponentsInChildren<selectedPlanePreviewManager>();

        for(int i = 0; i < allSelectionImageControllers.Length; i++) {
            allSelectionImageControllers[i].myIndex = i;
            allSelectionImageControllers[i].manager = this;
        }

        Populate();

    }

    void Populate() {
        int currentIndex = 0;
        foreach(PrefabOrganizer.Plane plane in PrefabOrganizer.Planes.Values) {
            GameObject newPlaneImage = Instantiate(planePreviewImagePrefab);
            PlaneImageController imageController = newPlaneImage.GetComponentInChildren<PlaneImageController>();

            imageController.myIndex = currentIndex;
            imageController.setImage(plane.playerType);
            imageController.manager = this;

            newPlaneImage.transform.SetParent(contentHolder.transform);
            allPreviewControllers.Add(imageController);
            currentIndex++;
        }
    }

    public void selectElement(int index) {
        allSelectionImageControllers[currentlySelectedIndexToChange].changeImage(allPreviewControllers[index].displayedPlaneType);
    }

    public void changeSelectedSelectionPreview(int newIndex) {
        allSelectionImageControllers[currentlySelectedIndexToChange].deselect();
        allSelectionImageControllers[newIndex].select();
        currentlySelectedIndexToChange = newIndex;
    }
}
