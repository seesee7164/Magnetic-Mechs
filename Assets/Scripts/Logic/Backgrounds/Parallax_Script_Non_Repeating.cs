using UnityEngine;

public class Parallax_Script_Non_Repeating : MonoBehaviour
{
    [Header("Components")]
    public Transform followTransform;
    [Header("Variables")]
    public float parallaxRate = 9;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector2(followTransform.position.x/parallaxRate,0);
    }
}