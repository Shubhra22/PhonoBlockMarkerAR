using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Scripting;

public class LogoutButtonHandler : MonoBehaviour
{
    private Button button;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickLogout);
    }

    void OnClickLogout()
    {
        SessionsDirector.instance.LogOut();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
