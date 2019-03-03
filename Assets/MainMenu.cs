using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public GameObject SoundPanel;
    public AudioMixer mixer;
    public AudioSource audiosource;
    public GameObject MuteButton;
    public GameObject UnMuteButton;



    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
        Application.Quit();
    }

    public void SoundMenu()
    {
        SoundPanel.SetActive(true);
        
    }

    public void ButtonSoundExit()
    {
        SoundPanel.SetActive(false);
    }

    public void  SetVolumeSlider(float Vol)
    {
        mixer.SetFloat("volume",Vol);
    }

    public void VolumeMute()
    {
        MuteButton.SetActive(false);
        UnMuteButton.SetActive(true);
        audiosource.mute = !audiosource.mute;

    }
    public void VolumeUnMute()
    {
        UnMuteButton.SetActive(false);
        MuteButton.SetActive(true);
        audiosource.mute = !audiosource.mute;

    }
}
