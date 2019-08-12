using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProcessSignup : MonoSingleton<ProcessSignup> {

    const string DB_SIGNUP_URL = "http://localhost/caliburn/signup.php";

    public void Request(string username, string password, string email, Action<int> onComplete = null) {
        StartCoroutine(SignupRequest(username, password, email, onComplete));
    }
    
    IEnumerator SignupRequest(string username, string password, string email, Action<int> onComplete = null) {

        Dictionary<string, string> parameters = new Dictionary<string, string>(){ { "uid", username }, { "password", password }, { "email", email } };

        using(UnityWebRequest signupRequest = UnityWebRequest.Post(DB_SIGNUP_URL, parameters)) {
            yield return signupRequest.SendWebRequest();
            
            int requestCode = 4;

            if (signupRequest.isNetworkError) {
                Debug.LogWarning("Error: " + signupRequest.error);
            } else {
                // Database server is up
                string result = signupRequest.downloadHandler.text;
                bool validRequest = int.TryParse(result, out requestCode);

                if(validRequest) {
                    switch(requestCode) {
                        case 0:
                            Debug.Log("Signup successful.");
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
