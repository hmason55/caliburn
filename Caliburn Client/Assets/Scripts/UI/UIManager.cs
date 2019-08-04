using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class UIManager : MonoSingleton<UIManager> {

    protected override void Init() {
        base.Init();
        DontDestroyOnLoad(this);
    }

    public void Logout() {
        Debug.Log("Logging out.");
        Client.singleton.OnApplicationQuit();
        DestroyImmediate(Client.singleton.gameObject);
        SceneManager.LoadScene("Main Menu");
    }
}
