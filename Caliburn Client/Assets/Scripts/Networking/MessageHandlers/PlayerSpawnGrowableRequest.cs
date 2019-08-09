using UnityEngine;
using Mirror;

public class PlayerSpawnGrowableRequest : MessageBase {
    public string ownerId;
    public int growableId;
    public Vector2 position;
    public int creationDate;
    public int completionDate;
    public bool complete = false;
    
    public void HandleRequest() {
        NetworkClient.Send<PlayerSpawnGrowableRequest>(this);
    }

    public void HandleRequestReceived() {
        //if(growableId >= Growables.Instance.plants.Count) {return;}

        //GameObject go = NetworkManager.Instantiate(Growables.Instance.plants[growableId], position, Quaternion.identity) as GameObject;
        //PlantView plantView = go.GetComponent<PlantView>();
        //if(!plantView.isValid) {return;}

        //System.Guid prefabAssetId = go.GetComponent<NetworkIdentity>().assetId;
        //ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnGrowable, OnUnSpawnGrowable);
        //NetworkServer.Spawn(go, prefabAssetId);
    }
}
