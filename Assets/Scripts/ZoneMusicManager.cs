using System.Collections;
using UnityEngine;

/// <summary>
/// Listens to <see cref="HeightTracker"/> and changes music whenever the
/// player crosses a zone boundary, using short fades instead of hard cuts.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ZoneMusicManager : MonoBehaviour
{
    // ───────────────────────────── data struct ──────────────────────────────
    [System.Serializable]
    public struct ZoneMusic
    {
        public AudioClip transition;   // optional one‑shot “stinger”
        public AudioClip loop;         // mandatory looping track
    }

    // ───────────────────────── Inspector fields ─────────────────────────────
    [Header("References")]
    public HeightTracker heightTracker;        // drag from the scene
    public AudioSource musicSource;          // same GO, “Loop” ON

    [Header("Zone Setup")]
    [Tooltip("Meter values where each zone BEGINS (ascending order!).")]
    public float[] zoneBoundaries = { 0f, 11f, 25f };

    [Tooltip("Music for each zone (array length must equal zoneBoundaries).")]
    public ZoneMusic[] zoneMusic;

    [Header("Fade Settings")]
    [Tooltip("Seconds for fade‑out + fade‑in.")]
    [Range(0f, 5f)] public float fadeSeconds = 0.75f;

    // ───────────────────────── internal state ───────────────────────────────
    int currentZone = -1;        // –1 forces first play
    Coroutine swapRoutine;

    // ───────────────────────── Unity life‑cycle ─────────────────────────────
    void Start()
    {
        if (musicSource == null) musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;

        // Start zone‑0 immediately (hard cut at launch only)
        ChangeMusic(GetZoneIndex(heightTracker.GetCurrentMeters()), forceCut: true);
    }

    void Update()
    {
        int zone = GetZoneIndex(heightTracker.GetCurrentMeters());
        if (zone != currentZone && swapRoutine == null)
            swapRoutine = StartCoroutine(ChangeMusicRoutine(zone));
    }

    // ───────────────────────── helpers ──────────────────────────────────────
    int GetZoneIndex(float meters)
    {
        for (int i = zoneBoundaries.Length - 1; i >= 0; i--)
            if (meters >= zoneBoundaries[i]) return i;
        return 0;
    }

    void ChangeMusic(int targetZone, bool forceCut)
    {
        if (forceCut && musicSource.isPlaying) musicSource.Stop();

        // Play transition (ascending only)
        if (targetZone > currentZone && zoneMusic[targetZone].transition != null)
            musicSource.PlayOneShot(zoneMusic[targetZone].transition);

        // Start new loop
        musicSource.clip = zoneMusic[targetZone].loop;
        musicSource.loop = true;
        musicSource.Play();

        currentZone = targetZone;
    }

    IEnumerator ChangeMusicRoutine(int targetZone)
    {
        bool ascending = targetZone > currentZone;

        /* 1) fade‑out current loop (if any) */
        if (musicSource.isPlaying)
        {
            float startVol = musicSource.volume;
            for (float t = 0; t < fadeSeconds; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeSeconds);
                yield return null;
            }
            musicSource.Stop();
            musicSource.volume = startVol;               // restore for later
        }

        /* 2) play transition stinger if we’re climbing up */
        if (ascending && zoneMusic[targetZone].transition != null)
        {
            musicSource.loop = false;
            musicSource.clip = zoneMusic[targetZone].transition;
            musicSource.Play();
            yield return new WaitWhile(() => musicSource.isPlaying);
        }

        /* 3) fade‑in the new loop */
        musicSource.loop = true;
        musicSource.clip = zoneMusic[targetZone].loop;
        musicSource.volume = 0f;                         // start silent
        musicSource.Play();

        for (float t = 0; t < fadeSeconds; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 0.2f, t / fadeSeconds);
            yield return null;
        }
        musicSource.volume = 0.2f;                         // ensure full volume

        currentZone = targetZone;
        swapRoutine = null;
    }
}
