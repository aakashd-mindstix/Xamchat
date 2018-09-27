using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Model class for individual message item
public class MessageItemBean{
	public string Message;
	public bool IsSend;
	public bool IsDisplayed;

	public MessageItemBean (string message, bool isSend, bool isDisplayed){
		Message = message;
		IsSend = isSend;
		IsDisplayed = isDisplayed;
	}
}
