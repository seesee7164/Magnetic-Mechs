using UnityEngine;

public class GroundCheckScript : MonoBehaviour
{
    [Header("Variables")]
    public LayerMask inGroundLayer;
    private Vector3 inGroundOffset = new Vector3(0, .25f, 0);
    private float currentFriction = 0f;
    public Vector3 lastPlatformPosition;
    [Header("Timers")]
    private float inGroundTimer = 0f;
    private float inGroundKillTime = .15f;
    [Header("Components")]
    public Rigidbody2D playerRigidBody;
    public GameObject parentMechObject;
    [Header("Scripts")]
    public VerticalMovementScript verticalMovementScript;
    public MechActionsScript mechActionsScript;
    public PlayerHealthScript playerHealthScript;
    public MagnetManagerScript magnetManagerScript;
    public PlankScript handlePlanks;

    [Header("Platform Friction")]
    public PhysicsMaterial2D lowFrictionMaterial;
    public PhysicsMaterial2D highFrictionMaterial;
    [Range(0f, 1000f)] public float frictionLerpSpeed = 10f;
    public float inputMemoryDuration = 0.1f;

    [Header("Ground Checks")]
    public bool onGround = false;
    public bool overlappingGround = false;
    public bool trulyOnGround = false;
    public bool nearGround = false;
    private float groundLength = .9f;
    private float nearGroundLength = 1.25f;
    private float legLength = .78f;
    private Vector3 distanceToLeg = new Vector3(.42f, 0, 0);
    public LayerMask groundLayer;

    [Header("Coyote Time")]
    private float coyoteTime = .1f;
    private float coyoteTimer = 1f;


    void Awake()
    {
        inGroundLayer = LayerMask.GetMask("Ground");
        groundLayer = LayerMask.GetMask("Ground", "Plank Ground");
    }

    private void FixedUpdate()
    {
        coyoteTimer += Time.deltaTime;
    }

    public bool isTrulyOnGround()
    {
        onGround = (Physics2D.Raycast(parentMechObject.transform.position - distanceToLeg, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(parentMechObject.transform.position + distanceToLeg, Vector2.down, groundLength, groundLayer));
        overlappingGround = (Physics2D.Raycast(parentMechObject.transform.position - distanceToLeg, Vector2.down, legLength, groundLayer) || Physics2D.Raycast(parentMechObject.transform.position + distanceToLeg, Vector2.down, legLength, groundLayer));
        trulyOnGround = onGround && !overlappingGround;
        if (trulyOnGround) coyoteTimer = 0;
        return trulyOnGround;
    }
    public void setGroundLayer(float verticalDirection)
    {
        if (verticalDirection <= -.25f && handlePlanks != null)
        {
            groundLayer = LayerMask.GetMask("Ground");
            handlePlanks.disablePlanks();
        }
        else
        {
            {
                groundLayer = LayerMask.GetMask("Ground", "Plank Ground");
            }
        }
    }

    public bool isNearGround()
    {
        nearGround = Physics2D.Raycast(transform.position - distanceToLeg, Vector2.down, nearGroundLength, groundLayer) || Physics2D.Raycast(transform.position + distanceToLeg, Vector2.down, nearGroundLength, groundLayer) && !overlappingGround;
        return nearGround;
    }
    public bool IsOnMovingPlatform(out Rigidbody2D platformRb)
    {
        platformRb = null;
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position - distanceToLeg, Vector2.down, groundLength, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + distanceToLeg, Vector2.down, groundLength, groundLayer);
        RaycastHit2D[] hits = { hitLeft, hitRight };
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("MovingPlatform"))
            {
                platformRb = hit.collider.attachedRigidbody;
                return true;
            }
        }
        return false;
    }

    public void UpdatePlatformFriction()
    {
        if (playerRigidBody == null) return;
        Rigidbody2D platformRb;
        bool onMovingPlatform = IsOnMovingPlatform(out platformRb);


        bool playerInteracting = HasRecentInput();
        //playerInteracting = false;
        //onMovingPlatform = true;
        //Debug.Log($"onMovingPlatform={onMovingPlatform}, playerInteracting={playerInteracting}");
        float targetFriction = (onMovingPlatform && !playerInteracting) ? highFrictionMaterial.friction : lowFrictionMaterial.friction;
        currentFriction = Mathf.Lerp(currentFriction, targetFriction, frictionLerpSpeed * Time.deltaTime);
        if (playerRigidBody.sharedMaterial == null)
        {
            var mat = new PhysicsMaterial2D("runtimeMat") { friction = currentFriction, bounciness = 0f };
            playerRigidBody.sharedMaterial = mat;
        }
        else
        {
            playerRigidBody.sharedMaterial.friction = currentFriction;
        }
        //Debug.Log(onMovingPlatform);
        if (onMovingPlatform && platformRb != null)
        {
            Vector3 platformDelta;
            if (lastPlatformPosition == Vector3.zero)
            {
                lastPlatformPosition = platformRb.transform.position;
            }

            Vector3 tempVec = platformRb.transform.position - lastPlatformPosition;
            platformDelta = tempVec.magnitude > 0.5f ? Vector3.zero : tempVec;
            //Debug.Log($"{platformDelta}, {platformRb.transform.position}, {lastPlatformPosition}");


            if (!playerInteracting && currentFriction >= highFrictionMaterial.friction * 0.5f)
            {
                parentMechObject.transform.position += platformDelta;
            }

            else
            {
                //The goal is to conserve player momentum when moving on the platform
                parentMechObject.transform.position += platformDelta;
            }


            lastPlatformPosition = platformRb.transform.position;
        }
        else
        {
            lastPlatformPosition = Vector3.zero;
        }

        //Debug.Log($"currentFriction={currentFriction}");

    }
    public void CheckIfStuckInGround()
    {
        bool inGround = (Physics2D.Raycast(parentMechObject.transform.position - distanceToLeg / 2 + inGroundOffset, Vector2.down, groundLength, inGroundLayer) || Physics2D.Raycast(parentMechObject.transform.position + distanceToLeg / 2 + inGroundOffset, Vector2.down, groundLength, inGroundLayer));
        if (inGround)
        {
            inGroundTimer += Time.deltaTime;
            if (inGroundTimer >= inGroundKillTime && playerHealthScript != null) playerHealthScript.HandlePlayerDeath(true);
        }
        else inGroundTimer = 0;
    }

    public bool HasRecentInput()
    {
        //Debug.Log(lastMoveInputTime);
        float now = Time.time;

        //Debug.Log($"RecentMove={now - lastMoveInputTime < 0.1f}, " +
        //  $"RecentJump={now - lastJumpInputTime < 0.1f}, " +
        //  $"RecentMagnet={now - lastRepelInputTime < 0.1f || now - lastAttractInputTime < 0.1f}");
        //Debug.Log($"lastMoveInputTime={lastMoveInputTime}, lastJumpInputTime={lastJumpInputTime}, lastRepelInputTime={lastRepelInputTime}, lastAttractInputTime={lastAttractInputTime}, now={now}");
        bool recentMove = now - mechActionsScript.lastMoveInputTime < inputMemoryDuration;
        bool recentJump = now - mechActionsScript.lastJumpInputTime < inputMemoryDuration;
        bool recentMagnet = (now - mechActionsScript.lastRepelInputTime < inputMemoryDuration && mechActionsScript.repelOn && magnetManagerScript.returnMyMagnet() != null) ||
            (now - mechActionsScript.lastAttractInputTime < inputMemoryDuration && mechActionsScript.attractOn && magnetManagerScript.returnMyMagnet() != null);

        return recentMove || recentJump || recentMagnet;
    }
        public bool returnTrulyOnGround()
    {
        return trulyOnGround;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(parentMechObject.transform.position - distanceToLeg, parentMechObject.transform.position - distanceToLeg + Vector3.down * legLength);
        Gizmos.DrawLine(parentMechObject.transform.position + distanceToLeg, parentMechObject.transform.position + distanceToLeg + Vector3.down * groundLength);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(playerObject.transform.position - distanceToLeg, playerObject.transform.position - distanceToLeg + Vector3.down * legLength);
    }
    public bool recentlyGrounded()
    {
        return coyoteTimer <= coyoteTime;
    }
}
