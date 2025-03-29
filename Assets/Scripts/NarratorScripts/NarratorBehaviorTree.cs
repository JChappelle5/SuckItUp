using UnityEngine;
using NPBehave;

public class NarratorBehaviorTree : MonoBehaviour
{
    private Root behaviorTree;

    [Header("References")]
    public PlayerRageEvents playerRageEvents;
    public NarratorManager narratorManager;

    [Header("Frustration Threshold")]
    public float frustrationThreshold = 5f;

    void Start()
    {
        behaviorTree = new Root(
            new Service(0.2f, UpdateBlackboard,
                new Selector(
                    new BlackboardCondition("bigFall", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        new Action(PlayBigFallAudio)
                    ),
                    new BlackboardCondition("repeatedFalls", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        new Action(PlayRepeatedFallAudio)
                    ),
                    new BlackboardCondition("newHeight", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        new Action(PlayNewHeightAudio)
                    ),
                    new Action(() => { })
                )
            )
        );
        behaviorTree.Start();
    }

    void UpdateBlackboard()
    {
        bool bigFallDetected =
            ((playerRageEvents.HighestPointBeforeFall - playerRageEvents.CurrentHeight) / playerRageEvents.UnityUnitsPerMeter)
            >= playerRageEvents.bigFallThreshold;

        bool repeatedFallsDetected =
            playerRageEvents.ConsecutiveFallCount >= playerRageEvents.repeatFallThreshold;

        // Only trigger new height when player is landed
        bool newHeightDetected = playerRageEvents.IsLanded &&
            ((playerRageEvents.CurrentHeight - playerRageEvents.LastHeightCheckpoint) / playerRageEvents.UnityUnitsPerMeter)
            >= playerRageEvents.heightCheckpointInterval;

        bool isFrustrated = playerRageEvents.FrustrationLevel >= frustrationThreshold;

        behaviorTree.Blackboard["bigFall"] = bigFallDetected;
        behaviorTree.Blackboard["repeatedFalls"] = repeatedFallsDetected;
        behaviorTree.Blackboard["newHeight"] = newHeightDetected;
        behaviorTree.Blackboard["isFrustrated"] = isFrustrated;

        Debug.Log($"Blackboard - BigFall: {bigFallDetected}, RepeatedFalls: {repeatedFallsDetected}, NewHeight: {newHeightDetected}, Frustrated: {isFrustrated}");
    }

    void PlayBigFallAudio()
    {
        Debug.Log("Playing Big Fall Audio!");
        narratorManager.PlayClipBasedOnFrustration(
            narratorManager.bigFallLowFrustration,
            narratorManager.bigFallMediumFrustration,
            narratorManager.bigFallHighFrustration,
            playerRageEvents.FrustrationLevel
        );
        playerRageEvents.ResetBigFallEvent();
        behaviorTree.Blackboard["bigFall"] = false;
    }

    void PlayRepeatedFallAudio()
    {
        Debug.Log("Playing Repeated Fall Audio!");
        narratorManager.PlayClipBasedOnFrustration(
            narratorManager.repeatedFallLowFrustration,
            narratorManager.repeatedFallMediumFrustration,
            narratorManager.repeatedFallHighFrustration,
            playerRageEvents.FrustrationLevel
        );
        behaviorTree.Blackboard["repeatedFalls"] = false;
    }

    void PlayNewHeightAudio()
    {
        Debug.Log("Playing New Height Audio!");
        narratorManager.PlayClipBasedOnFrustration(
            narratorManager.newHeightLowFrustration,
            narratorManager.newHeightMediumFrustration,
            narratorManager.newHeightHighFrustration,
            playerRageEvents.FrustrationLevel
        );
        playerRageEvents.UpdateLastHeightCheckpoint();
        behaviorTree.Blackboard["newHeight"] = false;
        playerRageEvents.DecreaseFrustration(1f);
    }

    void OnDestroy()
    {
        if (behaviorTree != null)
            behaviorTree.Stop();
    }
}
