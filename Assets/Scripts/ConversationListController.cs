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

// Class to handle Conversation
public class ConversationListController : MonoBehaviour {

	public Text ProfilePicture;
	public Text ProfileName;
	ArrayList MessageItemBeans;
	public GameObject ContentPanel;
	public GameObject SendMsgPrefab;
	public GameObject RecMsgPrefab;

	public InputField newMessage;

	LoginHandlerScript loginHandlerScript;

	// Use this for initialization
	void Start () {
		MessageItemBeans = new ArrayList();
		loginHandlerScript = LoginHandlerScript.loginHandlerScript;
		XmppClient xmppClient = loginHandlerScript.xmppClient;
		string receiverJid = loginHandlerScript.GetReceiverJid();
		SubscribeToPresence(xmppClient,receiverJid);
		xmppClient.OnPresence += new EventHandler<PresenceEventArgs>(xmpp_OnPresence);
		string userName = loginHandlerScript.GetReceiverName();
		ProfilePicture.text = userName.Substring(0,1);
		ProfileName.text = userName; 
 	} 

	//  Send new xmpp message 
	public void SendNewMessage(){
		string msg = newMessage.text;
		newMessage.text = "";
		if(!string.IsNullOrEmpty(msg)){
			loginHandlerScript.SendXmppMessage(msg,this);
			var messageItem = new MessageItemBean(msg,true,false);
			MessageItemBeans.Add(messageItem);
			PopulateChat();
		}
	}

	// Subscribe to Presence event of contact 
	void SubscribeToPresence(XmppClient xmppClient, string jid){
		var pm = new PresenceManager(xmppClient);
		pm.Subscribe(jid);
	}

	// Handle incoming message event
	public void ReceiveMessageEvent(string msg){
		var messageItem = new MessageItemBean(msg,false,false);
		MessageItemBeans.Add(messageItem);
		PopulateChat();
	}

	// Populate chat screen with messages
	void PopulateChat(){
		if(MessageItemBeans.ToArray().Length ==0){
			return;
		}
		foreach(MessageItemBean messageItemBean in MessageItemBeans){
			if(messageItemBean.IsDisplayed){
				continue;
			}
			messageItemBean.IsDisplayed = true;
			GameObject newMessageItem;
			if(messageItemBean.IsSend){
					newMessageItem = Instantiate(SendMsgPrefab) as GameObject;
			}else{
				newMessageItem = Instantiate(RecMsgPrefab) as GameObject;
			}

			var controller = newMessageItem.GetComponent<ConversationItemController>();
			controller.Message.text = messageItemBean.Message;
			newMessageItem.transform.parent = ContentPanel.transform;
			newMessageItem.transform.localScale = Vector3.one;
 		}
	}

	// EventHandler for Presence event
	void xmpp_OnPresence(object sender,PresenceEventArgs e){
        UnityEngine.Debug.Log(string.Format("OnPresence from {0}", e.Presence.From));
        UnityEngine.Debug.Log(string.Format("Status {0}", e.Presence.Status));
        UnityEngine.Debug.Log(string.Format("Show type {0}", e.Presence.Show));
        UnityEngine.Debug.Log(string.Format("Priority {0}", e.Presence.Priority));
	}

	void HandleBackPress(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("ShowChatsScene");
        }
	}

}
