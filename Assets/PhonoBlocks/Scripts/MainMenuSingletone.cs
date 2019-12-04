using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSingletone : MonoBehaviour
{
    static MainMenuSingletone instance;
	// Use this for initialization
	void Start ()
    {
		if(instance ==null)
        {
            instance = this;
        }
        else if(instance!=this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        
	}

    private void OnLevelWasLoaded(int level)
    {
        if(level == 0)
        {
            instance.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        else
        {
            gameObject.SetActive(false);
        }
    }
}
