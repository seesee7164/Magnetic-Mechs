using UnityEngine;

public class LavaFloorScript : MonoBehaviour
{
    [Header("variables")]
    private float speed = 1.7f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.position += Vector3.up * speed * Time.fixedDeltaTime;
    }
}
