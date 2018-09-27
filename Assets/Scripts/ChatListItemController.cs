using System.Collections;
using System.Collections.Generic;
using Matrix;
using System;
using UnityEngine;
using System.Xml.Linq;
using Matrix.Xmpp;
using Matrix.Xmpp.Client;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.SceneManagement;

// Model class to hold individual contact

public class ChatListItemController : MonoBehaviour {
 public Text ProfilePicture, Name, RecentMessage;

 public string Jid;

 public void LoadChatScene(){
     LoginHandlerScript loginHandlerScript = LoginHandlerScript.loginHandlerScript;
     loginHandlerScript.StartNewChat(Jid,Name.text);
     SceneManager.LoadScene("ConversationScene");
 }
}
