using UnityEngine;
using Mirror;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Server : NetworkManager {

    public static Server _instance;

    public static Server Instance {
        get {
            if(_instance == null) {throw new MissingReferenceException();}
            return _instance;
        }
    }

    public Dictionary<NetworkConnection, string> usernamesByConnection;
    public Dictionary<uint, PlayerData> playerDataByNetId;
    public Dictionary<uint, GrowableData> growableDataByNetId;
    public Dictionary<uint, SoilData> soilDataByNetId;
    public BoundsInt bounds;

    public RuleTile tile;

    public override void Awake() {
        _instance = this;
        base.Awake();
    }

    public override void Start() {

        Application.runInBackground = true;
        Initialize();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {

        }
    }

    GameObject OnSpawnTile(Vector3 position, System.Guid assetId) {
        return NetworkManager.Instantiate(Soils.Instance.soils[0], position, Quaternion.identity);
    }

    void OnUnSpawnTile(GameObject spawned) {

    }

    void Initialize() {
        usernamesByConnection = new Dictionary<NetworkConnection, string>();
        playerDataByNetId = new Dictionary<uint, PlayerData>();
        growableDataByNetId = new Dictionary<uint, GrowableData>();
        soilDataByNetId = new Dictionary<uint, SoilData>();
        NetworkServer.Listen(64);
        RegisterHandlers();
        SpawnGrowables();
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
        //NetworkServer.RegisterSpawn
    }  

    void SpawnSoil() {
        ProcessSoil.Instance.LoadRequest((requestCode) => {
            
            // Do other stuff after spawning soil.
        });
    }

    void SpawnGrowables() {


        ProcessGrowable.Instance.LoadRequest((requestCode) => {
            
            // Do other stuff after spawning growables.
        });
    }

    public void SpawnObject(GameObject prefab, Vector3 position) {
        GameObject go = Instantiate(prefab, position, Quaternion.identity) as GameObject;
        System.Guid prefabAssetId = go.GetComponent<NetworkIdentity>().assetId;
        ClientScene.RegisterSpawnHandler(prefabAssetId, OnSpawnPrefab, OnUnSpawnPrefab);
        NetworkServer.Spawn(go, prefabAssetId);
        //ProcessGrowable.Instance.SpawnRequest("username", 1, Vector2.zero, null);

        //if(NetworkIdentity.spawned.ContainsKey[identity.])
    }

    public GameObject OnSpawnPrefab(Vector3 position, System.Guid assetId) {
        return Instantiate(Growables.Instance.plants[0], position, Quaternion.identity);
    }

    public void OnUnSpawnPrefab(GameObject spawned) {
        Destroy(spawned);
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
        ProcessLogin.Instance.Request(netMsg.username, netMsg.password, (requestCode) => {
            netMsg.requestCode = requestCode;

            switch(requestCode) {
                case 0:
                    if(!usernamesByConnection.ContainsValue(netMsg.username)) {
                        netMsg.connectionId = connection.connectionId;
                        usernamesByConnection.Add(connection, netMsg.username);
                        Debug.Log(netMsg.username + " logged in.");
                        
                    } else {
                        netMsg.requestCode = 5;
                    }
                break;
            }
            
            Debug.Log("Login request completed with request code: " + requestCode);
            NetworkServer.SendToClient<UserLoginRequest>(connection.connectionId, netMsg);
        });
    }

    void OnSignupRequestReceived(NetworkConnection connection, UserSignupRequest netMsg) {
        ProcessSignup.Instance.Request(netMsg.username, netMsg.password, netMsg.email, (requestCode) => {
            Debug.Log("Signup request completed with request code: " + requestCode);
            netMsg.requestCode = requestCode;
            NetworkServer.SendToClient<UserSignupRequest>(connection.connectionId, netMsg);
        });
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
        Debug.Log("Message received");

        switch(netMsg.target) {
            case "World":
                NetworkServer.SendToAll<ChatMessage>(netMsg);
            break;

            default:
                //NetworkServer.SendToClient<ChatMessage>(netMsg);
            break;
        }
        
    }

    void OnPlayerMoveToRequestReceived(NetworkConnection connection, PlayerMoveToRequest netMsg) {
        if(!NetworkIdentity.spawned.ContainsKey(netMsg.networkId)) {return;}
        if(!playerDataByNetId.ContainsKey(netMsg.networkId)) {return;}
        netMsg.HandleRequestReceived(NetworkIdentity.spawned[netMsg.networkId]);
        //Debug.Log(netMsg.destination);
        playerDataByNetId[netMsg.networkId].destination = netMsg.destination;
    }

    void OnPlayerDataSyncMessageReceived(NetworkConnection connection, PlayerDataSyncMessage netMsg) {
        //Debug.Log("PlayerData sync request received.");
        //Debug.Log(playerDataByNetId.Count);
        foreach(KeyValuePair<uint, PlayerData>  playerData in playerDataByNetId) {
            
            PlayerView playerView = NetworkIdentity.spawned[playerData.Key].GetComponent<PlayerView>();

            PlayerDataSyncMessage playerDataMessage = new PlayerDataSyncMessage {
                networkId = playerData.Key,
                connectionId = playerData.Value.connectionId,
                username = playerData.Value.username,
                ipAddress = playerData.Value.ipAddress,
                position = (Vector2)playerView.transform.position,
                destination = playerData.Value.destination,
            };
            playerDataMessage.HandleRequestReceived(connection);
        }
    }

    public void OnPlayerSpawnGrowableRequestReceived(NetworkConnection connection, PlayerSpawnGrowableRequest netMsg) {
        if(!usernamesByConnection.ContainsKey(connection)) {return;}
        netMsg.ownerId = usernamesByConnection[connection];
        netMsg.HandleRequestReceived();
    }

    public void OnPlayerUnSpawnGrowableRequestReceived(NetworkConnection connection, PlayerUnSpawnGrowableRequest netMsg) {
        if(!usernamesByConnection.ContainsKey(connection)) {return;}
        netMsg.HandleRequestReceived();
    }

    public void OnGrowableDataRequestReceived(NetworkConnection connection, GrowableDataRequest netMsg) {
        if(!growableDataByNetId.ContainsKey(netMsg.networkId)) {Debug.Log("No such growable with netId: " + netMsg.networkId); return;}
        netMsg.HandleRequestReceived(connection);
    }

    public void OnPlayerInventorySyncRequestReceived(NetworkConnection connection, PlayerInventorySyncRequest netMsg) {
        if(!usernamesByConnection.ContainsKey(connection)) {return;}
        netMsg.HandleRequestReceived(connection);
    }

    public void OnPlayerSpawnSoilRequestReceived(NetworkConnection connection, PlayerSpawnSoilRequest netMsg) {
        Debug.Log(netMsg.position);
        netMsg.HandleRequestReceived();
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
