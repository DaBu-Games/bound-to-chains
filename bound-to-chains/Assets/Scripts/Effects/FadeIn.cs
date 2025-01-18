using UnityEngine;

public class FadeIn : MonoBehaviour
{

    [SerializeField] private CanvasGroup uiCanvasGroup;
    [SerializeField] private float fadeDuration = 2f;

    private float fadeTimer = 0f;
    private bool isFadingIn = false;

    private void Start()
    {

        uiCanvasGroup.alpha = 0f;
      
    }

    public void StartFadeIn()
    {
        isFadingIn = true;
        fadeTimer = 0f;
    }

    private void Update()
    {
        if ( isFadingIn )
        {
            fadeTimer += Time.deltaTime;

            // Lerp the alpha value over time
            uiCanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);

            // Stop fading when we reach full visibility
            if (fadeTimer >= fadeDuration)
            {
                uiCanvasGroup.alpha = 1f;
                isFadingIn = false;
            }

        }
    }
}
