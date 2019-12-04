using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButtonHandler : MonoBehaviour
{
    private Button button;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickBack);
        
    }

    void OnClickBack()
    {
        if (SceneManager.GetActiveScene().name == "Activity")
        {
            SessionsDirector.instance.ReturnToMainMenu();
        }
        else if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.Log(SessionsDirector.instance.currentPage);
            if (SessionsDirector.instance.currentPage == SessionsDirector.CurrentPage.Lessons)
            {
                SessionsDirector.instance.SetupModeSelectionMenu();
            }
            
            else if (SessionsDirector.instance.currentPage == SessionsDirector.CurrentPage.Modes)
            {
                //SessionsDirector.instance.ReturnToMainMenu();
                button.interactable = false;
            }
        }
    }

}
