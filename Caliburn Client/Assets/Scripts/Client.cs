using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Client : NetworkManagerSingleton<Client> {
    /*public static Client _instance;

    public static Client Instance {
        get {
            if(_instance == null) {throw new MissingReferenceException();}
            return _instance;
        }
    }*/

    public LoginManager loginManager;
    public NetworkIdentity playerIdentity;

    //public override void Awake() {
     //   base.Awake();
    //}
    //public override void Awake() {
        //_instance = this;
     //   base.Awake();
    //}

    public override void Start() {
        Application.runInBackground = true;
        StartCoroutine(AttemptConnection());
    }

    IEnumerator AttemptConnection() {
        int attempt = 0;
        int maxAttempts = 3;
        float attemptDelay = 1.0f;
        Connect();

        while(!NetworkClient.isConnected && attempt < maxAttempts) {
            attempt++;
            yield return new WaitForSeconds(attemptDelay);
        }

        if(!NetworkClient.isConnected) {
            Debug.Log("Could not connect to server.");
            DestroyImmediate(gameObject);
            SceneManager.LoadScene("Main Menu");
        }
        
        yield break;
    }

    void Connect() {
        Debug.Log("Connecting...");
        NetworkClient.Connect("localhost");
        RegisterHandlers();
    }

    void RegisterHandlers() {
        NetworkClient.RegisterHandler<ConnectMessage>(OnClientConnected);
        NetworkClient.RegisterHandler<DisconnectMessage>(OnClientDisconnected);
        NetworkClient.RegisterHandler<UserLoginRequest>(OnLoginRequestReceived);
        NetworkClient.RegisterHandler<UserSignupRequest>(OnSignupRequestReceived);
        NetworkClient.RegisterHandler<ChatMessage>(OnChatMessageReceived);
        NetworkClient.RegisterHandler<PlayerMoveToRequest>(OnPlayerMoveToRequestReceived);
        NetworkClient.RegisterHandler<PlayerPositionMessage>(OnPlayerPositionMessageReceived);
        NetworkClient.RegisterHandler<PlayerNameplateMessage>(OnPlayerNameplateMessageReceived);
        NetworkClient.RegisterHandler<PlayerDataSyncMessage>(OnPlayerDataSyncMessageReceived);
        NetworkClient.RegisterHandler<GrowableDataRequest>(OnGrowableDataRequestReceived);
        NetworkClient.RegisterHandler<PlayerInventorySyncRequest>(OnPlayerInventorySyncRequestReceived);
        NetworkClient.RegisterHandler<PlayerItemSyncMessage>(OnPlayerItemSyncMessageReceived);
        NetworkClient.RegisterHandler<SpawnSoilMessage>(OnSpawnSoilMessageReceived);
        NetworkClient.RegisterHandler<SoilDataRequest>(OnSoilDataRequestReceived);
        //NetworkClient.RegisterHandler<PlayerNameplateSyncRequest>;
        //NetworkClient.RegisterHandler<SpawnPrefabMessage>(OnSpawnPrefabMessageReceived);
    }

    void OnClientConnected(NetworkConnection connection, ConnectMessage netMsg) {
        Debug.Log("Connected to server.");
        LoginManager.Instance.OnShowButtonGroup();
    }

    void OnClientDisconnected(NetworkConnection connection, DisconnectMessage netMsg) {
        Debug.Log("Disconnected from server.");
        DestroyImmediate(this.gameObject);
        SceneManager.LoadScene("Main Menu");
    }

    void OnLoginRequestReceived(NetworkConnection connection, UserLoginRequest netMsg) {
        if(connection.connectionId == 0) {
            NetworkClient.connection.connectionId = netMsg.connectionId;
        }

        Debug.Log("Received reply from server with request code: " + netMsg.requestCode);
        switch(netMsg.requestCode) {
            case 0:
                Debug.Log("Login success.");
                ClientScene.RegisterPrefab(PlayerInfo.Instance.playerPrefab);
                PlayerInfo.Instance.PlayerName = netMsg.username;
                PlayerInfo.Instance.ipAddress = NetworkClient.connection.address;
                ClientScene.AddPlayer(NetworkClient.connection);
                
                SceneManager.LoadScene("World");

                PlayerDataSyncMessage playerDataSync = new PlayerDataSyncMessage {};
                playerDataSync.HandleRequest(connection);
                Debug.Log("Requesting player data...");
            break;

            case 1:
                Debug.Log("Invalid username or password.");
            break;

            case 5:
                Debug.Log("This account is currently in use.");
            break;

            default:
                Debug.Log("Could not connect to server.");
            break;
        }

        loginManager.OnLoginComplete();
    }

    void OnSignupRequestReceived(NetworkConnection connection, UserSignupRequest netMsg) {
        Debug.Log("Received reply from server with request code: " + netMsg.requestCode);

        switch(netMsg.requestCode) {
            case 0:
                Debug.Log("Signup success.");
            break;

            case 1:
                Debug.Log("Invalid username or password.");
            break;

            default:
                Debug.Log("Could not connect to server.");
            break;
        }

        loginManager.OnSignupComplete();
    }

    void OnChatMessageReceived(NetworkConnection connection, ChatMessage netMsg) {
        netMsg.HandleMessageReceived(connection);
    }

    void OnPlayerMoveToRequestReceived(NetworkConnection connection, PlayerMoveToRequest netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived(NetworkIdentity.spawned[netMsg.networkId]);
    }

    void OnPlayerPositionMessageReceived(NetworkConnection connection, PlayerPositionMessage netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived(NetworkIdentity.spawned[netMsg.networkId]);
    }

    void OnPlayerNameplateMessageReceived(NetworkConnection connection, PlayerNameplateMessage netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived(NetworkIdentity.spawned[netMsg.networkId]);
    }
    
    void OnPlayerDataSyncMessageReceived(NetworkConnection connection, PlayerDataSyncMessage netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived();
    }

    void OnGrowableDataRequestReceived(NetworkConnection connection, GrowableDataRequest netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived();
    }

    void OnPlayerInventorySyncRequestReceived(NetworkConnection connection, PlayerInventorySyncRequest netMsg) {
        netMsg.HandleRequestReceived();
    }

    void OnPlayerItemSyncMessageReceived(NetworkConnection connection, PlayerItemSyncMessage netMsg) {
        netMsg.HandleRequestReceived();
    }

    void OnSpawnSoilMessageReceived(NetworkConnection connection, SpawnSoilMessage netMsg) {
        netMsg.HandleRequestReceived();
    }

    void OnSoilDataRequestReceived(NetworkConnection connection, SoilDataRequest netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived();
    }

    void OnSpawnGrowable() {
        Debug.Log("Spawned!");
    }

    #region Utility
    byte[] StringToBytes(string str) {
        return Encoding.UTF8.GetBytes(str);
    }

    string BytesToString(byte[] bytes) {
        return Encoding.UTF8.GetString(bytes);
    }
    #endregion
}


