using UnityEngine;

public class FadeIn : MonoBehaviour
{

    [SerializeField] private GameObject screen;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Rigidbody2D player; 

    private float fadeTimer = 0f;
    private bool isFadingIn = false;
    private CanvasGroup uiCanvasGroup; 

    private void Start()
    {
        uiCanvasGroup = screen.GetComponent<CanvasGroup>();
        screen.SetActive(false);
        uiCanvasGroup.alpha = 0f;
      
    }

    public void StartFadeIn()
    {
        isFadingIn = true;
        screen.SetActive(true);
        fadeTimer = 0f;
        player.constraints = RigidbodyConstraints2D.FreezeAll;

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
