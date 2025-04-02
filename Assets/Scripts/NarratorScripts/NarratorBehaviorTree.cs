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
        behaviorTree.Blackboard["bigFall"] = playerRageEvents.BigFallEvent;
        behaviorTree.Blackboard["repeatedFalls"] = playerRageEvents.RepeatedFallEvent;
        behaviorTree.Blackboard["newHeight"] = playerRageEvents.NewHeightEvent; // <-- Directly use the new height event flag
    }

    void PlayBigFallAudio()
    {
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
        narratorManager.PlayClipBasedOnFrustration(
            narratorManager.repeatedFallLowFrustration,
            narratorManager.repeatedFallMediumFrustration,
            narratorManager.repeatedFallHighFrustration,
            playerRageEvents.FrustrationLevel
        );
        playerRageEvents.ResetRepeatedFallEvent();
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
        playerRageEvents.DecreaseFrustration(1f);
        behaviorTree.Blackboard["newHeight"] = false;
    }

    void OnDestroy()
    {
        if (behaviorTree != null)
            behaviorTree.Stop();
    }
}
