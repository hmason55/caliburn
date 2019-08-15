using UnityEngine;
using Mirror;

public abstract class NetworkManagerSingleton<T> : NetworkManager where T : NetworkManagerSingleton<T> {
    bool _initialized = false;
    static T _instance = null;
    public static T Instance {
        get {
            if(_instance == null) {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;
            }

            if(_instance._initialized == false) {
                _instance.Init();
            }

            return _instance;
        }
    }

    protected virtual void Init() {
        _initialized = true;
    }

    public override void OnApplicationQuit() {
        _instance = null;
    }
}
