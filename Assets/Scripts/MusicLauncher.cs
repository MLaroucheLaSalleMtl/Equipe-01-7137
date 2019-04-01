using System;
using System.Collections.Generic;
using UnityEngine;


public class MusicLauncher:MonoBehaviour
{
    public AudioSource audioSource;
    public grumbleAMP grumbleAMP;
    public AudioClip losingW, losingB, lost, warW, warW2, warB, warB2, misc, miscNegotiations, mainMenu, pictMainTheme;

    public void Losing(Owner owner)
    {
        if (owner.Culture == "civilised")
            audioSource.PlayOneShot(losingW);
        else
            audioSource.PlayOneShot(losingB);
    }
    public void Lost()
    {
            
        audioSource.PlayOneShot(lost);
            
    }
    public void war(Owner owner)
    {
        if (owner.Culture == "civilised")
            audioSource.PlayOneShot(warW);
        else
            audioSource.PlayOneShot(warB);
    }
    public void warContinuation(Owner owner)
    {
        if (owner.Culture == "civilised")
            audioSource.PlayOneShot(warW2);
        else
            audioSource.PlayOneShot(warB2);
    }
    public void Negotiations()
    {
            
        audioSource.PlayOneShot(miscNegotiations);
          
    }
    public void Miscellanious(Owner owner)
    {
           
        audioSource.PlayOneShot(misc);
        if (owner.Culture == "barbarian")
        {
            audioSource.PlayOneShot(pictMainTheme);

        }
           
    }


    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            //need logic to know player owner
            //if (Owner == isAtWar)
            //{
            //    warContinuation(owner)
            //}
        }
    }



}

