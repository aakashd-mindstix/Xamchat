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

// Handle events on NewChatScene
public class NewChatHandlerScript : MonoBehaviour {

	public InputField ReceiverName;

	public InputField NickName;

	XmppClient xmppClient;

	// Update is called once per frame
	void Update () {
		HandleBackPress();
	}

	// Set new receiver to start conversation
	public void setReceiver(){
		string userJid  = ReceiverName.text;
		string userName = NickName.text;
		if(!string.IsNullOrEmpty(userName)){
		LoginHandlerScript loginHandlerScript = LoginHandlerScript.loginHandlerScript;
		xmppClient = loginHandlerScript.xmppClient;
		if(string.IsNullOrEmpty(userJid) ){
		loginHandlerScript.StartNewChat(userName,userJid);	
		}else{
			loginHandlerScript.StartNewChat(userName,userJid);
		}
		AddNewContact();
		LoadConversation();
		}
	}

	// Load ConversationScene
	public void LoadConversation(){
		SceneManager.LoadScene("ConversationScene");
	}

	// Add contact to contact list
	void AddNewContact(){
		var rm = new RosterManager(xmppClient);
		var pm = new PresenceManager(xmppClient);
		Jid jid = ReceiverName.text;
		if(string.IsNullOrEmpty(NickName.text)){
			rm.Add(jid, ReceiverName.text);
		}else{
			rm.Add(jid, NickName.text);
		}
		pm.Subscribe(jid);
	}

	void HandleBackPress(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
            
            // Quit the application
            Application.Quit();
        }
	}
}
