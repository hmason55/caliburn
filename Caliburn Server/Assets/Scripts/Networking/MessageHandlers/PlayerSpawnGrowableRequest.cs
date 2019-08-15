using UnityEngine;
using Mirror;

public class PlayerSpawnGrowableRequest : MessageBase {
    public int uniqueId;
    public string ownerId;
    public string growableId;
    public Vector2 position;
    public int creationDate;
    public int completionDate;
    public int waterDate;
    public bool complete = false;


    public void HandleRequest() {
        if(!Growables.Instance.prefabsById.ContainsKey(growableId)) {complete = true; return;}
        
        GameObject go = NetworkManager.Instantiate(Growables.Instance.prefabsById[growableId], position, Quaternion.identity) as GameObject;
        PlantView plantView = go.GetComponent<PlantView>();

        //plantView.Validate((valid) => {
        //    if(valid) {
                NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                System.Guid prefabAssetId = identity.assetId;
                
                plantView.HandleRequest(this);
                ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnGrowable, OnUnSpawnGrowable);
                NetworkServer.Spawn(go, prefabAssetId);

                Server.Instance.growableDataByNetId.Add(identity.netId, CreateGrowable());
                complete = true;
         //   } else {
        //        NetworkManager.Destroy(go);
        //        complete = true;
        //    }
        //});
    }

    public void HandleRequestReceived() {
        if(!Growables.Instance.prefabsById.ContainsKey(growableId)) {return;}

        GameObject go = NetworkManager.Instantiate(Growables.Instance.prefabsById[growableId], position, Quaternion.identity) as GameObject;
        PlantView plantView = go.GetComponent<PlantView>();

        plantView.Validate((valid) => {
            if(valid) {
                
                creationDate = TimeUtility.CurrentUnixTimestamp();
                completionDate = creationDate + Growables.Instance.growablesById[growableId].duration;
                waterDate = creationDate - 1;

                ProcessGrowable.Instance.SpawnRequest(this, (requestCode) => {
                    Debug.Log("Request completed with a request code: " + requestCode);
                    switch(requestCode) {
                        case -1:
                            Debug.Log("Server offline.");
                        break;

                        case 0:
                            Debug.Log("Owner doesn't exist.");
                        break;

                        default:
                            NetworkManager.Destroy(go.GetComponent<Rigidbody2D>());
                            NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
                            System.Guid prefabAssetId = identity.assetId;
                            uniqueId = requestCode;
                            Debug.Log(uniqueId);
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
        GameObject go = NetworkManager.Instantiate(Growables.Instance.prefabsById[growableId], position, Quaternion.identity);
        NetworkIdentity identity = go.GetComponent<NetworkIdentity>();
        return go;
    }

    public void OnUnSpawnGrowable(GameObject spawned) {
        
        NetworkManager.Destroy(spawned);
    }

    GrowableData CreateGrowable() {
        return new GrowableData {
            uniqueId = uniqueId,
            ownerId = ownerId,
            growableId = growableId,
            posX = position.x,
            posY = position.y,
            creationDate = creationDate,
            completionDate = completionDate,
            waterDate = waterDate,
        };
    }
}
