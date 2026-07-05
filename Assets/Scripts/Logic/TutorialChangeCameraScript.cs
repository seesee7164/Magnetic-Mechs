using Cinemachine;
using UnityEngine;

public class TutorialChangeCameraScript : MonoBehaviour
{
    [Header("Components")]
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineConfiner2D Confiner;
    public Camera Camera;
    [Header("Variables")]
    private bool active = false;
    private bool increase;
    private float largeSize = 14f;
    private float originalSize = 9f;
    private float currSize;
    private float step = .1f;
    private float delay = .0075f;
    private float timer = 0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            active = true;
            increase = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            increase = false;
        }
    }
    private void Awake()
    {
        currSize = originalSize;
    }
    private void FixedUpdate()
    {
        if (active)
        {
            if (timer > delay)
            {
                if ((increase && currSize <= largeSize) || (!increase && currSize >= originalSize))
                {
                    timer = 0f;
                    currSize += (increase ? step : (-1.5f * step));
                    VirtualCamera.m_Lens.OrthographicSize = currSize;
                    Camera.orthographicSize = currSize;
                    Confiner.InvalidateCache();
                }
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }
        }
    }
}
