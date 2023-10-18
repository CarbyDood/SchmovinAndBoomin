using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public SoundClip[] clips;
    private static Dictionary<Sound, float> soundTimerDict;
    private GameObject oneShotGO;
    private AudioSource oneShotSrc;
    public enum Sound
    {
        PistolShot,
        SMGShot,
        ShotgunShot,
        LMGShot,
        NLShot,
        SuperShottyShot,
        SniperShot,
        RPGShot,
        AmmoPickUp,
        WeaponPickUp,
        Jump1,
        Jump2,
        Jump3,
        Jump4,
        Jump5,
        Step1,
        Step2,
        Step3,
        Step4,
        Step5,
        Explosion,
        NadeBounce,
        Rocket,
        ShottyPumpBack,
        ShottyPumpForward,
        SuperShottyPumpBack,
        SuperShottyPumpForward,
        Die1,
        Die2,
        Die3,
        Hurt1,
        Hurt2,
        Hurt3,
        Hurt4,
        Hurt5,
        ArmorPickUp,
        SmallHPPickUp,
        MediumHPPickUp,
        BigHPPickUp,
        SuperShellPickUp,
        TatuPowerPickUp,
        MaxMomentumPickUp,
        PlumberShoesPickUp,
        SawAttack,
        LightingAttack,
        SwordBeamAttack,
        HintNotif,
        SecretFound
    }

    public static SoundManager instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake() 
    {
        //singleton behavoir
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        soundTimerDict = new Dictionary<Sound, float>();
    }

    public void PlaySound(Sound sound, Vector3 position)
    {
        if(CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 60f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.volume = clips[(int)sound].volume;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    public void PlaySound(Sound sound) 
    {
        if(CanPlaySound(sound)){
            if(oneShotGO == null)
            {
                oneShotGO = new GameObject("Sound");
                oneShotSrc = oneShotGO.AddComponent<AudioSource>();
            }
            oneShotSrc.PlayOneShot(GetAudioClip(sound), clips[(int)sound].volume);
        }
    }

    public void PlaySoundAsChild(Sound sound, GameObject obj)
    {
        if(CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = obj.transform.position;
            soundGameObject.transform.parent = obj.transform;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.volume = clips[(int)sound].volume;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    private bool CanPlaySound(Sound sound)
    {
        if(!clips[(int)sound].hasCooldown) return true;
        else
        {
            if(soundTimerDict.ContainsKey(sound))
            {
                float lastTimePlayed = soundTimerDict[sound];
                if (lastTimePlayed + clips[(int)sound].cooldownTime < Time.time)
                {
                    soundTimerDict[sound] = Time.time;
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }
    }

    private AudioClip GetAudioClip(Sound sound)
    {
        if (clips[(int)sound] != null)
        {
            return clips[(int)sound].audioClip;
        }

        Debug.LogError("Sound does not exist!!");
        return null;
    }
}
