using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class InventoryView : MonoSingleton<InventoryView> {

    const float minimumWidth = 256f;
    const float maximumWidth = 728f;
    const float minimumHeight = 152f;
    const float maximumHeight = 460f;
    public RectTransform rectTransform;
    InventoryPanel inventoryPanel;
    public Button closeButton;
    public DragHandler resizeButton;

    public List<Item> items;
    public InventorySlotContainer slotContainer;

    public Image dragImage;

    public bool syncing = false;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        inventoryPanel = GetComponent<InventoryPanel>();
    }

    void Start() {

    }

    void FixedUpdate() {
        if(inventoryPanel.dragging) {
            Move();
        }

        if(resizeButton.dragging) {
            Resize();
        }
    }

    void Move() {
        float dragPositionX = inventoryPanel.currentPosition.x - inventoryPanel.startPosition.x;
        float dragPositionY = inventoryPanel.currentPosition.y - inventoryPanel.startPosition.y;
        dragPositionX = Mathf.Clamp(dragPositionX, UIBounds.paddingX, Screen.width - UIBounds.paddingX);
        dragPositionY = Mathf.Clamp(dragPositionY, UIBounds.paddingY, Screen.height - UIBounds.paddingY);
        rectTransform.position = new Vector2(dragPositionX, dragPositionY);
    }

    void Resize() {
        float resizeWidth = resizeButton.currentPosition.x - rectTransform.position.x;
        float resizeHeight = rectTransform.position.y - resizeButton.currentPosition.y;
        resizeWidth = Mathf.Clamp(resizeWidth, minimumWidth, maximumWidth);
        resizeHeight = Mathf.Clamp(resizeHeight, minimumHeight, maximumHeight);
        rectTransform.sizeDelta = new Vector2(resizeWidth, resizeHeight);
    }

    public void RequestInventoryData() {
        PlayerInventorySyncRequest inventorySyncRequest = new PlayerInventorySyncRequest {
            networkId = Client.Instance.playerIdentity.netId,
        };
        inventorySyncRequest.HandleRequest();
    }

    public void BeginSync() {
        Debug.Log("Begin sync");
        syncing = true;
        items = new List<Item>();
    }

    public void EndSync() {
        Debug.Log("End sync");
        LoadItems();
        syncing = false;  
    }

    public void LoadItems() {
        slotContainer.LoadItems(items);
    }

    public bool IsMouseOver() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
