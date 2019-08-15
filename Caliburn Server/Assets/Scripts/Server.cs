using UnityEngine;
using Mirror;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Server : NetworkManagerSingleton<Server> {

    /*public static Server _instance;

    public static Server Instance {
        get {
            if(_instance == null) {throw new MissingReferenceException();}
            return _instance;
        }
    }*/

    public Dictionary<NetworkConnection, string> usernamesByConnection;
    public Dictionary<uint, PlayerData> playerDataByNetId;
    public Dictionary<uint, GrowableData> growableDataByNetId;
    public Dictionary<uint, SoilData> soilDataByNetId;

    //public override void Awake() {
     //   base.Awake();
    //}

    public override void Start() {

        Application.runInBackground = true;
        Initialize();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {

        }
    }

    void Initialize() {
        usernamesByConnection = new Dictionary<NetworkConnection, string>();
        playerDataByNetId = new Dictionary<uint, PlayerData>();
        growableDataByNetId = new Dictionary<uint, GrowableData>();
        soilDataByNetId = new Dictionary<uint, SoilData>();
        SpawnSoils(); // onComplete -> SpawnGrowables

        NetworkServer.Listen(64);
        RegisterHandlers();
    } 

    void RegisterHandlers() {
        NetworkServer.RegisterHandler<ConnectMessage>(OnClientConnected);
        NetworkServer.RegisterHandler<DisconnectMessage>(OnClientDisconnected);
        NetworkServer.RegisterHandler<UserLoginRequest>(OnLoginRequestReceived);
        NetworkServer.RegisterHandler<UserSignupRequest>(OnSignupRequestReceived);
        NetworkServer.RegisterHandler<AddPlayerMessage>(OnServerAddPlayer);
        NetworkServer.RegisterHandler<ChatMessage>(OnChatMessageReceived);
        NetworkServer.RegisterHandler<PlayerMoveToRequest>(OnPlayerMoveToRequestReceived);
        NetworkServer.RegisterHandler<PlayerDataSyncMessage>(OnPlayerDataSyncMessageReceived);
        NetworkServer.RegisterHandler<PlayerSpawnGrowableRequest>(OnPlayerSpawnGrowableRequestReceived);
        NetworkServer.RegisterHandler<PlayerUnSpawnGrowableRequest>(OnPlayerUnSpawnGrowableRequestReceived);
        NetworkServer.RegisterHandler<GrowableDataRequest>(OnGrowableDataRequestReceived);
        NetworkServer.RegisterHandler<PlayerInventorySyncRequest>(OnPlayerInventorySyncRequestReceived);
        NetworkServer.RegisterHandler<PlayerSpawnSoilRequest>(OnPlayerSpawnSoilRequestReceived);
        NetworkServer.RegisterHandler<SoilDataRequest>(OnSoilDataRequestReceived);
        NetworkServer.RegisterHandler<WaterGrowableRequest>(OnWaterGrowableRequestReceived);
    }  

    void SpawnSoils() {
        ProcessSoil.Instance.LoadRequest((requestCode) => {
            
            // Do other stuff after spawning soil.
            SpawnGrowables();
        });
    }

    void SpawnGrowables() {
        ProcessGrowable.Instance.LoadRequest((requestCode) => {
            
            // Do other stuff after spawning growables.
        });
    }

    void OnClientConnected(NetworkConnection connection, ConnectMessage netMsg) {
        Debug.Log("Client connected to server");
    }

    void OnClientDisconnected(NetworkConnection connection, DisconnectMessage netMsg) {
        Debug.Log("Client disconnected from server");
        if(usernamesByConnection.ContainsKey(connection)) {
            uint netId = connection.playerController.netId;
            playerDataByNetId.Remove(netId);

            NetworkServer.DestroyPlayerForConnection(connection);
            Debug.Log(usernamesByConnection[connection] + " logged out.");
            usernamesByConnection.Remove(connection);
        }
    }

    void OnLoginRequestReceived(NetworkConnection connection, UserLoginRequest netMsg) {
        netMsg.HandleRequestReceived(connection);
    }

    void OnSignupRequestReceived(NetworkConnection connection, UserSignupRequest netMsg) {
        netMsg.HandleRequestReceived(connection);
    }

    public override void OnServerAddPlayer(NetworkConnection connection, AddPlayerMessage netMsg) {

        GameObject player = Instantiate(playerPrefab) as GameObject;
        player.transform.position = new Vector3(Random.Range(-3f, 3f) , 0f, 0f);
        NetworkServer.AddPlayerForConnection(connection, player);

        PlayerData playerData = new PlayerData {
            networkId = connection.playerController.netId,
            connectionId = connection.connectionId,
            username = usernamesByConnection[connection],
            ipAddress = "",
            position = (Vector2)player.transform.position,
            destination = (Vector2)player.transform.position
        };

        PlayerView playerView = player.GetComponent<PlayerView>();
        playerView.playerData = playerData;
        playerDataByNetId.Add(playerData.networkId, playerData);
        
        PlayerDataSyncMessage playerDataMessage = new PlayerDataSyncMessage {
            networkId = playerData.networkId,
            connectionId = playerData.connectionId,
            username = playerData.username,
            ipAddress = playerData.ipAddress,
            position = playerData.position,
            destination = playerData.destination
        };

        playerDataMessage.HandleRequest();
    }

    void OnChatMessageReceived(NetworkConnection connection, ChatMessage netMsg) {
        netMsg.HandleRequestReceived();
    }

    void OnPlayerMoveToRequestReceived(NetworkConnection connection, PlayerMoveToRequest netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        if(!playerDataByNetId.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived(NetworkIdentity.spawned[netMsg.networkId]);
    }

    void OnPlayerDataSyncMessageReceived(NetworkConnection connection, PlayerDataSyncMessage netMsg) {
        netMsg.HandleRequestReceived(connection);
    }

    void OnPlayerSpawnGrowableRequestReceived(NetworkConnection connection, PlayerSpawnGrowableRequest netMsg) {
        if(!usernamesByConnection.ContainsKey(connection)) {return;}
        netMsg.ownerId = usernamesByConnection[connection];
        netMsg.HandleRequestReceived();
    }

    void OnPlayerUnSpawnGrowableRequestReceived(NetworkConnection connection, PlayerUnSpawnGrowableRequest netMsg) {
        if(!usernamesByConnection.ContainsKey(connection)) {return;}
        netMsg.HandleRequestReceived();
    }

    void OnGrowableDataRequestReceived(NetworkConnection connection, GrowableDataRequest netMsg) {
        if(!growableDataByNetId.ContainsKey(netMsg.networkId)) {Debug.Log("No such growable with netId: " + netMsg.networkId); return;}
        netMsg.HandleRequestReceived(connection);
    }

    void OnPlayerInventorySyncRequestReceived(NetworkConnection connection, PlayerInventorySyncRequest netMsg) {
        if(!usernamesByConnection.ContainsKey(connection)) {return;}
        netMsg.HandleRequestReceived(connection);
    }

    void OnPlayerSpawnSoilRequestReceived(NetworkConnection connection, PlayerSpawnSoilRequest netMsg) {
        Debug.Log(netMsg.position);
        netMsg.HandleRequestReceived();
    }

    void OnSoilDataRequestReceived(NetworkConnection connection, SoilDataRequest netMsg) {
        if(!soilDataByNetId.ContainsKey(netMsg.networkId)) {Debug.Log("No such soil with netId: " + netMsg.networkId); return;}
        netMsg.HandleRequestReceived(connection);
    }

    void OnWaterGrowableRequestReceived(NetworkConnection connection, WaterGrowableRequest netMsg) {
        netMsg.HandleRequestReceived(connection);
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
