using Cinemachine;
using System.Collections;
using UnityEngine;
public class TutorialChangeCameraEndingScript : MonoBehaviour
{
    [Header("Components")]
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineConfiner2D Confiner;
    public Transform originalCameraTarget;
    public Camera Camera;
    public Transform target;
    public Transform playerTransform;
    public Transform alienTransform;
    private CinemachineTransposer virtualTransposer;
    public TutorialChangeCameraScript otherChangeCameraScript;
    [Header("Variables")]
    private bool active = false;
    private bool decrease;
    private float smallSize = 4f;
    private float originalSize = 9f;
    private float currSize;
    private float step = .08f;
    private float delay = .0075f;
    private float timer = 0f;
    [Header("Return Camera")]
    private float baseSpeed = 3f;
    private float currentSpeed = 3f;
    private bool returnToPlayer = true;
    public bool overridden = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            active = true;
            decrease = true;
            UpdateCameraPosition();
            VirtualCamera.Follow = target;
            returnToPlayer = false;
            currentSpeed = baseSpeed;
            overridden = false;
            otherChangeCameraScript.overridden = true;
            currSize = VirtualCamera.m_Lens.OrthographicSize;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            decrease = false;
            returnToPlayer = true;
        }
    }
    private void Awake()
    {
        currSize = originalSize;
        virtualTransposer = VirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }
    private void FixedUpdate()
    {
        if (active && !overridden)
        {
            if (!returnToPlayer) UpdateCameraPosition();
            //setCameraSize();
            if (timer > delay)
            {
                if ((decrease && currSize >= smallSize) || (!decrease && currSize <= originalSize))
                {
                    currSize += (decrease ? (-1.2f * step) : step);
                    VirtualCamera.m_Lens.OrthographicSize = currSize;
                    Camera.orthographicSize = currSize;
                    Confiner.InvalidateCache();
                }
                if (returnToPlayer) ReturnCameraToPlayer();
                timer = 0f;
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }
        }
    }
    private void UpdateCameraPosition()
    {
        target.position = new Vector2((playerTransform.position.x + alienTransform.position.x) / 2, (playerTransform.position.y + alienTransform.position.y) / 2);
    }
    private void ReturnCameraToPlayer()
    {
        if (overridden) return;
        target.position = Vector3.Lerp(target.position, playerTransform.position, currentSpeed * Time.fixedDeltaTime);
        currentSpeed += .2f;
        if ((target.position - playerTransform.position).magnitude <= 1f)
        {
            returnToPlayer = false;
            VirtualCamera.Follow = originalCameraTarget;
            currentSpeed = baseSpeed;
        }
    }
    public void Override()
    {
        overridden = true;
        VirtualCamera.Follow = originalCameraTarget;
    }
}
