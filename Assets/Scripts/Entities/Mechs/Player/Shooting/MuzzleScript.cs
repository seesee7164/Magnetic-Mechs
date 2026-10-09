using UnityEngine;

public class MuzzleScript : MonoBehaviour
{
    public Animator animator;
    private bool isRed = false;
    private bool isGreen = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.SetBool("Red", isRed);
        animator.SetBool("Green", isGreen);
    }
    public void ChangeToBlue()
    {
        isRed = false;
        animator.SetBool("Red", isRed);
        isGreen = false;
        animator.SetBool("Green", isGreen);
    }
    public void ChangeToRed()
    {
        isRed = true;
        animator.SetBool("Red", isRed);
        isGreen = false;
        animator.SetBool("Green", isGreen);
    }
    public void ChangeToGreen()
    {
        isRed = false;
        animator.SetBool("Red", isRed);
        isGreen = true;
        animator.SetBool("Green", isGreen);
    }
}
