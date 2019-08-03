using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Client : NetworkManager {

    public LoginManager loginManager;
    
    public override void Start() {
        Application.runInBackground = true;
        Connect();
    }

    void Connect() {
        NetworkClient.Connect("localhost");
        RegisterHandlers();
    }

    public void Login(string u, string p) {
        UserLoginRequest user = new UserLoginRequest {
            username = u,
            password = p
        };

        string passwordEncryption = user.password;
        
        using(MD5 md5Hash = MD5.Create()) {
            string hash = Hashing.GetMd5Hash(md5Hash, passwordEncryption);
            if(Hashing.VerifyMd5Hash(md5Hash, passwordEncryption, hash)) {
                user.password = hash;
                NetworkClient.Send<UserLoginRequest>(user);
                Debug.Log(hash);
            } else {
                Debug.Log("Hashes are different.");
            }
        }
    }

    public void Signup(string u, string p, string c, string e) {
        UserSignupRequest user = new UserSignupRequest {
            username = u,
            password = p,
            email = e,
        };

        string passwordRaw = user.password;
        
        using(MD5 md5Hash = MD5.Create()) {
            string hash = Hashing.GetMd5Hash(md5Hash, passwordRaw);
            if(Hashing.VerifyMd5Hash(md5Hash, passwordRaw, hash)) {
                user.password = hash;
                NetworkClient.Send<UserSignupRequest>(user);
                Debug.Log(hash);
            } else {
                Debug.Log("Hashes are different.");
            }
        }
    }

    void RegisterHandlers() {
        NetworkClient.RegisterHandler<ConnectMessage>(OnClientConnected);
        NetworkClient.RegisterHandler<DisconnectMessage>(OnClientDisconnected);
        NetworkClient.RegisterHandler<UserLoginRequest>(OnLoginRequestReceived);
        NetworkClient.RegisterHandler<UserSignupRequest>(OnSignupRequestReceived);
        NetworkClient.RegisterHandler<ChatMessage>(OnChatMessageReceived);
    }

    void OnClientConnected(NetworkConnection connection, ConnectMessage netMsg) {
        Debug.Log("Connected to server");
    }

    void OnClientDisconnected(NetworkConnection connection, DisconnectMessage netMsg) {
        Debug.Log("Disconnected from server");
    }

    void OnLoginRequestReceived(NetworkConnection connection, UserLoginRequest netMsg) {
        if(connection.connectionId == 0) {
            NetworkClient.connection.connectionId = netMsg.connectionId;
        }

        Debug.Log("Received reply from server with request code: " + netMsg.requestCode);
        switch(netMsg.requestCode) {
            case 0:
                Debug.Log("Login success.");
                //PlayerInfo.Instance.PlayerName = netMsg.username;
                ClientScene.RegisterPrefab(playerPrefab);
                ClientScene.AddPlayer(NetworkClient.connection);
                SceneManager.LoadScene("World");
            break;

            case 1:
                Debug.Log("Invalid username or password.");
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
        if(connection.connectionId == 0) {return;} // not logged in yet.
        
        netMsg.HandleMessageReceived();
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


