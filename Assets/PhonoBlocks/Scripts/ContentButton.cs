using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* We will make this into a class that allows teachers to pick the prolbem type, versus the session*/
public class ContentButton : MonoBehaviour
{
    public GameObject sessionsDirectorOB;
    SessionsDirector sessionsDirector;
    public ProblemsRepository.ProblemType problemType;

    private Button b;

    void Start()
    {
        sessionsDirector = sessionsDirectorOB.GetComponent<SessionsDirector>();
        b = GetComponent<Button>();
        b.onClick.AddListener(OnPress);
    }

    void OnPress()
    {
         sessionsDirector.SetContentForTeacherMode(problemType);
    }
}