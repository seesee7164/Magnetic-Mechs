using UnityEngine;

public class DeathAnimatorScript : MonoBehaviour
{
    public Animator animator;
    public bool isRed = false;

    private void OnEnable()
    {
        animator.SetBool("Red", isRed);
    }
}
