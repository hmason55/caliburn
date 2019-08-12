using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ProcessSoil : MonoSingleton<ProcessSoil> {
    const string DB_LOAD_SOIL_URL = "http://localhost/caliburn/load_soil.php";

    public void LoadRequest(Action<int> onComplete = null) {
        StartCoroutine(LoadSoilRequest(onComplete));
    }

    IEnumerator LoadSoilRequest(Action<int> onComplete = null) {
        using(UnityWebRequest loadRequest = UnityWebRequest.Get(DB_LOAD_SOIL_URL)) {
            yield return loadRequest.SendWebRequest();
            
            int requestCode = 4;

            if(loadRequest.isNetworkError) {
                Debug.LogWarning("Error: " + loadRequest.error);
            } else {
                // Database server is up
                string result = loadRequest.downloadHandler.text; // result
                Debug.Log(result);

                SoilDataPack dataPack = (SoilDataPack)JsonConvert.DeserializeObject(result, typeof(SoilDataPack));
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

    IEnumerator ProcessDataPack(SoilDataPack dataPack) {
        Debug.Log("Processing soil...");
        foreach(SoilData data in dataPack.soil) {
            PlayerSpawnSoilRequest spawnRequest = new PlayerSpawnSoilRequest {
                position = new Vector2(data.posX, data.posY),
            };
            spawnRequest.HandleRequest();
            yield return new WaitForFixedUpdate();  // Wait for collision check
        }
        Debug.Log("Soil loaded successfully.");
        yield break;
    }
}
