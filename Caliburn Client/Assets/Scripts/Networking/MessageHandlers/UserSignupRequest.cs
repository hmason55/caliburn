using Mirror;

public class UserSignupRequest : MessageBase {
    public string username;
    public string password;
    public string email;
    public int requestCode;

    public override void Deserialize(NetworkReader reader) {
        username = reader.ReadString();
        password = reader.ReadString();
        email = reader.ReadString();
        requestCode = reader.ReadPackedInt32();
    }

    public override void Serialize(NetworkWriter writer) {
        writer.WriteString(username);
        writer.WriteString(password);
        writer.WriteString(email);
        writer.WritePackedInt32(requestCode);
    }
}