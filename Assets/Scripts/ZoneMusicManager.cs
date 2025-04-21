using System.Collections;
using UnityEngine;

/// <summary>
/// Listens to <see cref="HeightTracker"/> and changes music whenever the
/// player crosses a zone boundary, using short fades and proper stinger logic.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ZoneMusicManager : MonoBehaviour
{
    [System.Serializable]
    public struct ZoneMusic
    {
        public AudioClip transition;   // optional one‑shot “stinger”
        public AudioClip loop;         // mandatory looping track
    }

    [Header("References")]
    public HeightTracker heightTracker;   // drag from your scene
    public AudioSource musicSource;       // same GameObject, Loop = ON

    [Header("Zone Setup")]
    [Tooltip("Meter values where each zone BEGINS (ascending order!).")]
    public float[] zoneBoundaries = { 0f, 11f, 25f };

    [Tooltip("Music for each zone (array length == zoneBoundaries.length).")]
    public ZoneMusic[] zoneMusic;

    [Header("Fade Settings")]
    [Tooltip("Seconds for fade‑out + fade‑in.")]
    [Range(0f, 5f)] public float fadeSeconds = 0.75f;

    int currentZone = -1;        // –1 forces first play
    bool initialized = false;    // block Update until Setup is done
    Coroutine swapRoutine;

    void Start()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;

        // kick off our one‑frame delay + first‑zone setup
        StartCoroutine(SetupInitialMusic());
    }

    IEnumerator SetupInitialMusic()
    {
        // wait one frame so HeightTracker.Update() has run
        yield return null;

        int zoneAtStart = GetZoneIndex(heightTracker.GetCurrentMeters());
        ChangeMusicImmediate(zoneAtStart);

        // now allow Update() to watch for real crossings
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        int newZone = GetZoneIndex(heightTracker.GetCurrentMeters());
        if (newZone == currentZone) return;

        int oldZone = currentZone;
        currentZone = newZone;

        if (swapRoutine != null)
            StopCoroutine(swapRoutine);
        swapRoutine = StartCoroutine(ChangeMusicRoutine(oldZone, newZone));
    }

    int GetZoneIndex(float meters)
    {
        for (int i = zoneBoundaries.Length - 1; i >= 0; i--)
            if (meters >= zoneBoundaries[i]) return i;
        return 0;
    }

    void ChangeMusicImmediate(int zone)
    {
        if (musicSource.isPlaying)
            musicSource.Stop();

        musicSource.clip = zoneMusic[zone].loop;
        musicSource.loop = true;
        musicSource.Play();

        currentZone = zone;
    }

    IEnumerator ChangeMusicRoutine(int oldZone, int targetZone)
    {
        bool ascending = targetZone > oldZone;

        // 1) Fade‑out current loop
        if (musicSource.isPlaying)
        {
            float origVol = musicSource.volume;
            for (float t = 0; t < fadeSeconds; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(origVol, 0f, t / fadeSeconds);
                yield return null;
            }
            musicSource.Stop();
            musicSource.volume = origVol;
        }

        // 2) Play transition only when climbing up
        if (ascending && zoneMusic[targetZone].transition != null)
        {
            musicSource.loop = false;
            musicSource.clip = zoneMusic[targetZone].transition;
            musicSource.Play();
            yield return new WaitWhile(() => musicSource.isPlaying);
        }

        // 3) Fade‑in the new loop
        musicSource.loop = true;
        musicSource.clip = zoneMusic[targetZone].loop;
        musicSource.volume = 0f;
        musicSource.Play();
        for (float t = 0; t < fadeSeconds; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 0.2f, t / fadeSeconds);
            yield return null;
        }
        musicSource.volume = 0.2f;

        swapRoutine = null;
    }
}
