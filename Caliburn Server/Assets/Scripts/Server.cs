using UnityEngine;
using Mirror;
using System.Text;
using System.Collections.Generic;

public class Server : NetworkManager {

    public static Server _instance;

    public static Server Instance {
        get {
            if(_instance == null) {throw new MissingReferenceException();}
            return _instance;
        }
    }

    public Dictionary<NetworkConnection, string> connectedPlayers;
    public Dictionary<uint, NetworkIdentity> playerIds;

    public override void Start() {
        Application.runInBackground = true;
        Initialize();
    }

    void Initialize() {
        connectedPlayers = new Dictionary<NetworkConnection, string>();
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
    }

    void OnClientConnected(NetworkConnection connection, ConnectMessage netMsg) {
        Debug.Log("Client connected to server");
    }

    void OnClientDisconnected(NetworkConnection connection, DisconnectMessage netMsg) {
        Debug.Log("Client disconnected from server");
        if(connectedPlayers.ContainsKey(connection)) {
            NetworkServer.DestroyPlayerForConnection(connection);
            Debug.Log(connectedPlayers[connection] + " logged out.");
            connectedPlayers.Remove(connection);
        }
    }

    void OnLoginRequestReceived(NetworkConnection connection, UserLoginRequest netMsg) {
        ProcessLogin.Instance.Request(netMsg.username, netMsg.password, (requestCode) => {
            netMsg.requestCode = requestCode;

            switch(requestCode) {
                case 0:
                    if(!connectedPlayers.ContainsValue(netMsg.username)) {
                        netMsg.connectionId = connection.connectionId;
                        connectedPlayers.Add(connection, netMsg.username);
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

        PlayerView playerView = player.GetComponent<PlayerView>();
        playerView.nameplate.text = connectedPlayers[connection];


        NetworkServer.AddPlayerForConnection(connection, player);

        // Set nameplate
        PlayerNameplateMessage nameplateMessage = new PlayerNameplateMessage {
            networkId = player.GetComponent<NetworkIdentity>().netId,
            username = connectedPlayers[connection]
        };

        nameplateMessage.HandleRequest();
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
        netMsg.HandleRequestReceived(NetworkIdentity.spawned[netMsg.networkId]);
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
