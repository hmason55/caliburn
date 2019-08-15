using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ProcessGrowable : MonoSingleton<ProcessGrowable> {

    const string DB_SPAWN_USER_GROWABLE_URL = "http://localhost/caliburn/spawn_user_growable.php";
    const string DB_LOAD_USER_GROWABLES_URL = "http://localhost/caliburn/load_user_growables.php";
    const string DB_WATER_USER_GROWABLE_URL = "http://localhost/caliburn/water_user_growable.php";

    #region Spawn Growable
    public void SpawnRequest(PlayerSpawnGrowableRequest request, Action<int> onComplete = null) {
        StartCoroutine(SpawnGrowableRequest(request, onComplete));
    }
    
    IEnumerator SpawnGrowableRequest(PlayerSpawnGrowableRequest request, Action<int> onComplete = null) {
        
        Growable growable = Growables.Instance.growablesById[request.growableId];

        Dictionary<string, string> parameters = new Dictionary<string, string>(){ 
            { "owner_id", request.ownerId }, 
            { "growable_id", growable.growableId }, 
            { "pos_x", request.position.x.ToString() },
            { "pos_y", request.position.y.ToString() },
            { "creation_date", request.creationDate.ToString() },
            { "completion_date", request.completionDate.ToString() },
            { "water_date", request.waterDate.ToString() },
        };

        using(UnityWebRequest spawnRequest = UnityWebRequest.Post(DB_SPAWN_USER_GROWABLE_URL, parameters)) {
            yield return spawnRequest.SendWebRequest();
            
            int requestCode = -1;

            if (spawnRequest.isNetworkError) {
                Debug.LogWarning("Error: " + spawnRequest.error);
            } else {
                // Database server is up
                string result = spawnRequest.downloadHandler.text; // result
                bool validRequest = int.TryParse(result, out requestCode);

                if(validRequest) {
                    switch(requestCode) {
                        default:
                            Debug.Log("Spawn successful.");
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
    #endregion

    #region Load Growable
    public void LoadRequest(Action<int> onComplete = null) {
        StartCoroutine(LoadGrowablesRequest(onComplete));
    }

    IEnumerator LoadGrowablesRequest(Action<int> onComplete = null) {
        using(UnityWebRequest loadRequest = UnityWebRequest.Get(DB_LOAD_USER_GROWABLES_URL)) {
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
                uniqueId = data.uniqueId,
                ownerId = data.ownerId,
                growableId = data.growableId,
                position = new Vector2(data.posX, data.posY),
                creationDate = data.creationDate,
                completionDate = data.completionDate,
                waterDate = data.waterDate,
            };
            spawnRequest.HandleRequest();
            yield return new WaitForFixedUpdate();  // Wait for collision check
        }
        Debug.Log("Growables loaded successfully.");
        yield break;
    }
    #endregion

    #region Water Growable
    public void WaterRequest(WaterGrowableRequest request, Action<int> onComplete = null) {
        StartCoroutine(WaterGrowableRequest(request, onComplete));
    }

    IEnumerator WaterGrowableRequest(WaterGrowableRequest request, Action<int> onComplete = null) {

        Dictionary<string, string> parameters = new Dictionary<string, string>(){ 
            { "unique_id", request.uniqueId.ToString() }, 
            { "completion_date", request.completionDate.ToString() },
            { "water_date", request.waterDate.ToString() },
        };

        using(UnityWebRequest waterRequest = UnityWebRequest.Post(DB_WATER_USER_GROWABLE_URL, parameters)) {
            yield return waterRequest.SendWebRequest();
            
            int requestCode = 4;

            if(waterRequest.isNetworkError) {
                Debug.LogWarning("Error: " + waterRequest.error);
            } else {
                // Database server is up
                string result = waterRequest.downloadHandler.text; // result

                bool validRequest = int.TryParse(result, out requestCode);

                switch(requestCode) {
                    case 0:
                        Debug.Log("Growable watered.");
                    break;

                    case 1:
                        Debug.Log("Could not water this growable.");
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
    #endregion


}
