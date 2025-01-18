using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class PlayAudio : MonoBehaviour
{
    [SerializeField] private AudioSource wakeUp;
    [SerializeField] private AudioSource threeMin;
    [SerializeField] Timer timer; 

    private bool played = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.CompareTag("Player") && !played)
        {
            wakeUp.Play();

            played = true;

            StartCoroutine( PlaySecondAudio() );
        }

    }


    private IEnumerator PlaySecondAudio()
    {
        // Wait until the first audio finishes
        yield return new WaitForSeconds(wakeUp.clip.length - wakeUp.time);

        // Play the second audio
        threeMin.Play();

        StartCoroutine( WaitForAudio() );

    }

    private IEnumerator WaitForAudio()
    {

        yield return new WaitForSeconds(threeMin.clip.length - threeMin.time);

        timer.StartTimer();

    }
}