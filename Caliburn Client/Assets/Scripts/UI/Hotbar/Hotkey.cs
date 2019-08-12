using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hotkey : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

    public enum Mode {
        None = 0,        
        Till = 1,
        Plant = 2,
        Water = 3,
        Fertilize = 4,
        Harvest = 5,
        Chop = 6,
        Uproot = 7,
    }

    public string keystroke;
    public string input;
    public Image image;
    public Item item;

    void Awake() {
        input = "Hotkey " + (transform.GetSiblingIndex()+1);
    }

    void Update() {
        if(Input.GetButtonDown(input)) {
            OnHotkey();
        }
    }

    void OnHotkey() {
        if(item == null) {return;}

        switch(item.primaryUsage) {
            case "till":
                Debug.Log("Till mode");
                PlayerController.Instance.SetPrimaryUsage(item.primaryUsage, item);
            break;

            case "plant":
                Debug.Log("Plant mode");
                PlayerController.Instance.SetPrimaryUsage(item.primaryUsage, item);
            break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        //OnHotkey();
        //InventoryView.Instance.dragImage.enabled = true;
        //InventoryView.Instance.dragImage.sprite = image.sprite;
        //dragging = true;
    }

    public void OnDrag(PointerEventData eventData) {
        //InventoryView.Instance.dragImage.rectTransform.localPosition = eventData.position - (Vector2)InventoryView.Instance.rectTransform.position + new Vector2(-24f, 24f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        //OnHotkey();
        //image.rectTransform.localPosition = Vector2.zero;
        //InventoryView.Instance.dragImage.rectTransform.localPosition = Vector2.zero;
        //InventoryView.Instance.dragImage.enabled = false;
        //dragging = false;
    }

    public void OnDrop(PointerEventData eventData) {
        ItemSlot itemSlot = eventData.pointerDrag.GetComponent<ItemSlot>();
        Hotkey hotkey = eventData.pointerDrag.GetComponent<Hotkey>();

        Debug.Log(itemSlot.item);
        if(itemSlot == null && hotkey == null) {return;}

        if(itemSlot != null) {

            SetItem(itemSlot);
        } else if(hotkey != null) {
            SwapHotkey(hotkey);
        }
        
        //image.rectTransform.localPosition = Vector2.zero;
        //InventoryView.Instance.dragImage.rectTransform.localPosition = Vector2.zero;
        //InventoryView.Instance.dragImage.enabled = false;
        //dragging = false;
    }

    void SetItem(ItemSlot itemSlot) {
        if(itemSlot.item == null) {return;}
        Debug.Log("Set item");
        image.sprite = itemSlot.image.sprite;
        item = itemSlot.item;


    }

    void SetAction() {

    }

    void SwapHotkey(Hotkey hotkey) {

    }
}
