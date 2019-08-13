using Mirror;
using UnityEngine;

public class GrowableDataRequest : MessageBase {
    public string ownerId;
    public uint networkId;
    public int creationDate;
    public int completionDate;

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
