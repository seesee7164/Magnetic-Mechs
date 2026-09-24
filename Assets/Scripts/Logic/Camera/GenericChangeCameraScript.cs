using UnityEngine;
using Unity.Cinemachine;

public class GenericChangeCameraScript : MonoBehaviour
{
    [Header("Components")]
    public CinemachineCamera VirtualCamera;
    public CinemachineConfiner2D Confiner;
    public Camera Camera;
    public GenericChangeCameraScript otherChangeCameraScript;
    [Header("Variables")]
    private bool active = false;
    private bool increase = true;
    private bool ChangedSize;
    public float newSize = 14f;
    private float originalSize = 9f;
    private float currSize;
    private float step = .2f;
    private float delay = .0075f;
    private float timer = 0f;
    public bool overridden = false;
    private void Awake()
    {
        currSize = originalSize;
        increase = newSize >= originalSize;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            active = true;
            ChangedSize = true;
            currSize = VirtualCamera.Lens.OrthographicSize;
            overridden = false;
            if(otherChangeCameraScript != null) otherChangeCameraScript.Override();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            ChangedSize = false;
        }
    }
    private void FixedUpdate()
    {
        if (active && !overridden)
        {
            if (timer > delay)
            {
                if (increase)
                {
                    if ((ChangedSize && currSize <= newSize) || (!ChangedSize && currSize >= originalSize))
                    {
                        currSize += (ChangedSize ? step : (-1.2f * step));
                        UpdateCamera();
                    }
                }
                else
                {
                    if ((ChangedSize && currSize >= newSize) || (!ChangedSize && currSize <= originalSize))
                    {
                        currSize += (ChangedSize ? (-1.2f * step) : step);
                        UpdateCamera();
                    }
                }
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }
        }
    }
    private void UpdateCamera()
    {
        timer = 0f;
        VirtualCamera.Lens.OrthographicSize = currSize;
        Camera.orthographicSize = currSize;
        Confiner.InvalidateBoundingShapeCache();
    }
    public void Override()
    {
        overridden = true;
        //VirtualCamera.Follow = originalCameraTarget;
    }
}
