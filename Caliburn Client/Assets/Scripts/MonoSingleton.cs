using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
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

    protected void OnApplicationQuit() {
        _instance = null;
    }

}
