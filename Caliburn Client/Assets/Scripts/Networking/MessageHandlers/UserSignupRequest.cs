using System.Security.Cryptography;
using UnityEngine;
using Mirror;

public class UserSignupRequest : MessageBase {
    public string username;
    public string password;
    public string email;
    public int requestCode;

    public void HandleRequest() {
        string passwordRaw = password;
        
        using(MD5 md5Hash = MD5.Create()) {
            string hash = Hashing.GetMd5Hash(md5Hash, passwordRaw);
            if(Hashing.VerifyMd5Hash(md5Hash, passwordRaw, hash)) {
                password = hash;
                NetworkClient.Send<UserSignupRequest>(this);
            } else {
                Debug.Log("Hashes are different.");
            }
        }
    }
}