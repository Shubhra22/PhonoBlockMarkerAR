using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class AudioSourceController : MonoBehaviour
{

    static AudioSource source;
    static readonly string RESOURCES_WORD_PATH = "audio/words/";
    static readonly string RESOURCES_SOUNDED_OUT_WORD_PATH = "audio/sounded_out_words/";
    static LinkedList<AudioClip> bufferedClips = new LinkedList<AudioClip>();
    static AudioSourceController instance;

    void Start()
    {
        if (source == null)
            source = gameObject.GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public static AudioClip GetSoundedOutWordFromResources(string word)
    {
        StringBuilder path = new StringBuilder(RESOURCES_SOUNDED_OUT_WORD_PATH);
        path.Append(word);
        return (AudioClip)Resources.Load(path.ToString(), typeof(AudioClip));

    }

    public static AudioClip GetClipFromResources(string path)
    {

        return (AudioClip)Resources.Load(path, typeof(AudioClip));

    }

    public static AudioClip GetWordFromResources(string word)
    {

        return (AudioClip)Resources.Load(RESOURCES_WORD_PATH + word, typeof(AudioClip));

    }


    //play the first, when it's done, play the second.
    //return true if the argument clip was not null; false otherwise
    public static bool PushClip(AudioClip next)
    {
        if (next != null)
        {
            bufferedClips.AddLast(next);
            return true;
        }
        return false;


    }

    void Update()
    {
        if (bufferedClips.Count > 0)
        {
            if (!source.isPlaying)
            {
                Play(bufferedClips.First.Value);
                bufferedClips.RemoveFirst();
            }
        }

    }

    static void Play(AudioClip clip)
    {


        if (!source.isPlaying)
        {
            source.clip = clip;
            source.Play();
        }

    }

}