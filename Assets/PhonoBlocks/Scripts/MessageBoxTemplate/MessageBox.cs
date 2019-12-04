using System.Collections;
using System.Collections.Generic;
using JoystickLab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : SingleToneManager<MessageBox>
{
    public MessageBoxObject messageBoxObject;

//    private void Awake()
//    {
//        messageBoxObject = Instantiate(messageBoxObject, Vector3.zero, Quaternion.identity);
//        messageBoxObject.gameObject.SetActive(false);
//    }

    public void Show(string message)
    {
        messageBoxObject.Message.text = message;

        Instantiate(messageBoxObject, Vector3.zero, Quaternion.identity);

        messageBoxObject.gameObject.SetActive(true);
        
        Button okButton = messageBoxObject.OkButton;
        
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(Close);
    }
    
    public void Show(string message, UnityAction okEvent)
    {
        Instantiate(messageBoxObject, Vector3.zero, Quaternion.identity);
        messageBoxObject.Message.text = message;

        Button okButton = messageBoxObject.OkButton;
        
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(okEvent);        
        okButton.onClick.AddListener(Close);
    }
    
   
    void Close()
    {
        messageBoxObject.gameObject.SetActive(false);
        //Destroy(messageBoxObject);
    }
}
