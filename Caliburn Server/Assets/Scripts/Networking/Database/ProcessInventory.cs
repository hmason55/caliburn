using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Mirror;

public class ProcessInventory : MonoSingleton<ProcessInventory> {

    const string DB_LOAD_INVENTORY_URL = "http://localhost/caliburn/load_inventory.php";

    public void LoadRequest(NetworkConnection connection, PlayerInventorySyncRequest request, Action<int> onComplete = null) {
        StartCoroutine(LoadInventoryRequest(connection, request, onComplete));
    }

    IEnumerator LoadInventoryRequest(NetworkConnection connection, PlayerInventorySyncRequest request, Action<int> onComplete = null) {
        Dictionary<string, string> parameters = new Dictionary<string, string>(){ { "uid", Server.Instance.usernamesByConnection[connection] }};

        using(UnityWebRequest loadRequest = UnityWebRequest.Post(DB_LOAD_INVENTORY_URL, parameters)) {
            yield return loadRequest.SendWebRequest();
            
            int requestCode = 4;

            if(loadRequest.isNetworkError) {
                Debug.LogWarning("Error: " + loadRequest.error);
            } else {
                // Database server is up
                string result = loadRequest.downloadHandler.text; // result
                Debug.Log(result);

                InventoryDataPack dataPack = (InventoryDataPack)JsonConvert.DeserializeObject(result, typeof(InventoryDataPack));
                requestCode = dataPack.result;

                switch(requestCode) {
                    case 0:
                        ProcessDataPack(connection, request, dataPack);
                    break;

                    case 1:
                        Debug.Log("No growables to load.");
                    break;

                    default:
                        Debug.Log("Server offline.");
                        break;
                }  
            }

            if(onComplete != null) {
                onComplete(requestCode);
            }
        }
        yield break;
    }

    void ProcessDataPack(NetworkConnection connection, PlayerInventorySyncRequest request, InventoryDataPack dataPack) {
        Debug.Log("Processing items...");

        // Begin inventory sync.
        request.requestCode = 0;
        request.HandleRequest(connection);

        // Send items to client.
        foreach(Item item in dataPack.items) {
            PlayerItemSyncMessage inventorySyncMessage = new PlayerItemSyncMessage {
                networkId = request.networkId,
                itemId = item.id,
                name = item.name,
                value = item.value,
                quantity = item.quantity,
                stackable = item.stackable,
                img = item.img,
                storageType = item.storageType,
                primaryUsage = item.primaryUsage,
            };
            inventorySyncMessage.HandleRequest(connection);
        }

        // End inventory sync.
        request.requestCode = 1;
        request.HandleRequest(connection);


        Debug.Log("Items loaded successfully.");
    }
}
