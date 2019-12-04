using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SessionButton : MonoBehaviour
{
    public int session_num;
    public GameObject sessionsDirectorOB;
    SessionsDirector sessionsDirector;
    private Button b;


    void Start()
    {
        sessionsDirector = sessionsDirectorOB.GetComponent<SessionsDirector>();
        b = GetComponent<Button>();
        b.onClick.AddListener(OnPress);
    }

    void OnPress()
    {
        sessionsDirector.SetSessionForPracticeMode(session_num);
    }
}