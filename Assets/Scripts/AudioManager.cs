using UnityEngine;

//Gestore dell'audio
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;

    //Effetti sonori relativi allo sparo e al countdown
    public AudioClip shoot;
    public AudioClip countdownTic;
    public AudioClip countdownEnd;

    //Riproduce l'effetto sonoro specificato
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
