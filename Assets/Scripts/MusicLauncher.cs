using System;
using System.Collections.Generic;
using UnityEngine;


public class MusicLauncher:MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource AudioSource2;
    public grumbleAMP grumbleAMP;
    public AudioClip losingW, losingB, lost, warW, warW2, warB, warB2, misc, miscNegotiations, mainMenu, pictMainTheme;

    public void Losing(Owner owner)
    {
        if (owner.Culture == "civilised")
            InterSectionCheck(losingW);
        else
            InterSectionCheck(losingB);
    }
    public void Lost()
    {

        InterSectionCheck(lost);
            
    }
    public void war(Owner owner)
    {
        if (owner.Culture == "civilised")
            InterSectionCheck(warW);
        else
            InterSectionCheck(warB);
    }
    public void warContinuation(Owner owner)
    {
        if (owner.Culture == "civilised")
            InterSectionCheck(warW2);
        else
            InterSectionCheck(warB2);
    }
    public void Negotiations()
    {

        InterSectionCheck(miscNegotiations);
          
    }
    public void Miscellanious(Owner owner)
    {

        InterSectionCheck(misc);
        if (owner.Culture == "barbarian")
        {
            InterSectionCheck(pictMainTheme);

        }
           
    }
         

   private void InterSectionCheck (AudioClip audioClip)
    {
        if (audioSource.isPlaying)
        {
            AudioSource2.volume = 0;
            AudioSource2.PlayOneShot(audioClip);
            while (AudioSource2.volume < .6f)
            {
                AudioSource2.volume += 0.1f;
                audioSource.volume -= 0.1f;
                System.Threading.Thread.Sleep(1000);
            }
            audioSource.Stop();
        }
        else
        {
            audioSource.volume = 0;
            audioSource.PlayOneShot(audioClip);
            while (audioSource.volume < .6f)
            {
                audioSource.volume += 0.1f;
                AudioSource2.volume -= 0.1f;
                System.Threading.Thread.Sleep(1000);
            }
            AudioSource2.Stop();
        }
    }



}

