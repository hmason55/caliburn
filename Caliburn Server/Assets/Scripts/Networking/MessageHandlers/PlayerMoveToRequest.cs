using UnityEngine;
using Mirror;

public class PlayerMoveToRequest : MessageBase {
    public uint networkId;
    public Vector2 destination;

    public void HandleRequest() {
        // Client stuff
    }

    public void HandleRequestReceived(NetworkIdentity identity) {
        // Server
        PlayerView playerView = identity.GetComponent<PlayerView>();
        Vector2 heading = new Vector2(destination.x - playerView.transform.position.x, destination.y - playerView.transform.position.y);

        // Raycast to terrain objects and other collision layers that obstruct movement.
        RaycastHit2D hit = Physics2D.Raycast(playerView.transform.position, heading.normalized, heading.magnitude, LayerMask.GetMask("Terrain Collision"));
        if(hit.collider != null) {
            destination = hit.point - (heading.normalized * playerView.transform.GetComponent<CircleCollider2D>().radius);
        }
        
        Debug.DrawLine(new Vector3(playerView.transform.position.x, playerView.transform.position.y, 1f), new Vector3(destination.x, destination.y, 1f), Color.red, 2f);
        playerView.destination = destination;

        // Update position.
        Server.Instance.playerDataByNetId[networkId].destination = destination;

        // Make sure the player is able to do this movement.
        NetworkServer.SendToAll<PlayerMoveToRequest>(this);
    }
}