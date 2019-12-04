using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxObject : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI message;
    
    [SerializeField]
    private Button okButton;

    public TextMeshProUGUI Message
    {
        get { return message; }
        set { message = value; }
        
    }

    public Button OkButton
    {
        get { return okButton;}
        set { okButton = value; }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
    
    
}
