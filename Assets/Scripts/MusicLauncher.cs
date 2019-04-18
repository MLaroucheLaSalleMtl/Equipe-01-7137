using System;
using System.Collections.Generic;
using UnityEngine;


public class MusicLauncher:MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource AudioSource2;
    public grumbleAMP grumbleAMP;
    public AudioClip losingW, losingB, lost, warW, warW2, warB, warB2, misc, miscNegotiations, mainMenu, pictMainTheme;
    static Owner owners;
    float songPlayTime;
    float timeElapsed;
    float timeCheck;
    bool changing = false;
    bool changingSource1 = false;
    bool play = false;
    AudioClip AudioClip;

    public void Losing(Owner owner)
    {
        if (owner.Culture == "civilised")
            InterSectionCheck(losingW, owner);
        else
            InterSectionCheck(losingB, owner);
    }
    public void Lost()
    {

        InterSectionCheck(lost, owners);
            
    }
    public void war(Owner owner)
    {
        if (owner.Culture == "civilised")
            InterSectionCheck(warW, owner);
        else
            InterSectionCheck(warB, owner);
    }
    public void warContinuation(Owner owner)
    {
        if (owner.Culture == "civilised")
            InterSectionCheck(warW2, owner);
        else
            InterSectionCheck(warB2, owner);
    }
    public void Negotiations()
    {

        InterSectionCheck(miscNegotiations, owners);
          
    }
    public void Miscellanious(Owner owner)
    {

        
        if (owner.Culture == "barbarian")
        {
            InterSectionCheck(pictMainTheme, owner);

        }
        else
        {
            InterSectionCheck(warW2, owner);
        }
           
    }

    private void Awake()
    {
        AudioSource2.volume = .59f;
        audioSource.volume = 0;
    }

    private void Update()
    {
        if (Time.realtimeSinceStartup > 10)
        {
            timeElapsed = Time.realtimeSinceStartup - timeCheck;
            //UnityEngine.Debug.Log(timeElapsed + "   " + songPlayTime);
            if ((int)timeElapsed == (int)songPlayTime - 5)
            {
                if (AudioClip == warB || AudioClip == warW)
                {
                    this.warContinuation(owners);
                }
                else if(AudioClip == warB2 || AudioClip == warW2)
                {
                    this.war(owners);
                }
                else
                {
                    this.Miscellanious(owners);
                }
            }
            
        }
        
        if (changing)
        {

            if (changingSource1)
            {

                
                if (AudioSource2.volume < .6f)
                {
                   // Debug.Log("@@@@@@@@@@@@@@@@@@@@@");
                    if (play)
                    {
                        AudioSource2.volume = .2f;
                        AudioSource2.PlayOneShot(AudioClip);

                    }
                    var a = (float)(Math.Pow(AudioSource2.volume, 2)) / 100;
                    if (a >= .6f && audioSource.volume > 0)
                    {
                        a = AudioSource2.volume;
                    }
                    var b = (float)(Math.Pow(-audioSource.volume, 2)) / 100;
                    if (audioSource.volume < .2f)
                    {
                        b /= 1.0001f;
                    }
                    
                    AudioSource2.volume += a;
                    audioSource.volume -= b;
                    //System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    changing = false;
                    audioSource.Stop();
                }
                play = false;
            }
            else
            {
               
                if (audioSource.volume < .6f)
                {
                   
                    if (play)
                    {
                       
                        audioSource.volume = .2f;
                        audioSource.PlayOneShot(AudioClip);
                    }
                    var a = (float)(Math.Pow(audioSource.volume, 2)) / 100;
                    if (a >= .6f && AudioSource2.volume >0)
                    {
                        a = audioSource.volume;
                    }
                    var b = (float)(Math.Pow(-AudioSource2.volume, 2)) / 100;
                    if (AudioSource2.volume < .2f)
                    {
                        b /= 1.0001f;
                    }
                    audioSource.volume += a;
                    AudioSource2.volume -= b;
                    //System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    changing = false;
                    AudioSource2.Stop();
                }
                play = false;
                
            }
        }
       
    }
    private void InterSectionCheck (AudioClip audioClip, Owner owner)
    {
        
            owners = GameManager.owners[0];
            songPlayTime = audioClip.length;
            timeCheck = Time.realtimeSinceStartup;
            AudioClip = audioClip;
            play = true;
            if (audioSource.isPlaying)
            {
                changing = true;
                changingSource1 = true;
            }
            else
            {
                changing = true;
                changingSource1 = false;
            }
        
        
    }



}

