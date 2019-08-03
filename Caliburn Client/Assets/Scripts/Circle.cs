using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Circle : NetworkBehaviour {

    [Command]
    void CmdPickUp() {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        CmdPickUp();
    }
}
