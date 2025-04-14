using UnityEngine;

public class WindZoneAnimator : MonoBehaviour
{
    public Sprite[] windSprites;
    public float frameRate = 10f;
    public WindZone windZone;

    public float fadeDuration = 0.5f;

    private SpriteRenderer sr;
    private int currentFrame = 0;
    private float timer = 0f;

    private float targetAlpha = 0f;
    private float currentAlpha = 0f;
    private bool wasBlowing = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (windZone == null)
            windZone = GetComponent<WindZone>();
        SetAlpha(0f); // Start invisible
        sr.enabled = true;
    }

    void Update()
    {
        bool isBlowing = windZone != null && windZone.isActive && windZone.IsBlowing;

        // Update target alpha based on wind state
        targetAlpha = isBlowing ? 1f : 0f;

        // Smoothly transition current alpha toward target
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime / fadeDuration);
        SetAlpha(currentAlpha);

        // Skip animation loop if fully invisible
        if (currentAlpha <= 0f || windSprites.Length == 0)
        {
            return;
        }

        // Reset animation if we just started blowing
        if (!wasBlowing && isBlowing)
        {
            currentFrame = 0;
            timer = 0f;
        }

        // Animate sprite frame
        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % windSprites.Length;
            sr.sprite = windSprites[currentFrame];
        }

        wasBlowing = isBlowing;
    }

    private void SetAlpha(float alpha)
    {
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}
