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

// Class to handle All Xmpp events 

public class LoginHandlerScript : MonoBehaviour {

	public InputField userNameInputField;	
	public InputField passwordInputField;
	public InputField domainInputField;

	public static LoginHandlerScript loginHandlerScript; 
	public static bool isContactFetchComplete;
	public static bool isUserOnline;
	public string xmppDomain;
	public XmppClient xmppClient;
	public string license;
	ArrayList ChatItemBeans;
	static bool isLoginSuccess;
	static bool isMsgReceived;

	string receiverUserName;
	string receiverJid;
	string receivedMsg;

	ConversationListController ConversationListController;

	// Create a Singleton instance
	void Awake(){
		if(loginHandlerScript == null){
			DontDestroyOnLoad(gameObject);
			loginHandlerScript = this;
		}else{
			if(loginHandlerScript != this){
				Destroy(gameObject);
			}
		}
	}

	// Use this for initialization
	void Start () {
		isLoginSuccess = false;
		isMsgReceived = false;
		isContactFetchComplete = false;
		isUserOnline = false;

		// Set Matrix license key
		Matrix.License.LicenseManager.SetLicense(license);

		SignInUser();
	}
	
	// Update is called once per frame
	void Update () {

		// Check if the user login was successful
		if(isLoginSuccess){
			isLoginSuccess = false;
			SceneManager.LoadScene("ShowChatsScene");
		}

		// Check if the user has received a message
		if(isMsgReceived){
			isMsgReceived = false;

			 // pass the message to ConversationListController
			ConversationListController.ReceiveMessageEvent(receivedMsg);
		}
		HandleBackPress();
	}

	// Sign in user with username and password
	public void SignInUser(){
		string userName = userNameInputField.text;
		string password = passwordInputField.text;
		if(!string.IsNullOrEmpty(domainInputField.text)){
			xmppDomain = domainInputField.text;
		}
		if(!string.IsNullOrEmpty(PlayerPrefs.GetString("userJid"))
			&& !string.IsNullOrEmpty(PlayerPrefs.GetString("xmppDomain"))
			&& !string.IsNullOrEmpty(PlayerPrefs.GetString("userPassword"))){
				userName = PlayerPrefs.GetString("userJid");
				password = PlayerPrefs.GetString("userPassword");
				xmppDomain = PlayerPrefs.GetString("xmppDomain");
		}
		if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)){
				return;
		}

		PlayerPrefs.SetString("userJid",userName);
		PlayerPrefs.SetString("userPassword", password);
		PlayerPrefs.SetString("xmppDomain",xmppDomain);
		// Setup XMPP client
		xmppClient = new XmppClient {XmppDomain = xmppDomain, Username = userName, Password = password};

		// Login Event
		xmppClient.OnLogin += new EventHandler<Matrix.EventArgs>(xmpp_OnLogin);
		// Login error event
		xmppClient.OnAuthError += new EventHandler<Matrix.Xmpp.Sasl.SaslEventArgs>(xmpp_OnAuthError);
		// Set AutoRoster true to fetch contacts after login
		xmppClient.AutoRoster = true;
		// Roster events
		xmppClient.OnRosterStart += new EventHandler<Matrix.EventArgs>(xmpp_OnRosterStart);
		xmppClient.OnRosterItem += new EventHandler<Matrix.Xmpp.Roster.RosterEventArgs>(xmpp_OnRosterItem);
		xmppClient.OnRosterEnd += new EventHandler<Matrix.EventArgs>(xmpp_OnRosterEnd);
		// On Message received event
		xmppClient.OnMessage += new EventHandler<MessageEventArgs>(xmpp_OnMessage);
		// Open the connection
		xmppClient.Open();
	
	}

	// Handle Login event
	void xmpp_OnLogin(object sender, Matrix.EventArgs e){
        UnityEngine.Debug.Log("On Login Success "+e);
		isLoginSuccess = true;
	}

	// Handle login failure event
	void xmpp_OnAuthError(object sender, Matrix.EventArgs e){
		UnityEngine.Debug.Log("On Login Failure "+e);	
		isLoginSuccess = false;
	}

	// Handle RosterStart event
	void xmpp_OnRosterStart(object sender, Matrix.EventArgs e){
		UnityEngine.Debug.Log("OnRoasterStart :"+e);
	}

	// Handle RosterItem event used for fetching contacts
	void xmpp_OnRosterItem(object sender, Matrix.Xmpp.Roster.RosterEventArgs rosterEventArgs){
        UnityEngine.Debug.Log("OnRoasterItem :"+rosterEventArgs.ToString());
		UnityEngine.Debug.Log(rosterEventArgs.RosterItem.Jid);
		UnityEngine.Debug.Log(rosterEventArgs.RosterItem.Name);
		if(ChatItemBeans == null){
			ChatItemBeans = new ArrayList();
		}
		// Don't add if contact is already displayed 
		ChatItemBean chatItemBean = new ChatItemBean(rosterEventArgs.RosterItem.Jid,rosterEventArgs.RosterItem.Name,"");
		if(!ChatItemBeans.Contains(chatItemBean)){
			ChatItemBeans.Add(chatItemBean);
		}
	}

	// Handle RosterEnd event indicates all contacts are fetched
	void xmpp_OnRosterEnd(object sender, Matrix.EventArgs eventArgs){
		UnityEngine.Debug.Log("OnRoasterEnd");
		isContactFetchComplete = true;
	}

	// Set Jid and UserName for starting new chat
	public void StartNewChat(string userJid, string userName){
		receiverJid = userJid;
		receiverUserName = userName;
	}

	// Send XMPP message 
	public void SendXmppMessage(string msg, ConversationListController conversationListController){
		xmppClient.Send(new Message {To = receiverJid, Type = MessageType.Chat, Body = msg});
		if(ConversationListController == null){
			ConversationListController = conversationListController;
		}
	}

	// Handle XMPP receive message event
	void xmpp_OnMessage(object sender, MessageEventArgs e){
        if (e.Message.Body != null){
          	UnityEngine.Debug.Log(string.Format("OnMessage from {0}", e.Message.From));
        	UnityEngine.Debug.Log(string.Format("Body {0}", e.Message.Body));
        	UnityEngine.Debug.Log(string.Format("Type {0}", e.Message.Type));
			receivedMsg = e.Message.Body;
			isMsgReceived = true;
        }
    }

	public void LogoutUser(){
		isLoginSuccess = false;
		isMsgReceived = false;
		isContactFetchComplete = false;
		isUserOnline = false;
		receiverUserName = "";
		receiverJid = "";
		ChatItemBeans = new ArrayList();
		xmppClient.Close();
		PlayerPrefs.DeleteAll();
		Destroy(gameObject);
		}

    // Get chat items 
	public ArrayList GetChatItemBeans(){
		if(ChatItemBeans != null){
			return ChatItemBeans;
		}
		else{
			return null;
		}
	}

	// Get receiver's nickname
	public string GetReceiverName(){
		return receiverUserName;
	}

	// Get receiver's jid
	public string GetReceiverJid(){
		return receiverJid;
	}

	// Handle backpress event 
	void HandleBackPress(){
		if (Input.GetKeyDown(KeyCode.Escape)) {    
           // Quit the application
            Application.Quit();
        }
	}
}
