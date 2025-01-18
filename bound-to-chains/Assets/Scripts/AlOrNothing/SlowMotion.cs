using UnityEngine;

public class SlowMotion : MonoBehaviour
{

    [SerializeField] private float slowMotionFactor = 0.2f;
    [SerializeField] private float transitionSpeed = 5f;
    [SerializeField] private AudioSource breathing; 

    private bool isInSlowMotion = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (isInSlowMotion)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionFactor, transitionSpeed * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, transitionSpeed * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.CompareTag("Player") )
        {
            isInSlowMotion = true;
            breathing.loop = false; 
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInSlowMotion = false;
        }
    }
}
