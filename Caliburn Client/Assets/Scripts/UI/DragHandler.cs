using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public Vector2 startPosition;
    public Vector2 currentPosition;
    public bool dragging = false;

    public void OnBeginDrag(PointerEventData eventData) {
        startPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("Dragging");
        currentPosition = eventData.position;
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("End Drag");
        dragging = false;
    }
}
