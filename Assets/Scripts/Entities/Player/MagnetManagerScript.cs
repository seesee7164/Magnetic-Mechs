using UnityEngine;
using System;

public class MagnetManagerScript : MonoBehaviour
{
    [Header("Components")]
    public GameObject MagnetSpawner;
    public AudioSource magnetAttractionAudio;
    public AudioSource magnetRepulsionAudio;
    private GameObject myMagnet;
    public MagnetVisualEffectScript magnetVisualEffectScript;
    public Rigidbody2D playerRigidBody;


    [Header("Scripts")]
    private MagnetSpawnerScript magnetSpawnerScript;
    public PlayerAnimationManagerScript playerAnimationManagerScript;

    [Header("Variables")]
    private float magnetDistanceMultiplyingForceRepulsion = 98;
    private float magnetDistanceMultiplyingForceAttraction = -87;
    private float magnetBaseForceRepulsion = 7f;
    private float magnetBaseForceAttraction = -7f;
    private float maximumMagnetDistance = 30;
    private Vector2 magnetRelativePosition;
    public float magnetDistance;
    public bool magnetActive = false;
    public bool attracting = false;
    void Awake()
    {
        MagnetSpawner = GameObject.FindGameObjectWithTag("MagnetSpawner");
        magnetSpawnerScript = MagnetSpawner.GetComponent<MagnetSpawnerScript>();
    }

    public void setMagnet(GameObject magnet)
    {
        myMagnet = magnet;
        magnetVisualEffectScript.Magnet = magnet;
    }
    public void SetMagnetAudio(bool repelOn, bool attractOn)
    {
        if (magnetAttractionAudio != null && magnetRepulsionAudio != null)
        {
            if (magnetSpawnerScript.magnetActive && (repelOn ^ attractOn))
            {
                if (repelOn)
                {
                    if (!magnetRepulsionAudio.isPlaying) magnetRepulsionAudio.Play();
                }
                if (attractOn)
                {
                    if (!magnetAttractionAudio.isPlaying) magnetAttractionAudio.Play();
                }
                magnetActive = true;
            }
            else
            {
                magnetAttractionAudio.Stop();
                magnetRepulsionAudio.Stop();
                magnetActive = false;
            }
        }
    }
    public void launchMagnet()
    {
        magnetSpawnerScript.Launch();
    }
    public void playerKilled()
    {
        if (magnetAttractionAudio != null && magnetAttractionAudio.isPlaying) magnetAttractionAudio.Stop();
        if (magnetRepulsionAudio != null && magnetRepulsionAudio.isPlaying) magnetRepulsionAudio.Stop();
    }
    public void setMagnetSpawnerAngle(Vector2 firingAngle)
    {
        MagnetSpawner.transform.right = firingAngle;
    }
    public void handleMagneticRepulsion(bool repelOn, bool attractOn)
    {
        if (myMagnet == null || !(repelOn ^ attractOn) || !magnetSpawnerScript.magnetActive) return;
        if (attractOn) attracting = true;
        else attracting = false;
        magnetRelativePosition = transform.position - myMagnet.transform.position;
        magnetDistance = magnetRelativePosition.magnitude;
        if (magnetDistance < 1.1f)
        {
            magnetDistance = 1.1f;
        }
        if (magnetDistance < (maximumMagnetDistance))
        {
            float size = 1 - 1.5f * (magnetDistance - 1.1f) / maximumMagnetDistance;
            size = Mathf.Max(size, 0f);
            magnetVisualEffectScript.StartMagnetEffect(repelOn, size);
            applyMagnetism(magnetRelativePosition.normalized, magnetDistance, attractOn);
        }
    }
    void applyMagnetism(Vector2 forceDirection, float magnetDistance, bool attractOn)
    {
        float forceMagnitude = 1 / (float)Math.Sqrt(magnetDistance);
        if (attractOn)
        {
            forceMagnitude *= magnetDistanceMultiplyingForceAttraction;
            forceMagnitude += magnetBaseForceAttraction;
        }
        else
        {
            forceMagnitude *= magnetDistanceMultiplyingForceRepulsion;
            forceMagnitude += magnetBaseForceRepulsion;
        }
        playerRigidBody.AddForce(forceDirection * forceMagnitude, ForceMode2D.Force);
    }
    public float getMagnetMaxYSpeed(float maxYSpeed, float maxYSpeedPressingDown, bool pressingDown, bool repelOn, bool attractOn)
    {
        float currentMaxSpeed = maxYSpeed;
        if (pressingDown)
        {
            currentMaxSpeed = maxYSpeedPressingDown;
        }
        if (repelOn ^ attractOn)
        {
            if (myMagnet != null && magnetSpawnerScript.magnetActive)
            {
                Vector2 magnetRelativePosition = transform.position - myMagnet.transform.position;
                float magnetDistance = magnetRelativePosition.magnitude;
                if (magnetDistance < (maximumMagnetDistance))
                {
                    float angle = Mathf.Atan2(magnetRelativePosition.y, magnetRelativePosition.x);
                    float sinAngle = Mathf.Sin(angle);
                    float sign = MathF.Abs(playerRigidBody.linearVelocity.y) / playerRigidBody.linearVelocity.y;
                    if (attractOn) sign *= -1;//if attract is on then we want the + in the next step to be a minus
                    currentMaxSpeed = maxYSpeed * MathF.Abs(sign + 4 * sinAngle * MathF.Sqrt((maximumMagnetDistance - magnetDistance) / maximumMagnetDistance));
                }
            }
        }
        return currentMaxSpeed;
    }
    public float getMagnetMaxXSpeed(float maxXSpeed, bool repelOn, bool attractOn)
    {
        float currentMaxSpeed = maxXSpeed;
        if (repelOn ^ attractOn)
        {
            if (magnetSpawnerScript != null && magnetSpawnerScript.magnetActive)
            {
                Vector2 magnetRelativePosition = transform.position - myMagnet.transform.position;
                float magnetDistance = magnetRelativePosition.magnitude;
                if (magnetDistance < (maximumMagnetDistance))
                {
                    float angle = Mathf.Atan2(magnetRelativePosition.y, magnetRelativePosition.x);
                    float cosAngle = Mathf.Cos(angle);
                    float sign = MathF.Abs(playerRigidBody.linearVelocity.x) / playerRigidBody.linearVelocity.x;
                    if (attractOn) sign *= -1;//if attract is on then we want the + in the next step to be a minus
                    currentMaxSpeed = maxXSpeed * MathF.Abs(sign + 4 * cosAngle * MathF.Sqrt((maximumMagnetDistance - magnetDistance) / maximumMagnetDistance));
                }
            }
        }
        return currentMaxSpeed;
    }
    public GameObject returnMyMagnet()
    {
        return myMagnet;
    }
    public float returnMagnetMaximumDistance()
    {
        return maximumMagnetDistance;
    }
    public float returnPolarMagnetAngle()
    {
        float originalAngle = Mathf.Atan2(magnetRelativePosition.y, magnetRelativePosition.x) * Mathf.Rad2Deg;
        return playerAnimationManagerScript.convertToPolarCoordinates(originalAngle);
    }
}
