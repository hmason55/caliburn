using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerWaterGrowableRequest : MessageBase {
    public uint networkId;
    public int growableId; // This is the unique id on the database.

    void HandleRequestReceived() {
        // server side, 
    }
}
