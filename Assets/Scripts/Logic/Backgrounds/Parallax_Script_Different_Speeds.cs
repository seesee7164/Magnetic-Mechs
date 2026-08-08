using UnityEngine;

public class Parallax_Script_Different_Speeds : MonoBehaviour
{
    [Header("Components")]
    public Transform followTransform;
    public int numberOfObjects;
    public Transform[] objectTransforms;
    public Transform objectOneTransform;
    public Transform objectTwoTransform;
    public Transform objectThreeTransform;
    public Transform objectFourTransform;
    public Transform objectFiveTransform;
    public Transform objectSixTransform;
    public Vector2[] objectInitialPositions;
    private Vector2 objectOneInitialPosition;
    private Vector2 objectTwoInitialPosition;
    private Vector2 objectThreeInitialPosition;
    private Vector2 objectFourInitialPosition;
    private Vector2 objectFiveInitialPosition;
    private Vector2 objectSixInitialPosition;

    [Header("Variables")]
    public float parallaxRateOne = 0;
    public float parallaxRateTwo = 0;
    public float parallaxRateThree = 0;
    public float parallaxRateFour = 0;
    public float parallaxRateFive = 0;
    public float parallaxRateSix = 0;
    private void Awake()
    {
        objectTransforms = new Transform[numberOfObjects];
        objectInitialPositions = new Vector2[numberOfObjects];
        if (numberOfObjects >= 1 && objectOneTransform != null)
        {
            objectTransforms[0] = objectOneTransform;
            objectInitialPositions[0] = objectOneTransform.position;
        }
        if (numberOfObjects >= 2 && objectTwoTransform != null)
        {
            objectTransforms[1] = objectTwoTransform;
            objectInitialPositions[1] = objectTwoTransform.position;
        }
        if (numberOfObjects >= 3 && objectThreeTransform != null)
        {
            objectTransforms[2] = objectThreeTransform;
            objectInitialPositions[2] = objectThreeTransform.position;
        }
        if (numberOfObjects >= 4 && objectFourTransform != null)
        {
            objectTransforms[3] = objectFourTransform;
            objectInitialPositions[3] = objectFourTransform.position;
        }
        if (numberOfObjects >= 5 && objectFiveTransform != null)
        {
            objectTransforms[4] = objectFiveTransform;
            objectInitialPositions[4] = objectFiveTransform.position;
        }
        if (numberOfObjects >= 6 && objectSixTransform != null)
        {
            objectTransforms[5] = objectSixTransform;
            objectInitialPositions[5] = objectSixTransform.position;
        }
    }
    void Update()
    {
        for(int i = 0; i < numberOfObjects; i++)
        {
            objectTransforms[i].position = new Vector2(objectInitialPositions[i].x + followTransform.position.x / parallaxRateOne, objectInitialPositions[i].y);
        }
    }
}
