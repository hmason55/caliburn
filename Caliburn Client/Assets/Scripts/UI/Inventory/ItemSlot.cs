﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler  {
    public int index;
    public Item item;
    public Image image;
    public Text quantityText;
    bool dragging = false;

    InventorySlotContainer slotContainer;

    void Awake() {
        slotContainer = GetComponentInParent<InventorySlotContainer>();
    }

    public void LoadItem(int index, Item item) {
        this.index = index;
        this.item = item;
        image.sprite = ItemSpriteAtlas.Instance.items.GetSprite(item.img);
        if(item.quantity > 1) {
            quantityText.text = item.quantity.ToString();
        } else {
            quantityText.text = "";
        }
        
    }

    public void Clear() {
        image.sprite = null;
        quantityText.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData) {
        InventoryView.Instance.dragImage.enabled = true;
        InventoryView.Instance.dragImage.sprite = image.sprite;
        dragging = true;
    }

    public void OnDrag(PointerEventData eventData) {
        InventoryView.Instance.dragImage.rectTransform.localPosition = eventData.position - (Vector2)InventoryView.Instance.rectTransform.position + new Vector2(-24f, 24f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        image.rectTransform.localPosition = Vector2.zero;
        InventoryView.Instance.dragImage.rectTransform.localPosition = Vector2.zero;
        InventoryView.Instance.dragImage.enabled = false;
        dragging = false;
    }

    public void OnDrop(PointerEventData eventData) {
        //if(eventData.pointerDrag;
        //image.rectTransform.localPosition = Vector2.zero;
        //InventoryView.Instance.dragImage.rectTransform.localPosition = Vector2.zero;
        //InventoryView.Instance.dragImage.enabled = false;
        //dragging = false;
    }
}
