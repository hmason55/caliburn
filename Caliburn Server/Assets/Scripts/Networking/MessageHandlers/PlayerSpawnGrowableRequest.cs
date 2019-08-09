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
        if(growableId >= Growables.Instance.plants.Count) {complete = true; return;}

        GameObject go = NetworkManager.Instantiate(Growables.Instance.plants[growableId], position, Quaternion.identity) as GameObject;
        PlantView plantView = go.GetComponent<PlantView>();

        plantView.Validate((valid) => {
            if(valid) {
                NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                System.Guid prefabAssetId = identity.assetId;
                
                plantView.HandleRequest(this);
                ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnGrowable, OnUnSpawnGrowable);
                NetworkServer.Spawn(go, prefabAssetId);

                Server.Instance.growableDataByNetId.Add(identity.netId, CreateGrowable());
                complete = true;
            } else {
                NetworkManager.Destroy(go);
                complete = true;
            }
        });
    }

    public void HandleRequestReceived() {
        if(growableId >= Growables.Instance.plants.Count) {return;}

        GameObject go = NetworkManager.Instantiate(Growables.Instance.plants[growableId], position, Quaternion.identity) as GameObject;
        PlantView plantView = go.GetComponent<PlantView>();

        plantView.Validate((valid) => {
            if(valid) {

                creationDate = TimeUtility.CurrentUnixTimestamp();
                completionDate = creationDate + Growables.Instance.growableData[growableId].duration;

                ProcessGrowable.Instance.SpawnRequest(this, (requestCode) => {
                    Debug.Log("Request completed with a request code: " + requestCode);
                    switch(requestCode) {
                        case 0:
                            NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                            NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                            System.Guid prefabAssetId = identity.assetId;
                            
                            plantView.HandleRequest(this);
                            ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnGrowable, OnUnSpawnGrowable);
                            NetworkServer.Spawn(go, prefabAssetId);

                            Server.Instance.growableDataByNetId.Add(identity.netId, CreateGrowable());
                        break;
                    }
                });
            } else {
                NetworkManager.Destroy(go);
            }
        });
    }
    
    public GameObject OnSpawnGrowable(Vector3 position, System.Guid assetId) {
        GameObject go = NetworkManager.Instantiate(Growables.Instance.plants[growableId], position, Quaternion.identity);
        NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
        return go;
    }

    public void OnUnSpawnGrowable(GameObject spawned) {
        
        NetworkManager.Destroy(spawned);
    }

    GrowableData CreateGrowable() {
        return new GrowableData {
            ownerId = ownerId,
            growableId = growableId,
            posX = position.x,
            posY = position.y,
            creationDate = creationDate,
            completionDate = completionDate
        };
    }
}
