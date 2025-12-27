using UnityEngine;

public enum Audios
{
    EatingApple,
    SnakeDeath,
    Click,
    MovingBox,
    PickupKey,
    SnakeMove
        
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;
    private AudioSource[] audioSources;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundManager = this;
        audioSources = GetComponents<AudioSource>();
    }

    public void PlaySound(Audios audio)
    {
        switch (audio)
        {
            case Audios.EatingApple:
                audioSources[0].pitch = Random.Range(1f - 0.1f, 1f + 0.1f);
                audioSources[0].PlayOneShot(audioSources[0].clip);
                break;
            case Audios.SnakeDeath:
                audioSources[1].PlayOneShot(audioSources[1].clip);
                break;
            case Audios.Click:
                audioSources[2].PlayOneShot(audioSources[2].clip);
                break;
            case Audios.MovingBox:
                audioSources[3].Play();
                break;
            case Audios.PickupKey:
                audioSources[4].PlayOneShot(audioSources[4].clip);
                break;
            case Audios.SnakeMove:
                audioSources[5].pitch = Random.Range(1f - 0.1f, 1f + 0.1f);
                audioSources[5].PlayOneShot(audioSources[5].clip);
                break;
        }
    }

    public void StopSound(Audios audio)
    {
        switch (audio)
        {
            case Audios.MovingBox:
                audioSources[3].Stop();
                break;
        }
    }
    
}
