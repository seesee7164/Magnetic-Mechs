using UnityEngine;

public class MuzzleScript : MonoBehaviour
{
    public Animator animator;
    private bool isRed = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        animator.SetBool("Red", isRed);
    }
    public void ChangeToBlue()
    {
        isRed = false;
        animator.SetBool("Red", isRed);
    }
    public void ChangeToRed()
    {
        isRed = true;
        animator.SetBool("Red", isRed);
    }
}
