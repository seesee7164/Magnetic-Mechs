using UnityEngine;

public class HorizontalMovementScript : MonoBehaviour
{
    [Header("Variables")]
    private float baseSpeed = 15f;
    private float maxSpeed = 11f;
    public float horizontalSpeed;
    public bool facingRight = true;
    public bool movementDisabled;
    [Header("Components")]
    public Rigidbody2D playerRigidBody;
    [Header("Scripts")]
    public PlayerAnimationManagerScript playerAnimationManagerScript;
    public MechActionsScript mechActionsScript;
    public PlayerPhysicsScript playerPhysicsScript;
    public VerticalMovementScript verticalMovementScript;

    public void handleHorizontalMovement(float direction)
    {
        playerAnimationManagerScript.setHorizontalSpeed(Mathf.Abs(direction));
        //animator.SetFloat("HorizontalInput", Mathf.Abs(direction));
        horizontalSpeed = baseSpeed * direction;
        playerRigidBody.AddForce(Vector2.right * horizontalSpeed);
        if ((horizontalSpeed > 0 && !facingRight) || (horizontalSpeed < 0 && facingRight))
        {
            Flip();
        }
        adjustMaxXSpeed();
    }
    void adjustMaxXSpeed()
    {
        //turns on damping if the player is above a certain x speed
        float currentMaxSpeed = mechActionsScript.startMagnetMaxXSpeed(maxSpeed);
        playerPhysicsScript.ApplyMaxHorizontalSpeedDrag(currentMaxSpeed);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        playerAnimationManagerScript.flipLegs(facingRight);
        //transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        verticalMovementScript.remainingFuelImage.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
