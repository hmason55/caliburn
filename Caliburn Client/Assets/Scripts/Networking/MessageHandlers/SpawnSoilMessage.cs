using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnSoilMessage : MessageBase {
    public uint networkId;
    public int soilId;
    public int soilLayer;
    public Vector2 position;

    public void HandleRequestReceived() {
        Vector3Int gridPos = new Vector3Int((int)position.x, (int)position.y, 0);
        for(int x = gridPos.x-1; x <= gridPos.x+1; x++) {
            for(int y = gridPos.y-1; y <= gridPos.y+1; y++) {
                World.Instance.tilemaps[soilLayer].SetTile(new Vector3Int(x, y, 0), Soils.Instance.soilTiles[soilId]);
            }
        }
    }
}
