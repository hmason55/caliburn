using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : MonoSingleton<PlayerController> {
    void Awake() {
        //if(!GetComponent<NetworkIdentity>().isLocalPlayer) { Destroy(this); }
    }

    void Update() {
        //if(Input.GetKeyDown(KeyCode.D)) {
        //    MoveTo(new Vector2(transform.position.x + 1f, transform.position.y));
        //}
    }

    void MoveTo(Vector2 destination) {
        //PlayerMoveToRequest moveTo = new PlayerMoveToRequest { destination = destination };
        
        //moveTo.HandleRequest();
    }
}
