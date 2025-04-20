using System.Collections;
using UnityEngine;

/// <summary>
/// Watches the player’s height (via HeightTracker) and
/// cross‑fades music when the zone changes.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ZoneMusicManager : MonoBehaviour
{
    [System.Serializable]
    public struct ZoneMusic
    {
        public AudioClip transition;   // one‑shot stinger (optional)
        public AudioClip loop;         // main loop (required)
    }

    [Header("References")]
    public HeightTracker heightTracker;    // drag the HeightTracker from the scene
    public AudioSource musicSource;        // same GameObject, set Loop = true in Inspector

    [Header("Zone Setup")]
    [Tooltip("Meters at which each new zone begins — must be ascending!")]
    public float[] zoneBoundaries = { 0f, 30f, 60f };   // 0‑29.9 = zone‑0, 30‑59.9 = zone‑1, etc.

    [Tooltip("Music for each zone (array size must match zoneBoundaries length)")]
    public ZoneMusic[] zoneMusic;

    // ––––––––––––––––––––––––––––––––––––––––––––––––

    int currentZone = -1;        // “–1” forces first StartMusic call
    Coroutine swapRoutine;

    void Start()
    {
        if (musicSource == null) musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;                     // loops for main tracks
        UpdateZone(forceInstant: true);              // start immediately
    }

    void Update()
    {
        UpdateZone(forceInstant: false);             // poll every frame
    }

    // ------------------------------------------------
    void UpdateZone(bool forceInstant)
    {
        int newZone = GetZoneIndex(heightTracker.GetCurrentMeters());

        if (newZone == currentZone) return;           // still in same zone

        if (swapRoutine != null) StopCoroutine(swapRoutine);
        swapRoutine = StartCoroutine(ChangeMusicRoutine(newZone, forceInstant));
    }

    int GetZoneIndex(float meters)
    {
        // zoneBoundaries[i] is the FIRST meter value that belongs to zone i
        for (int i = zoneBoundaries.Length - 1; i >= 0; i--)
            if (meters >= zoneBoundaries[i]) return i;

        return 0; // shouldn’t happen
    }

    IEnumerator ChangeMusicRoutine(int targetZone, bool instant)
    {
        // Step 1 – let the currently playing loop finish (unless this is the first time or “instant”)
        if (!instant && musicSource.isPlaying)
            yield return new WaitWhile(() => musicSource.isPlaying);

        // Step 2 – play transition clip if one exists
        AudioClip trans = zoneMusic[targetZone].transition;
        if (trans != null)
        {
            musicSource.loop = false;
            musicSource.clip = trans;
            musicSource.Play();
            yield return new WaitWhile(() => musicSource.isPlaying);
        }

        // Step 3 – switch to the main loop for the new zone
        musicSource.loop = true;
        musicSource.clip = zoneMusic[targetZone].loop;
        musicSource.Play();

        currentZone = targetZone;
        swapRoutine = null;
    }
}