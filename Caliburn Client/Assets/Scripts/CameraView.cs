using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraView : MonoSingleton<CameraView> {
    float panSpeed = 3f;
    void FixedUpdate() {
        if(NetworkClient.connection == null) {return;}
        if(NetworkClient.connection.playerController == null) {return;}

        Vector3 targetPosition = NetworkClient.connection.playerController.transform.position;
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y, transform.position.z), Time.deltaTime * panSpeed);
    }
}
