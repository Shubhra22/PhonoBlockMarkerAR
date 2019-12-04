using UnityEngine;
using System.Collections;

public class Permanent : MonoBehaviour
{

    public int active_level = -1;

    Permanent instance;
    // Use this for initialization
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        else if(instance!= this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void OnLevelWasLoaded(int level)
    {
        if (active_level > -1)
        {
            if (level == active_level)
            {
                gameObject.SetActive(true);

            }
            else
            {

                gameObject.SetActive(false);
            }

        }
    }

}
