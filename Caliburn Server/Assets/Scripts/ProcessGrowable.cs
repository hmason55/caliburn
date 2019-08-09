using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ProcessGrowable : MonoSingleton<ProcessGrowable> {

    const string DB_SPAWN_GROWABLE_URL = "http://localhost/caliburn/spawn_growable.php";
    const string DB_LOAD_GROWABLES_URL = "http://localhost/caliburn/load_growables.php";

    
    public void SpawnRequest(PlayerSpawnGrowableRequest request, Action<int> onComplete = null) {
        StartCoroutine(SpawnGrowableRequest(request, onComplete));
    }
    
    IEnumerator SpawnGrowableRequest(PlayerSpawnGrowableRequest request, Action<int> onComplete = null) {
        
        Growable growable = Growables.Instance.growableData[request.growableId];
        Debug.Log(request.ownerId);
        Debug.Log(growable.growableId.ToString());
        Debug.Log(request.position.x.ToString());
        Debug.Log(request.position.y.ToString());
        Debug.Log(request.creationDate.ToString());
        Debug.Log(request.completionDate.ToString());
        Dictionary<string, string> parameters = new Dictionary<string, string>(){ 
            { "owner_uid", request.ownerId }, 
            { "growable_id", growable.growableId.ToString() }, 
            { "pos_x", request.position.x.ToString() },
            { "pos_y", request.position.y.ToString() },
            { "creation_date", request.creationDate.ToString() },
            { "completion_date", request.completionDate.ToString() },
        };

        using(UnityWebRequest spawnRequest = UnityWebRequest.Post(DB_SPAWN_GROWABLE_URL, parameters)) {
            yield return spawnRequest.SendWebRequest();
            
            int requestCode = 4;

            if (spawnRequest.isNetworkError) {
                Debug.LogWarning("Error: " + spawnRequest.error);
            } else {
                // Database server is up
                string result = spawnRequest.downloadHandler.text; // result
                Debug.Log(result);
                bool validRequest = int.TryParse(result, out requestCode);

                if(validRequest) {
                    switch(requestCode) {
                        case 0:
                            Debug.Log("Spawn successful.");
                        break;

                        case 1:
                            Debug.Log("Owner doesn't exist.");
                        break;

                        default:
                            Debug.Log("Server offline.");
                            break;
                    }
                }
            }

            if(onComplete != null) {
                onComplete(requestCode);
            }
        }
        yield break;
    }

    public void LoadRequest(Action<int> onComplete = null) {
        StartCoroutine(LoadGrowablesRequest(onComplete));
    }

    IEnumerator LoadGrowablesRequest(Action<int> onComplete = null) {
        using(UnityWebRequest loadRequest = UnityWebRequest.Get(DB_LOAD_GROWABLES_URL)) {
            yield return loadRequest.SendWebRequest();
            
            int requestCode = 4;

            if(loadRequest.isNetworkError) {
                Debug.LogWarning("Error: " + loadRequest.error);
            } else {
                // Database server is up
                string result = loadRequest.downloadHandler.text; // result
                Debug.Log(result);

                GrowableDataPack dataPack = (GrowableDataPack)JsonConvert.DeserializeObject(result, typeof(GrowableDataPack));
                requestCode = dataPack.result;

                switch(requestCode) {
                    case 0:
                        StartCoroutine(ProcessDataPack(dataPack));
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

    IEnumerator ProcessDataPack(GrowableDataPack dataPack) {
        Debug.Log("Processing growables...");
        foreach(GrowableData data in dataPack.growables) {
            PlayerSpawnGrowableRequest spawnRequest = new PlayerSpawnGrowableRequest {
                ownerId = data.ownerId,
                growableId = data.growableId,
                position = new Vector2(data.posX, data.posY),
                creationDate = data.creationDate,
                completionDate = data.completionDate
            };
            spawnRequest.HandleRequest();
            yield return new WaitForFixedUpdate();  // Wait for collision check
        }
        Debug.Log("Growables loaded successfully.");
        yield break;
    }
}
