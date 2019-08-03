using UnityEngine;
using Mirror;
using System.Text;

public class Server : NetworkManager {

    public static Server _instance;

    public static Server Instance {
        get {
            if(_instance == null) {throw new MissingReferenceException();}
            return _instance;
        }
    }

    public override void Start() {
        Application.runInBackground = true;
        Initialize();
    }

    void Initialize() {
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
    }

    void OnClientConnected(NetworkConnection connection, ConnectMessage netMsg) {
        Debug.Log("Client connected to server");
    }

    void OnClientDisconnected(NetworkConnection connection, DisconnectMessage netMsg) {
        Debug.Log("Client disconnected from server");
    }

    void OnLoginRequestReceived(NetworkConnection connection, UserLoginRequest netMsg) {
        ProcessLogin.Instance.Request(netMsg.username, netMsg.password, (requestCode) => {
            Debug.Log("Login request completed with request code: " + requestCode);
            netMsg.requestCode = requestCode;
            netMsg.connectionId = connection.connectionId;
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

    void OnServerAddPlayer(NetworkMessage netMsg) {

        GameObject player = Instantiate(playerPrefab) as GameObject;
        player.transform.position = new Vector3(Random.Range(-3f, 3f) , 0f, 0f);

        NetworkServer.AddPlayerForConnection(netMsg.conn, player);
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

    #region Utility
    byte[] StringToBytes(string str) {
        return Encoding.UTF8.GetBytes(str);
    }

    string BytesToString(byte[] bytes) {
        return Encoding.UTF8.GetString(bytes);
    }
    #endregion
}
