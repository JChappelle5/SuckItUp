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

        bool newHeightDetected =
            ((playerRageEvents.CurrentHeight - playerRageEvents.LastHeightCheckpoint) / playerRageEvents.UnityUnitsPerMeter)
            >= playerRageEvents.heightCheckpointInterval;

        behaviorTree.Blackboard["bigFall"] = bigFallDetected;
        behaviorTree.Blackboard["repeatedFalls"] = repeatedFallsDetected;
        behaviorTree.Blackboard["newHeight"] = newHeightDetected;
    }

    void PlayBigFallAudio()
    {
        narratorManager.PlayClipBasedOnFrustration(
            narratorManager.bigFallLowFrustration,
            narratorManager.bigFallMediumFrustration,
            narratorManager.bigFallHighFrustration,
            playerRageEvents.FrustrationLevel
        );
        behaviorTree.Blackboard["bigFall"] = false;
    }

    void PlayRepeatedFallAudio()
    {
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
        behaviorTree.Stop();
    }
}
