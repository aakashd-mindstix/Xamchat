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

public class ChatListController : MonoBehaviour {

	public GameObject ContentPanel;
	public GameObject ChatItemPrefab;
	ArrayList ChatItemBeans;

	bool areContactsFetched;

	// Use this for initialization
	void Start () {
		areContactsFetched = false;
 	} 
	
	// Update is called once per frame
	void Update () {
		if(!areContactsFetched){
			FetchContacts();
		}

		HandleBackPress();
	}

// Fetch contacts from xmppclient
	void FetchContacts(){
		if(LoginHandlerScript.isContactFetchComplete){
			if(ChatItemBeans == null ){
				ChatItemBeans = new ArrayList();
			}
			ChatItemBeans.Clear();
			LoginHandlerScript loginHandlerScript = LoginHandlerScript.loginHandlerScript;
			ArrayList chatItems = new ArrayList();
			if(loginHandlerScript.GetChatItemBeans()!= null){
				chatItems.AddRange(loginHandlerScript.GetChatItemBeans());
			}
			PopulateContacts(chatItems);
		}
	}

// Display the fetched contacts
	void PopulateContacts(ArrayList chatItemBeans){
		ChatItemBeans.AddRange(chatItemBeans);
		foreach(ChatItemBean chatItemBean in ChatItemBeans){
			GameObject newChatItem = Instantiate(ChatItemPrefab) as GameObject;
			var controller = newChatItem.GetComponent<ChatListItemController>();
			controller.ProfilePicture.text = chatItemBean.Name.Substring(0,1);
			controller.Jid = chatItemBean.Jid;
			controller.Name.text = chatItemBean.Name;
			controller.RecentMessage.text = chatItemBean.RecentMessage;
			newChatItem.transform.parent = ContentPanel.transform;
			newChatItem.transform.localScale = Vector3.one;
		}
		areContactsFetched = true;
	}

	// Load New chat scene to add new contact
	public void LoadNewChatScene(){
		SceneManager.LoadScene("NewChatScene");
	}

	public void LogoutUser(){
		LoginHandlerScript loginHandlerScript = LoginHandlerScript.loginHandlerScript;
		loginHandlerScript.LogoutUser();
		SceneManager.LoadScene("LoginScene");
	}

	void HandleBackPress(){
		if (Input.GetKeyDown(KeyCode.Escape)) {
            // Quit the application
            SceneManager.LoadScene("ShowChatsScene");
        }
	}
}