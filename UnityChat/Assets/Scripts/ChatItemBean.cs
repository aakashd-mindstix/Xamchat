using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Model class for individual chat item

public class ChatItemBean{

	public string Jid;
	public string Name;
	public string RecentMessage;

	public ChatItemBean (string jid, string name, string recentMessage){
		Jid = jid;
		Name = name;
		RecentMessage = recentMessage;
	}
}
