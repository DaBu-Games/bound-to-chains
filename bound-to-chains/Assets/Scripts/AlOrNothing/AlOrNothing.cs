using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AlOrNothing : MonoBehaviour
{
    [SerializeField] private Collider2D invisibleWall; 
    [SerializeField] private GameObject objectToSpawn; // The prefab to spawn
    [SerializeField] private Transform spawnPoint;    // Where the object will spawn
    [SerializeField] private float throwForce = 10f;  // Force to throw the object
    [SerializeField] private AudioSource lastJump1;
    [SerializeField] private AudioSource lastJump2;
    [SerializeField] private CameraEffect cameraEffect;
    [SerializeField] private Timer timer;

    private bool played = false; 

    private void OnTriggerEnter2D( Collider2D collider )
    {

        if ( collider.CompareTag("Player") && !played )
        {

            collider.gameObject.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = false;

            cameraEffect.StartCameraEffect();

            invisibleWall.isTrigger = true;

            timer.StopTimer();

            lastJump1.time = 0.6f;
            lastJump1.Play();

            // Spawn the object
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);

            // Calculate the direction to the player
            Vector3 directionToPlayer = ( collider.transform.position - spawnPoint.position ).normalized;

            // Get the Rigidbody of the spawned object and apply force
            spawnedObject.GetComponent<Rigidbody2D>().AddForce( directionToPlayer * throwForce, ForceMode2D.Impulse );

            played = true;

            StartCoroutine( PlaySecondAudio() );
        }
    }

    private IEnumerator PlaySecondAudio()
    {
        // Wait until the first audio finishes
        yield return new WaitForSeconds(lastJump1.clip.length - lastJump1.time);

        // Play the second audio
        lastJump2.Play();

    }
}
