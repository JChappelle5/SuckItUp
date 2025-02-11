using UnityEngine;

public class PlungerController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isSuckedIn", false); // Ensure it starts in Normal state
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // HOLD Left mouse = Stay in "SuckedIn"
        {
            animator.SetBool("isSuckedIn", true);
        }
        else // RELEASE left mouse = Move to "StretchedOut" then Reset
        {
            animator.SetBool("isSuckedIn", false);
        }
    }

    void ResetPlunger()
    {
        animator.Play("Plunger_Normal"); // Force the animation back to Normal
    }
}
