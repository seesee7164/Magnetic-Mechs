using UnityEngine;

public class Camera_Experiment : MonoBehaviour
{
    public Transform player;
    public float cursorInfluence = 0.4f;
    public float cusorClamp = 0.5f;
    public float smoothSpeed = 0.2f;
    private float clampY = 16f;
    private float clampX = 22f;
    [Header("Velocity")]
    private float velocityInfluence = .3f;
    private float velocityMultiplier = 1.2f;
    private float minVelocity = .50f;
    private float maxVelocity = 100f;
    private float facingMultiplier = 1.0f;
    private void LateUpdate()
    {
        if (player == null) return;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Assuming a 2D game, set z to 0
        Vector3 directionToCursor = (mouseWorldPos - player.position);
        if (directionToCursor.magnitude > cusorClamp)
        {
            directionToCursor = directionToCursor.normalized * cusorClamp;
            mouseWorldPos = player.position + directionToCursor;
        }
        Vector3 targetPosition = Vector3.Lerp(player.position, mouseWorldPos, cursorInfluence);

        Vector3 currentVelocity = player.GetComponent<Rigidbody2D>().linearVelocity;
        if (currentVelocity.magnitude > minVelocity)
        {
            if(currentVelocity.magnitude > maxVelocity)
            {
                currentVelocity = currentVelocity.normalized * maxVelocity;
            }
            bool facingRight = player.GetComponent<PlayerScript>().torsoFacingRight;
            Vector3 targetVelocityPos = targetPosition + currentVelocity * velocityMultiplier + Vector3.right * (facingRight ? 1 : -1) * facingMultiplier;
            targetPosition = Vector3.Lerp(targetPosition, targetVelocityPos, velocityInfluence);
        }
        
        targetPosition.z = 0f;

        transform.position = Vector3.Lerp(transform.position, ClampedTarget(targetPosition), smoothSpeed*Time.deltaTime);
    }

    private Vector3 ClampedTarget(Vector3 targetPosition)
    {
        Vector3 returnedPosition = targetPosition;
        if(targetPosition.y - player.position.y > clampY)
        {
            returnedPosition.y = player.position.y + clampY;
        }
        else if (targetPosition.y - player.position.y < -clampY)
        {
            returnedPosition.y = player.position.y - clampY;
        }
        if (targetPosition.x - player.position.x > clampX)
        {
            returnedPosition.x = player.position.x + clampX;
        }
        else if (targetPosition.x - player.position.x < -clampX)
        {
            returnedPosition.x = player.position.x - clampX;
        }
        return returnedPosition;
    }
}
