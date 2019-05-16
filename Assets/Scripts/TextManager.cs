using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Colony {

    public class TextManager : MonoBehaviour
    {
        public int maxMessages = 25;

        public GameObject chatPanel, textObject;

        public Color dialouge, info, warning;

        [SerializeField]
        List<Message> messageList = new List<Message>();

        void Update() {

        }

        public void SendMessageToChat(string text, Message.MessageType messageType) {
            if (messageList.Count >= maxMessages) {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }

            Message newMessage = new Message();
            newMessage.text = text;
            GameObject newText = Instantiate(textObject, chatPanel.transform);

            newMessage.textObject = newText.GetComponent<Text>();
            newMessage.textObject.text = newMessage.text;
            newMessage.textObject.color = MessageTypeColor(messageType);
            if (messageType == Message.MessageType.warning) {
                newMessage.textObject.fontStyle = FontStyle.Bold;
            }

            messageList.Add(newMessage);

        }

        public void ReplaceMessageInChat(string text) {
            messageList[messageList.Count -1].text = text;
            messageList[messageList.Count -1].textObject.text = text;
        }

        public string LastMessageInChat() {
            return messageList[messageList.Count -1].text;
        }

        Color MessageTypeColor(Message.MessageType messageType) {
            Color color = info;

            switch(messageType) {
                case Message.MessageType.dialouge:
                    color = dialouge;
                    break;

                case Message.MessageType.warning:
                    color = warning;
                    break;
            }
            return color;
        }
    }

    [System.Serializable]
    public class Message 
    {
        public string text;
        public Text textObject;
        public MessageType messageType;

        public enum MessageType {
            dialouge,
            info,
            warning
        }
    }
}