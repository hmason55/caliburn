using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProcessLogin : MonoSingleton<ProcessLogin> {
    
    const string DB_USER_LOGIN_URL = "http://localhost/caliburn/user_login.php";

    public void Request(string username, string password, Action<int> onComplete = null) {
        StartCoroutine(LoginRequest(username, password, onComplete));
    }
    
    IEnumerator LoginRequest(string username, string password, Action<int> onComplete = null) {

        Dictionary<string, string> parameters = new Dictionary<string, string>(){ { "unique_id", username }, { "password", password } };

        using(UnityWebRequest loginRequest = UnityWebRequest.Post(DB_USER_LOGIN_URL, parameters)) {
            yield return loginRequest.SendWebRequest();
            
            int requestCode = 4;

            if (loginRequest.isNetworkError) {
                Debug.LogWarning("Error: " + loginRequest.error);
                Debug.Log("Login server offline.");
            } else {
                // Database server is up
                string result = loginRequest.downloadHandler.text;
                bool validRequest = int.TryParse(result, out requestCode);

                if(validRequest) {
                    switch(requestCode) {
                        case 0:
                            Debug.Log("Login successful.");
                        break;

                        case 1:
                            Debug.Log("Invalid username or password.");
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
}
