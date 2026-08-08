using UnityEngine;

public class Parallax_Background_Repeating : MonoBehaviour
{
    [Header("Components")]
    public Transform followTransform;
    public Transform parentTransform;
    [Header("Variables")]
    public float parallaxRate = 9;
    private float offset = 0;
    private void Awake()
    {
        offset = transform.position.x - parentTransform.position.x;
    }
    // Update is called once per frame
    void Update()
    {
        float relativeDistance = followTransform.position.x - parentTransform.position.x;
        gameObject.transform.position = new Vector2(offset + parentTransform.position.x + relativeDistance / parallaxRate, 0);
    }
}
