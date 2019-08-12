using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public Vector2 startPosition;
    public Vector2 currentPosition;
    public bool dragging = false;

    public void OnBeginDrag(PointerEventData eventData) {
        startPosition = eventData.position - (Vector2)InventoryView.Instance.rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        currentPosition = eventData.position;
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData) {
        dragging = false;
    }
}
