using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class PlayerController : MonoSingleton<PlayerController> {
    public enum State {
        Movement = 0,
        Till = 1,
        Plant = 2,
        Water = 3,
        Fertilize = 4,
        Harvest = 5,
        Chop = 6,
        Uproot = 7,
    }
    
    public string primaryUsage = "movement";
    NetworkIdentity identity;

    PlayerView playerView;

    public Item plantable;

    void Awake() {
        identity = GetComponent<NetworkIdentity>();
        playerView = GetComponent<PlayerView>();
    }

    void Start() {
        if(!identity.isLocalPlayer) {return;}

        Client.Instance.playerIdentity = identity;
        InventoryView.Instance.RequestInventoryData();
    }

    void Update() {
        if(!identity.isLocalPlayer) {return;}

        /*
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            state = State.Movement;
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            state = State.Plant;
        } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            state = State.Destroy;
        }
        */

        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        Cursor.Instance.usage = primaryUsage;
        switch(primaryUsage) {
            case "movement":
                if(Input.GetMouseButtonDown(0) && !MouseOverUI()) {
                    Vector2 target = new Vector2(point.x, point.y);
                    MoveTo(target);
                }
            break;

            case "plant":
                Cursor.Instance.SnapTo(point);
                if(Input.GetMouseButtonDown(0)) {
                    Cursor.Instance.Plant(plantable);
                }
            break;

            case "till":
                Cursor.Instance.SnapTo(point);
                if(Input.GetMouseButtonDown(0) && !MouseOverUI()) {
                    Cursor.Instance.Till();
                }
            break;

            case "uproot":
                Cursor.Instance.SnapTo(point);
                if(Input.GetMouseButtonDown(0)) {
                    Cursor.Instance.Destroy();
                }
            break;

            case "water":
                Cursor.Instance.SnapTo(point);
                if(Input.GetMouseButtonDown(0)) {
                    Cursor.Instance.Water();
                }
            break;
        }

        float mx = Input.GetAxis("Horizontal");
        float my = Input.GetAxis("Vertical");
        if(mx != 0f || my != 0f) {
            Vector2 target = new Vector2(transform.position.x, transform.position.y) + (new Vector2(mx, my).normalized * playerView.minMovement);
            MoveTo(target);
        }

        //if(Input.GetKeyDown(KeyCode.D)) {
        //    MoveTo(new Vector2(transform.position.x + 1f, transform.position.y));
        //}
    }

    void MoveTo(Vector2 target) {
        playerView.localDestination = target;
        //PlayerMoveToRequest moveTo = new PlayerMoveToRequest { destination = destination };
        
        //moveTo.HandleRequest();
        PlayerMoveToRequest moveTo = new PlayerMoveToRequest {
            networkId = identity.netId,
            destination = target 
        };

        moveTo.HandleRequest();
    }

    bool MouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void SetPrimaryUsage(string usage, Item item) {
        primaryUsage = usage;
        switch(primaryUsage) {
            case "till":
                Debug.Log("item set to " + item.name);
                // Set soil type here
                Cursor.Instance.spriteRenderer.enabled = true;
                Cursor.Instance.spriteRenderer.sprite = ItemSpriteAtlas.Instance.items.GetSprite(item.img);
            break;

            case "plant":
                Debug.Log("item set to " + item.name);
                plantable = item;
                Cursor.Instance.spriteRenderer.enabled = true;
                Cursor.Instance.spriteRenderer.sprite = ItemSpriteAtlas.Instance.items.GetSprite(item.img);
            break;
        }
    }
}
