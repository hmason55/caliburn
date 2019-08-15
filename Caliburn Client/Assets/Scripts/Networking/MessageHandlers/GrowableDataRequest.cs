using Mirror;
using UnityEngine;

public class GrowableDataRequest : MessageBase {
    public uint networkId;
    public int uniqueId;
    public string ownerId;
    public string growableId;
    public Vector2 position;
    public int creationDate;
    public int completionDate;
    public int waterDate;

    public void HandleRequest() {
        NetworkClient.Send<GrowableDataRequest>(this);
    }

    public void HandleRequestReceived() {
        NetworkIdentity identity = NetworkIdentity.spawned[networkId];

        PlantView plantView = identity.GetComponent<PlantView>();
        if(plantView == null) {return;}
        plantView.UpdateGrowableData(this);
    }
}
