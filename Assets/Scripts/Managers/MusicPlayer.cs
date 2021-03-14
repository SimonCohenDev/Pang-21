using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    AudioSource player;

    public AudioClip GameOver;
    public AudioClip Level;
    public AudioClip Win;
    

    public void PlayLose()
    {
        PlayClip(GameOver, 2);
    }
    public void PlayWin()
    {
        PlayClip(Win, 2);
    }
    public void PlayLevelMusic()
    {
        PlayClip(Level, 2, true);
    }

    private void Awake()
    {
        player = GetComponent<AudioSource>();
        Level.LoadAudioData();

    }

    private void PlayClip(AudioClip clip, float position, bool loop=false)
    {
        player.clip = clip;
        player.time = position;
        player.loop = loop;
        player.Play();
    }
}
