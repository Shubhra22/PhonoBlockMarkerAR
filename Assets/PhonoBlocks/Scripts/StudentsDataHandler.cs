using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
public class StudentsDataHandler : MonoBehaviour
{
    static readonly string DATA_FILE_EXTENSION = ".csv";
    static readonly string DATA_FILE_DIRECTORY = Application.persistentDataPath+"/data/";
    static string LOG_FILE_DIRECTORY = Application.persistentDataPath+"/logs/";
    public readonly string ASSESSMENT_EXTENSION = "_t";
    public static StudentsDataHandler instance = new StudentsDataHandler();
    static string templateForExperimentWideParams = "[0,0]";
    const string experimentWideParamStart = "[";
    const string experimentWideParamEnd = "]";
    const int idxOfCurrentSessionInTemplate = 1;
    const int idxOfNumStarsInTemplate = 3;
    const string activityDelimiter = "!";

    //these arent indexes of the values in a string. they are idx of the values in the array
    //that we store in the student data object and which we place values into before saving it a single
    //string in player prefs.


    const string valDelimiter = ",";
    const int asciiFor0 = 48;
    
    static Dictionary<int, StudentData> dummyData;
    static StudentData currUser;

     #region Student Data Class

    public class StudentData
    {
        //values that we update each session. then we store the new values in the player prefs.
        int currSession;
        public int numStars;
        public int solved;
        public string studentName;
        public string dataFromPreviouslyCompletedActivities = "";
        public string targetWordOfCurrentActivity;
        public string initialWordOfCurrentActivity;
        public string conceptOfCurrentActivity;
        public string finalSubmittedWord;
        string asString;

        public int CurrentSession
        {
            get { return currSession; }
            set { currSession = value; }
        }


        //construct an object that "caches" the data of this student in the application
        //so that we can update the fields of the student that are stored in player prefs
        public StudentData(string studentName, int currSession, int numStars)
        {
            this.studentName = studentName;
            this.currSession = currSession;
            this.numStars = numStars;
        }

        public string ToString()
        {
            return asString;
        }

        public void AppendDataForCurrentActivityToPreviousData()
        {
            StringBuilder data = new StringBuilder(dataFromPreviouslyCompletedActivities);
            AppendDatum(data, currSession);
            AppendDatum(data, numStars);
            AppendDatum(data, solved);
            AppendDatum(data, conceptOfCurrentActivity);
            AppendDatum(data, targetWordOfCurrentActivity);
            AppendDatum(data, initialWordOfCurrentActivity);
            AppendDatum(data, finalSubmittedWord);
            //append the activity delimiter (!) to the end of the string.
            data.Append(activityDelimiter);
            //overwrite the old string to the one that includes the new data
            dataFromPreviouslyCompletedActivities = data.ToString();
        }

        void AppendDatum(StringBuilder data, object datum)
        {
            data.Append(datum);
            data.Append(valDelimiter);
        }
    }

    #endregion
    
    private void Awake()
    {
       
    }

   
    
    public void CreateNewStudentFile(string studentName)
    {
        Debug.Log("Created file for " + studentName);
        PlayerPrefs.SetString(studentName, templateForExperimentWideParams);
    }

    char IntAsChar(int i)
    {
        return (char) (i + asciiFor0);
    }

    int CharAsInt(char c)
    {
        return (int) c - asciiFor0;
    }

    string UpdateSavedSessionAsString(int currentSession, int numStars)
    {
        StringBuilder s = new StringBuilder(templateForExperimentWideParams);

        s[idxOfCurrentSessionInTemplate] = IntAsChar(currentSession);
        s[idxOfNumStarsInTemplate] = IntAsChar(numStars);

        return s.ToString();
    }

    public bool LoadStudentData(string name)
    {
        string studentData = PlayerPrefs.GetString(name);

        bool wasData = studentData != "";
        if (wasData)
        {
            currUser = ParseStudentData(name, studentData);
        }

        return wasData;

        //return true;
    }

    StudentData ParseStudentData(string studentName, string studentData)
    {
        string expWideParams = studentData.Substring(0, templateForExperimentWideParams.Length);

        int lastSession = CharAsInt(expWideParams[idxOfCurrentSessionInTemplate]);
        int numStars = CharAsInt(expWideParams[idxOfNumStarsInTemplate]);
        StudentData data = new StudentData(studentName, lastSession, numStars);
        if (ThereIsDataFromEarlierSessions(studentData))
            data.dataFromPreviouslyCompletedActivities = studentData.Substring(templateForExperimentWideParams.Length,
                studentData.Length - templateForExperimentWideParams.Length);
        return data;
    }


    public bool ThereIsDataFromEarlierSessions(string studentData)
    {
        return studentData.Length > templateForExperimentWideParams.Length;
    }

    void RecordActivityConcept(string concept)
    {
        currUser.conceptOfCurrentActivity = concept;
    }

    public void RecordActivityTargetWord(string word)
    {
        currUser.targetWordOfCurrentActivity = word;
    }

    void RecordActivityInitialWord(string word)
    {
        currUser.initialWordOfCurrentActivity = word;
    }

    public void RecordActivitySolved(bool solved, string finalSubmittedAnswer, bool solvedOnFirstAttempt)
    {
        currUser.solved = (solved ? 1 : 0);
        currUser.numStars += (solvedOnFirstAttempt ? 1 : 0);
        currUser.finalSubmittedWord = finalSubmittedAnswer;
    }


    //will save to player prefs, in case something bad happens.


    public void SaveActivityDataAndClearForNext(string activityTargetWord, string activityInitialWord)
    {
        //store all the activity data (contained in an array) to the string representing all activity data.


        RecordActivityInitialWord(activityInitialWord);
        currUser.AppendDataForCurrentActivityToPreviousData();

        LogActivitySummaryData(currUser.targetWordOfCurrentActivity, currUser.finalSubmittedWord);
    }

    public void UpdateUserSessionAndWriteAllUpdatedDataToPlayerPrefs()
    {
        //update the current session number so that next time we retrieve this students data we set up the right session
        //int nextSession = currUser.CurrentSession + 1; Min wants it to be such that she picks which activities the students do,
        //so for now will just save the session they did this time into the current session field (will communicate basically the last session)

        string experimentWideParametersOfStudent =
            UpdateSavedSessionAsString(currUser.CurrentSession, currUser.numStars);
        StringBuilder studentData = new StringBuilder(experimentWideParametersOfStudent);
        //nbe sure to save the activity data after each activity and just re-save the data when done
        studentData.Append(currUser.dataFromPreviouslyCompletedActivities);
        string updatedData = studentData.ToString();
        PlayerPrefs.SetString(currUser.studentName, updatedData);
    }

    public int GetUsersSession()
    {
        if (!ReferenceEquals(currUser, null))
            return currUser.CurrentSession;
        return 0;
    }

    public void UpdateUsersSession(int currentSession)
    {
        if (!ReferenceEquals(currUser, null))
            currUser.CurrentSession = currentSession;
    }


    public int GetUsersNumStars()
    {
        if (!ReferenceEquals(currUser, null))
            return currUser.numStars;
        return 0;
    }


    //stupid. just going to log the summary data here too.
    public void LogActivitySummaryData(string targetWord, string answer)
    {
        //only log events in activity mode.


        string fileName = currUser.studentName + "_" + currUser.CurrentSession + "_" +
                          SessionsDirector.assessmentStartTime.ToString("yyyyMMdd_hh_mm_ss") + ".csv";

        string filePath = System.IO.Path.Combine(DATA_FILE_DIRECTORY, fileName);
        System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true);

        //thanks http://stackoverflow.com/questions/18757097/writing-data-into-csv-file
        List<string> elements = new List<string>();

        elements.Add(DateTime.Now.ToString("u"));

        elements.Add(targetWord);
        elements.Add(answer);


        var csv = string.Join(",", elements.ToArray());


        file.WriteLine(csv);

        file.Close();
    }


    /*
 * distinct from summary data.
 * events are a record of every input the user put into the program
 * (simmilar to the event logger from caravan)
 * code is ripped from caravan event logger.
 * 
 * 
 * each message is the event we want to log.
 * in caravan, we made it to that each event would correlate with other info
 * but, in this case, we dont need a lot of that because each child is unique.
 * 
 * */


    static string fileName = "";

    void SetFileName()
    {
        //Debug.Log(currUser.studentName);
        fileName = currUser.studentName + "_" + currUser.CurrentSession + "_" + SessionsDirector.assessmentStartTime.ToString ("yyyyMMdd_hh_mm_ss") + ".csv";
    }

    public void LogEvent(string eventName, string eventParam1, string eventParam2)
    {
        //only log events in activity mode.

        if (SessionsDirector.DelegateControlToStudentActivityController)
        {
            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log

            string assessType = (SessionsDirector.IsStudentMode ? "activity" : "assessment");
            if (fileName.Length < 1)
                SetFileName();
            string filePath = System.IO.Path.Combine(LOG_FILE_DIRECTORY, fileName);
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true);

            //thanks http://stackoverflow.com/questions/18757097/writing-data-into-csv-file
            List<string> elements = new List<string>();

            elements.Add(DateTime.Now.ToString("u"));


            elements.Add(assessType);
            elements.Add(eventName);
            elements.Add(eventParam1);
            elements.Add(eventParam2);

            var csv = string.Join(",", elements.ToArray());


            file.WriteLine(csv);

            file.Close();
        }
    }
}