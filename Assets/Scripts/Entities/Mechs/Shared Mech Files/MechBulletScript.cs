using UnityEngine;

public class MechBulletScript : BulletScript
{
    [Header("Red")]
    private bool isRed = false;
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
