using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaterGrowableRequest : MessageBase {
    
    public uint networkId;      // network id of the growable
    public int uniqueId;        // datebase id of user_growable
    public int completionDate;
    public int waterDate;

    public void HandleRequest() {
        Debug.Log("Sending watering request...");
        NetworkClient.Send<WaterGrowableRequest>(this);
    }  
}
