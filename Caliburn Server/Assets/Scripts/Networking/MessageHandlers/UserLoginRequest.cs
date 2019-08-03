using Mirror;

public class UserLoginRequest : MessageBase {
    public string username;
    public string password;
    public int requestCode;
    public int connectionId;

    public override void Deserialize(NetworkReader reader) {
        username = reader.ReadString();
        password = reader.ReadString();
        requestCode = reader.ReadPackedInt32();
        connectionId = reader.ReadPackedInt32();
    }

    public override void Serialize(NetworkWriter writer) {
        writer.WriteString(username);
        writer.WriteString(password);
        writer.WritePackedInt32(requestCode);
        writer.WritePackedInt32(connectionId);
    }
}
