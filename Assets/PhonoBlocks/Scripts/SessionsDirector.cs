using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * session manager needs to instantiate and set up the variables of the SessionParameters component if the mode is student mode
 * 
 * */
public class SessionsDirector : MonoBehaviour
//change this between students
{

    public GameObject loadingScene;
    public static SessionsDirector instance;
    public static ColourCodingScheme  colourCodingScheme = new NoColour ();//new RControlledVowel();
    public GameObject loginScreen;

    //[SerializeField]
    List<string> consonantBlend = new List<string>(){"bl","cr","fl","gr","sk","st","tr","dr"};

    //[SerializeField]
    List<string> consonantDigraph = new List<string>() { "th", "ch", "gh","sh","ck" };


    public List<string> ConsonantBlend
    {
        get
        {
            return consonantBlend;
        }
    }

    public List<string> ConsonantDigraph
    {
        get
        {
            return consonantDigraph;
        }
    }


    public bool IsMagicERule
    {
        get
        {
            return colourCodingScheme.label.Equals("vowelInfluenceE");
        }
    }

    public bool IsSyllableDivisionMode
    {
        get
        {
            return colourCodingScheme.label.Equals("syllableDivision");

        }

    }

    public bool IsRControlledMode
    {
        get 
        {
            return colourCodingScheme.label.Equals("rControlledVowel");
        }
    }

    public bool IsVowelTeam
    {
        get 
        {
            return colourCodingScheme.label.Equals("vowel Digraphs");
        }
    }

    public bool IsConsonantDiagraph
    {
        get
        {
            return colourCodingScheme.label.Equals("consonantDigraphs");
        }
    }

    public bool IsConsonantBlend
    {
        get
        {
            return colourCodingScheme.label.Equals("Blends");
        }
    }

    public bool IsCVC
    {
        get
        {
            return colourCodingScheme.label.Equals("openClosedVowel");
        }
    }
    public INTERFACE_TYPE INTERFACE;
    public static bool started;


    public enum INTERFACE_TYPE
    {
        TANGIBLE,
        SCREEN_KEYBOARD
    }
    ;

    public bool IsScreenMode()
    {
        return INTERFACE == INTERFACE_TYPE.SCREEN_KEYBOARD;

    }

    public static int currentUserSession; //will obtain from player prefs
    public static int numStarsOfCurrentUser; //will obtain from player prefs


    public static bool IsTheFirstTutorLedSession()
    {

        return currentUserSession == 0;

    }

    static Mode mode; //testing mode. can be student driven (usual phonoblocks practice session, phono reads words), test (assessment) or sandbox

    public static bool DelegateControlToStudentActivityController
    {
        get
        {
            return mode == Mode.STUDENT;
        }


    }

    /* also more like a "sandbox" mode; teacher can create whatever words they want */
    public static bool IsTeacherMode
    {
        get
        {
            return mode == Mode.TEACHER;
        }
    }

    public static bool IsStudentMode
    {
        get
        {
            return mode == Mode.STUDENT;
        }


    }


    public GameObject studentActivityControllerOB;
    public GameObject activitySelectionButtons;
    public GameObject sessionSelectionButtons;
    //public GameObject modeSelectionScreen;
    public GameObject teacherModeButton;
    public GameObject studentModeButton;
    //public GameObject studentNameInputField;
    //public GameObject returnToModeSelectButton;
    public GameObject dataTables;
    string studentName;
    public AudioClip noDataForStudentName;
    public AudioClip enterAgainToCreateNewFile;
    public static DateTime assessmentStartTime;

    public GameObject backButton;

    public enum CurrentPage
    {
        Login,
        Modes,
        Lessons,
    }

    public CurrentPage currentPage;

    public enum Mode
    {
        TEACHER,
        STUDENT

    }

    private void Awake()
    {
        Init();
        InitializeLoginInformation();
    }

    void Start()
    {
        
        if(instance==null)
        {
            instance = this;
            SpeechSoundReference.Initialize();
        }
        else if(instance!=this)
        {
            Destroy(this.gameObject);
        }

        //studentName = studentNameInputField.GetComponent<InputField>();
        SetupModeSelectionMenu();

    }

    #region Derectory Initialization   
    private string DATA_FILE_DIRECTORY;
    private string LOG_FILE_DIRECTORY;
    public void InitializeLoginInformation()
    {
        DATA_FILE_DIRECTORY = Application.persistentDataPath + "/data/";
        LOG_FILE_DIRECTORY = Application.persistentDataPath+"/logs/";
        CreateDirectoryWhenNeeded(DATA_FILE_DIRECTORY);
        CreateDirectoryWhenNeeded(LOG_FILE_DIRECTORY);
    }

    

    void CreateDirectoryWhenNeeded(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
    }

    #endregion
    public void SetStudentNameOnClick(UnityEngine.UI.InputField textBox)
    {
        studentName = textBox.text;

        if (studentName == "")
        {
            MessageBox.Instance.Show("Username can not be empty");
            return;
        }
        
        InitializeLoginInformation();
        bool wasStoredDataForName = StudentsDataHandler.instance.LoadStudentData(studentName);
        
        bool newUser = studentName[studentName.Length - 1] == '*';

        if (newUser)
        {
            CreateNewFileIfNeeded(studentName);
        }
        
        if (wasStoredDataForName || newUser)
        {
            loginScreen.SetActive(false);
            textBox.text = "";
        }
        else
        {
            MessageBox.Instance.Show("User Not found. If you are a new user, please put an * after your name");
        }
        

        currentPage = CurrentPage.Modes;
        
    }

    void Init()
    {
        activitySelectionButtons = GameObject.Find("ContentSelectionUI");
        sessionSelectionButtons = GameObject.Find("SessionSelectionUI");
        teacherModeButton = GameObject.Find("StartTeacherModeButton");
        studentModeButton = GameObject.Find("StartStudentModeButton");
       // studentNameInputField = GameObject.Find("StudentNameInputField");
        
    }
    
    public void SetupModeSelectionMenu()
    {

        assessmentStartTime = DateTime.Now;
        activitySelectionButtons.SetActive(false);
        sessionSelectionButtons.SetActive(false);
        studentModeButton.SetActive(true);
        teacherModeButton.SetActive(true);
        //studentNameInputField.SetActive(false);
        loadingScene.SetActive(false);
        loadingScene.GetComponentInChildren<Slider>().value = 0;
        currentPage = CurrentPage.Modes;
        backButton.SetActive(false);
        
//        Debug.Log(mode);

    }

    public void ReturnToMainMenu()
    {

        if (!Application.loadedLevelName.Equals("MainMenu"))
            Application.LoadLevel("MainMenu");

        if (mode == Mode.TEACHER)
        {
            SelectTeacherMode();
        }
        else if (mode == Mode.STUDENT)
        {
            SelectStudentMode();
        }
        //SetupModeSelectionMenu();
        

    }

    //Teacher mode is the current "sandbox" mode, which just defaults to rthe colour scheme chosen at the head of this file.
    public void SelectTeacherMode()
    {
        mode = Mode.TEACHER;
        activitySelectionButtons.SetActive(true);
        studentModeButton.SetActive(false);
        teacherModeButton.SetActive(false);
        currentPage = CurrentPage.Lessons;
        backButton.SetActive(true);
        //studentNameInputField.SetActive(false);
    }

    public void SetContentForTeacherMode(ProblemsRepository.ProblemType problemType)
    {

        colourCodingScheme = ProblemsRepository.instance.GetColourCodingSchemeGivenProblemType(problemType);
        //Application.LoadLevel("Activity");
        StartCoroutine(LoadSessions());
    }

    public void LogOut()
    {
        currentUserSession = -1;
        SceneManager.LoadScene("MainMenu");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(sceneMode);
        if (scene.name == "MainMenu")
        {
            currentPage = CurrentPage.Login;
            currentUserSession = -1;
            loginScreen.SetActive(true);
        }
    }

    public void SetSessionForPracticeMode(int session)
    {

        currentUserSession = session;
        SetParametersForStudentMode(studentActivityControllerOB);
        StartCoroutine(LoadSessions());
        //Application.LoadLevel("Activity");
        //SceneManager.LoadSceneAsync("Activity");
    }

    IEnumerator LoadSessions()
    {
        loadingScene.SetActive(true);
        Slider s = loadingScene.GetComponentInChildren<Slider>();       
        //yield return new WaitForSeconds(3);
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("Activity");
        
        //sceneLoading.allowSceneActivation = false;
        while (!sceneLoading.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoading.progress / 0.9f);
            s.value = progress;
            yield return null;
        }
        loadingScene.SetActive(false);
    }
    
    public void LoadSessionSelectionScreen()
    {
        sessionSelectionButtons.SetActive(true);
        studentModeButton.SetActive(false);
        teacherModeButton.SetActive(false);

        currentPage = CurrentPage.Lessons;
        //studentNameInputField.SetActive(false);
    }


    public void SelectStudentMode()
    {
        string nameEntered = studentName.Trim().ToLower();
        backButton.SetActive(true);
        if (nameEntered.Length > 0 || true)
        {
            print("Came Absoulute");

            nameEntered = CreateNewFileIfNeeded(nameEntered);


            bool wasStoredDataForName = StudentsDataHandler.instance.LoadStudentData(nameEntered);

            GameObject clone = null;
            if (wasStoredDataForName || true)
            {
                mode = Mode.STUDENT;

                try
                {
                    studentActivityControllerOB = FindObjectOfType<StudentActivityController>().gameObject;

                }
                catch
                {
                    studentActivityControllerOB = (GameObject)GameObject.Instantiate(studentActivityControllerOB);
                }
                    

                DontDestroyOnLoad(studentActivityControllerOB);

                LoadSessionSelectionScreen();

            }
            else
            {
                AudioSourceController.PushClip(noDataForStudentName);

            }
        }
//        else
//            studentNameInputField.SetActive(true);

    }

    string CreateNewFileIfNeeded(string nameEntered)
    {
        bool createNewFile = nameEntered[nameEntered.Length - 1] == '*'; //mark new file with asterik

        if (createNewFile)
        {


            nameEntered = nameEntered.Substring(0, nameEntered.Length - 1);


            StudentsDataHandler.instance.CreateNewStudentFile(nameEntered);



        }

        return nameEntered;
    }

    public void SetParametersForStudentMode(GameObject studentActivityController)
    {
        StudentsDataHandler.instance.UpdateUsersSession(currentUserSession);

        numStarsOfCurrentUser = StudentsDataHandler.instance.GetUsersNumStars();

        ProblemsRepository.instance.Initialize(currentUserSession);

        colourCodingScheme = ProblemsRepository.instance.ActiveColourScheme;

        StudentActivityController sc = studentActivityControllerOB.GetComponent<StudentActivityController>();

    }

    
}