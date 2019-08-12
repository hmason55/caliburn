using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatController : MonoSingleton<ChatController> {
    public GameObject messagePrefab;
    public Transform messageContainer;

    public void CreateNewMessage(string playerName, string newMessage) {
        GameObject message = Instantiate(messagePrefab) as GameObject;
        message.GetComponent<Text>().text = playerName + ": " + newMessage;
        message.transform.SetParent(messageContainer);
    }

    public bool IsMouseOver() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
