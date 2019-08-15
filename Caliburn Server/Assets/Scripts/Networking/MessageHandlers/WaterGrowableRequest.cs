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
        //NetworkClient.Send(this);
    }

    public void HandleRequestReceived(NetworkConnection connection) {
        if(!Server.Instance.growableDataByNetId.ContainsKey(networkId)) {
            Debug.Log("No such growable with networkId: " + networkId);
            return;
        }

        // Get the growable data
        GrowableData growableData = Server.Instance.growableDataByNetId[networkId];

        if(!Growables.Instance.growablesById.ContainsKey(growableData.growableId)) {
            Debug.Log("No such growable with growableId: " + growableData.growableId);
            return;
        }

        // get the growable
        Growable growable = Growables.Instance.growablesById[growableData.growableId];

        int currentTime = TimeUtility.CurrentUnixTimestamp();

        if(currentTime > growableData.completionDate) {
            Debug.Log("This growable is already complete");
            return;
        }
        if(currentTime <= growableData.waterDate) {
            Debug.Log("This growable isn't ready to be watered yet.");
            return;
        }

        // Calculate what growth stage the growable is in.
        int currentStage = (int)( 
            (currentTime - growableData.creationDate) / ( 
                (growableData.completionDate - growableData.creationDate) / (
                    growable.growthStages
                )
            ) 
        ) + 1;
        
        // Reduce the completion time.
        growableData.completionDate -= (int)(
            (growableData.completionDate - currentTime) * 0.20 // <- Watering modifier ( reduces the total remaining time by 20% )
        );


        // Set the next watering date to the beginning of the next growth stage.
        growableData.waterDate = growableData.creationDate + (
            (growableData.completionDate - growableData.creationDate) /
                growable.growthStages    
        ) * currentStage;
        
        // Assign completion date and next water date.
        uniqueId = growableData.uniqueId;
        completionDate = growableData.completionDate;
        waterDate = growableData.waterDate;

        // Request database update.
        ProcessGrowable.Instance.WaterRequest(this, (requestCode) => {
            GrowableDataRequest dataRequest = new GrowableDataRequest {
                networkId = networkId,
                uniqueId = growableData.uniqueId,
                ownerId = growableData.ownerId,
                growableId = growable.growableId,
                position = new Vector2(growableData.posX, growableData.posY),
                creationDate = growableData.creationDate,
                completionDate = completionDate,
                waterDate = waterDate
            };
            //Debug.Log("Sending watering data...");
            dataRequest.HandleRequest();
        });
    }
}
