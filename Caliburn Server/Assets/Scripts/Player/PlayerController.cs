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

        //Client.Instance.playerIdentity = identity;
        //InventoryView.Instance.RequestInventoryData();
    }

    bool MouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
