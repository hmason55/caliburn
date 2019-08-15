using Mirror;
using UnityEngine;

public class UserLoginRequest : MessageBase {
    public string username;
    public string password;
    public int requestCode;
    public int connectionId;

    public void HandleRequestReceived(NetworkConnection connection) {
        ProcessLogin.Instance.Request(username, password, (requestCode) => {
            this.requestCode = requestCode;

            switch(requestCode) {
                case 0:
                    if(!Server.Instance.usernamesByConnection.ContainsValue(username)) {
                        connectionId = connection.connectionId;
                        Server.Instance.usernamesByConnection.Add(connection, username);
                        Debug.Log(username + " logged in.");
                        
                    } else {
                        this.requestCode = 5;
                    }
                break;
            }
            
            Debug.Log("Login request completed with request code: " + requestCode);
            NetworkServer.SendToClient<UserLoginRequest>(connection.connectionId, this);
        });
    }
}
