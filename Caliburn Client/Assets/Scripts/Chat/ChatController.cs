using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour {
    static ChatController _instance;
    public static ChatController Instance {
        get {
            if(_instance == null) {
                throw new System.Exception();
            }

            return _instance;
        }
    }

    public GameObject messagePrefab;
    public Transform messageContainer;

    void Awake() {
        _instance = this;
    }

    public void CreateNewMessage(int connectionId, string newMessage) {
        GameObject message = Instantiate(messagePrefab) as GameObject;
        message.GetComponent<Text>().text = connectionId + ": " + newMessage;
        message.transform.SetParent(messageContainer);
    }
}
