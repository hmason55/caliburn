using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerUnSpawnGrowableRequest : MessageBase {
    public string ownerId;
    public uint networkId;

    public void HandleRequest() {
        
    }

    public void HandleRequestReceived() {
        Debug.Log("request received");
        if(!NetworkIdentity.spawned.ContainsKey(networkId)) {return;}

        PlantView plantView = NetworkIdentity.spawned[networkId].GetComponent<PlantView>();
        if(plantView == null) {return;}
        Debug.Log("plant view exists");
        Debug.Log(plantView.ownerId + ", " + ownerId);
        if(plantView.ownerId == ownerId) {
            Debug.Log("destroying...");
            NetworkManager.Destroy(plantView.gameObject);
        }
    }

}
