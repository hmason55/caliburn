using Mirror;
using UnityEngine;

public class UserSignupRequest : MessageBase {
    public string username;
    public string password;
    public string email;
    public int requestCode;

    public void HandleRequestReceived(NetworkConnection connection) {
        ProcessSignup.Instance.Request(username, password, email, (requestCode) => {

            Debug.Log("Signup request completed with request code: " + requestCode);
            this.requestCode = requestCode;
            NetworkServer.SendToClient<UserSignupRequest>(connection.connectionId, this);
        });
    }
}