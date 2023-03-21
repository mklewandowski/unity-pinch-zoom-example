using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollZoomImage : MonoBehaviour, IScrollHandler
{
    private Vector3 initialScale;
    private Vector3 currentScale;
    private Vector3 currentUnscaledCenter;

    [SerializeField]
    private float zoomSpeed = 0.1f;
    [SerializeField]
    private float maxZoom = 10f;
    [SerializeField]
    private ScrollRect scrollRect;

    private void Awake()
    {
        initialScale = transform.localScale;
        currentScale = initialScale;
    }

    public void OnScroll(PointerEventData eventData)
    {
        var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);

        transform.localScale = new Vector3(desiredScale.x, desiredScale.y, 1f);
        currentScale = desiredScale;
        this.transform.localPosition = currentUnscaledCenter * (currentScale.x / initialScale.x);
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(initialScale, desiredScale);
        desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
        return desiredScale;
    }

	// Update is called once per frame
	void Update()
    {
        if (Input.touchCount == 2)
        {
            scrollRect.enabled = false; // the touch events and the scroll rect clash, turn the scroll rect off when pinch zooming
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float pinchDelta = currentMagnitude - prevMagnitude;

            PinchZoom(pinchDelta * 0.01f);
            this.transform.localPosition = currentUnscaledCenter * (currentScale.x / initialScale.x);
        }
        else
        {
            scrollRect.enabled = true;
            currentUnscaledCenter = this.transform.localPosition * (initialScale.x / currentScale.x);
        }
	}

    void PinchZoom(float pinchDelta)
    {
        var delta = Vector3.one * (pinchDelta);

        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);

        transform.localScale = new Vector3(desiredScale.x, desiredScale.y, 1f);
        currentScale = desiredScale;
    }
}
