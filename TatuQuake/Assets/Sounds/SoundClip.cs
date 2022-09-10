using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[System.Serializable]
public class SoundClip
{
    public SoundManager.Sound sound;
    public AudioClip audioClip;

    public bool canLoop;
    public bool hasCooldown;
    public float cooldownTime;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    //0 = sfx, 1 = music, 2 = voice
    public int type;
}
