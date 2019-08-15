using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using Mirror;

public class UserLoginRequest : MessageBase {
    public string username;
    public string password;
    public int requestCode;
    public int connectionId;

    public void HandleRequest() {
        string passwordEncryption = password;
        
        using(MD5 md5Hash = MD5.Create()) {
            string hash = Hashing.GetMd5Hash(md5Hash, passwordEncryption);
            if(Hashing.VerifyMd5Hash(md5Hash, passwordEncryption, hash)) {
                password = hash;
                NetworkClient.Send<UserLoginRequest>(this);
            } else {
                Debug.Log("Hashes are different.");
            }
        }
    }
}
