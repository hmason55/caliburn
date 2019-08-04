using UnityEngine;

public class PixelCamera : MonoBehaviour {
    new Camera camera;
    Vector2 resolution;
    public int pixelsPerUnit = 16;
    public float zoomScale = 1f;

    void Awake() {
        camera = GetComponent<Camera>();
    }
    void Update() {
        if(resolution != new Vector2(Screen.width, Screen.height)) {
            ScaleCamera();
        }
    }

    void ScaleCamera() {
        resolution = new Vector2(Screen.width, Screen.height);
        camera.orthographicSize = (resolution.y / ((float)pixelsPerUnit * zoomScale)) * 0.5f;
    }
}
