using UnityEngine;
using UnityEngine.UI;

public class SelectionUIManager : MonoBehaviour {

    [SerializeField] private GameObject planeTypeUIBlueprint;
    [SerializeField] private GameObject ContentParent;

    [SerializeField] private int distanceFromTop;
    [SerializeField] private int distanceFromRight;
    [SerializeField] private int distanceFromBottom;
    [SerializeField] private int distanceFromLeft;
    [SerializeField] private int distanceBetweenRows;
    [SerializeField] private int distanceBetween;




    private void Start() {

        for(int y = 0; y < 10; y++) {
            for(int x = 0; x < 50; x++) {
                GameObject currentPlaneType = Instantiate(planeTypeUIBlueprint);
                // currentPlaneType.GetComponentInChildren<Image>().color = Color.black;
                currentPlaneType.transform.SetParent(ContentParent.transform);
                float newXDelta = Mathf.Clamp((currentPlaneType.GetComponentInChildren<Image>().rectTransform.rect.width + 10) * x, distanceFromLeft, ContentParent.GetComponentInChildren<RectTransform>().rect.x + ContentParent.GetComponentInChildren<RectTransform>().rect.width + distanceFromRight);
                float newYDelta = Mathf.Clamp((currentPlaneType.GetComponentInChildren<Image>().rectTransform.rect.height + 10) * y, distanceFromTop, ContentParent.GetComponentInChildren<RectTransform>().rect.y + ContentParent.GetComponentInChildren<RectTransform>().rect.height + distanceFromBottom);
                Debug.Log($"Max x Value {ContentParent.GetComponentInChildren<RectTransform>().rect.x + Mathf.Abs(ContentParent.GetComponentInChildren<RectTransform>().rect.width) - distanceFromRight}");
                currentPlaneType.transform.position = new Vector3(currentPlaneType.transform.position.x + newXDelta, currentPlaneType.transform.position.y - 100 - (currentPlaneType.GetComponentInChildren<Image>().rectTransform.rect.height + 10) * y, currentPlaneType.transform.position.z);
            }
        }
    }
}
