using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [System.Serializable]
    public class AudioEvent
    {
        public AudioSource audioSource; // The audio source to play
        public float delay;             // The delay before playing it
        public bool startEffect; 
    }

    [SerializeField] private List<AudioEvent> audioEvents = new List<AudioEvent>();
    [SerializeField] private CameraEffect cameraEffect;

    [SerializeField] private FadeIn gameOverScreen;

    private float elapsedTime = 0f;
    private int currentIndex = 0; 
    private bool startTime = false;

    public void StartTimer()
    {
        startTime = true;
    }

    public void StopTimer()
    {
        startTime = false;

        if (currentIndex < audioEvents.Count) 
        {
            audioEvents[currentIndex].audioSource.Stop();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if ( startTime ) 
        {

            elapsedTime += Time.deltaTime;

            Debug.Log( elapsedTime );

            if ( elapsedTime >= audioEvents[currentIndex].delay ) 
            { 
                audioEvents[currentIndex].audioSource.Play();

                if( audioEvents[currentIndex].startEffect && !cameraEffect.startEffect )
                {
                    cameraEffect.StartCameraEffect();
                }

                if ( currentIndex >= audioEvents.Count - 1 )
                {

                    StartCoroutine(WaitForAudio());
                    startTime = false;
                }
                else
                {
                    currentIndex++;
                }
                

            }
        }
    }


    private IEnumerator WaitForAudio()
    {

        yield return new WaitForSeconds( audioEvents[currentIndex].audioSource.clip.length - audioEvents[currentIndex].audioSource.time );

        cameraEffect.ResetCameraEffect();

        gameOverScreen.StartFadeIn();

    }
}
