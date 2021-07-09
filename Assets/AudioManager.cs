using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    Sound lastPlayedSound;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        Play("Start");
    }

    public void Play(string name)
    {
        if(lastPlayedSound != null)
        {
            if (string.Compare(lastPlayedSound.name,name)==0)
            {
                return;
            }
            else
            {
                lastPlayedSound.source.Stop();
            }
        }


        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s==null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");

            return;
        }
        if (lastPlayedSound != null)
        {
            lastPlayedSound.source.Stop();
        }
        s.source.Play();
        lastPlayedSound = s;
    }
}
