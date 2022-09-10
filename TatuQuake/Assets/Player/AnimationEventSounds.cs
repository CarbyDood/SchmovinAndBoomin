using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSounds : MonoBehaviour
{
    public void PlayStepAudio()
    {
        int rando = Random.Range(0, 4);
        if(rando == 0) SoundManager.instance.PlaySound(SoundManager.Sound.Step1);
        else if(rando == 1) SoundManager.instance.PlaySound(SoundManager.Sound.Step2);
        else if(rando == 2) SoundManager.instance.PlaySound(SoundManager.Sound.Step3);
        else if(rando == 3) SoundManager.instance.PlaySound(SoundManager.Sound.Step4);
        else if(rando == 4) SoundManager.instance.PlaySound(SoundManager.Sound.Step5);
    }
}
