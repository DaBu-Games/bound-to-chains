using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class CameraEffect : MonoBehaviour
{

    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private Image imageEffect;
    [SerializeField] private AudioSource breathing;
    [SerializeField] private float fadeSpeedDurration = 2f;

    public bool startEffect { get; private set; }

    private bool fadingToEnd;
    private Color targetColor;
    private float transitionTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetCameraEffect(); 
    }

    public void StartCameraEffect()
    {
        startEffect = true; 
        imageEffect.enabled = true;
        fadingToEnd = false;
        targetColor = startColor;
        breathing.Play();
    }

    public void ResetCameraEffect()
    {
        imageEffect.enabled = false;
        startEffect = false;
        imageEffect.color = new Color(startColor.r, startColor.g, startColor.b, 0);
        breathing.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if ( startEffect )
        {

            transitionTimer += (fadingToEnd ? 1 : -1) * Time.deltaTime / ( fadeSpeedDurration / 2f);

            // Clamp the timer between 0 and 1
            transitionTimer = Mathf.Clamp01(transitionTimer);

            // Lerp towards the target color
            imageEffect.color = Color.Lerp(startColor, endColor, transitionTimer);

            // Check if the target color has been reached
            if ( Vector4.Distance(imageEffect.color, targetColor) < 0.01f )
            {
                fadingToEnd = !fadingToEnd; 
                targetColor = fadingToEnd ? endColor : startColor;
            }

        }

    }
}
