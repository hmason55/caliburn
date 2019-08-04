using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoSingleton<PlayerInfo> {

    protected override void Init() {
        base.Init();

        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject playerPrefab;
    public string PlayerName;
    public string ipAddress;
    
    
}
