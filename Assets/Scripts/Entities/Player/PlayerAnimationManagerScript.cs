using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
public class PlayerAnimationManagerScript : MonoBehaviour
{
    [Header("Components")]
    public Animator legAnimator;
    public SpriteRenderer legSprite;
    public GameObject legGameObject;
    public GameObject legGameObjectParent;
    public GameObject upperBodyGameObject;
    public SpriteRenderer torsoSprite;
    //public GameObject DeathAnimation;
    [Header("Arms")]
    public GameObject frontForwardArm;
    public GameObject frontUpForwardArm;
    public GameObject frontDownForwardArm;
    public GameObject frontUpArm;
    public GameObject frontDownArm;
    public GameObject backForwardArm;
    public GameObject backUpForwardArm;
    public GameObject backDownForwardArm;
    public GameObject backUpArm;
    public GameObject backDownArm;
    public GameObject forwardArmRotationPoint;
    public GameObject backArmRotationPoint;
    public void setLanding(bool nearGround)
    {
        legAnimator.SetBool("OnGround", nearGround);
        //legAnimator.SetBool("LandingFast", onGround && yVelocity < -5);
    }
    public void setHorizontalSpeed(float xVelocity)
    {
        legAnimator.SetFloat("HorizontalInput", xVelocity);
    }
    public void setAllSpritesColor(Color targetColor)
    {
        legSprite.color = targetColor;
        torsoSprite.color = targetColor;
        frontForwardArm.GetComponent<SpriteRenderer>().color = targetColor;
        backForwardArm.GetComponent<SpriteRenderer>().color = targetColor;
        frontUpForwardArm.GetComponent<SpriteRenderer>().color = targetColor;
        backUpForwardArm.GetComponent<SpriteRenderer>().color = targetColor;
        frontDownForwardArm.GetComponent<SpriteRenderer>().color = targetColor;
        backDownForwardArm.GetComponent<SpriteRenderer>().color = targetColor;
        frontUpArm.GetComponent<SpriteRenderer>().color = targetColor;
        backUpArm.GetComponent<SpriteRenderer>().color = targetColor;
        frontDownArm.GetComponent<SpriteRenderer>().color = targetColor;
        backDownArm.GetComponent<SpriteRenderer>().color = targetColor;
    }
    public void flipLegs(bool facingRight)
    {
        legSprite.flipX = !facingRight;
        legGameObjectParent.transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
        //legGameObjectParent.transform.position = new Vector3(facingRight ? 0 : 180, 0, 0);
    }
    public bool setFiringAngle(float originalAngle)
    {
        float angle = convertToPolarCoordinates(originalAngle);
        if(angle < 0) 
        {
            upperBodyGameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            upperBodyGameObject.transform.localPosition = new Vector3(-.12f, 0f, 0f);
            //forwardArm.transform.localPosition = new Vector3(forwardArm.transform.localPosition.x, forwardArm.transform.localPosition.y, -.6f);
            //backArm.transform.localPosition = new Vector3(backArm.transform.localPosition.x, backArm.transform.localPosition.y, 0f);
        }
        else
        {
            upperBodyGameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            upperBodyGameObject.transform.localPosition = new Vector3(-.05f, 0f, 0f);
            //forwardArm.transform.localPosition = new Vector3(forwardArm.transform.localPosition.x, forwardArm.transform.localPosition.y, -.2f);
            //backArm.transform.localPosition = new Vector3(backArm.transform.localPosition.x, backArm.transform.localPosition.y, 0f);
        }
        float absAngle = Mathf.Abs(angle);
        Vector2 directionAngle;
        setCorrectArms(absAngle);
        if (angle < 0)
        {
            forwardArmRotationPoint.transform.localScale = new Vector3(-1f, 1f, 1f);
            backArmRotationPoint.transform.localScale = new Vector3(-1f, 1f, 1f);
            directionAngle = getDirection(Mathf.Deg2Rad * (angle + 180));
        }
        else
        {
            forwardArmRotationPoint.transform.localScale = new Vector3(1f, 1f, 1f);
            backArmRotationPoint.transform.localScale = new Vector3(1f, 1f, 1f);
            directionAngle = getDirection(Mathf.Deg2Rad * angle);
        }
        forwardArmRotationPoint.transform.right = directionAngle;
        backArmRotationPoint.transform.right = directionAngle;
        return angle >= 0;
    }
    private Vector2 getDirection(float radAngle)
    {
        Vector2 directionAngle = new Vector2(Mathf.Sin(radAngle), Mathf.Cos(radAngle));
        return directionAngle;
    }
    private void setCorrectArms(float absAngle)
    {
        if(absAngle <= 130 && absAngle >= 60)
        {
            frontForwardArm.SetActive(true);
            backForwardArm.SetActive(true);
            frontUpForwardArm.SetActive(false);
            backUpForwardArm.SetActive(false);
            frontDownForwardArm.SetActive(false);
            backDownForwardArm.SetActive(false);
            frontUpArm.SetActive(false);
            backUpArm.SetActive(false);
            frontDownArm.SetActive(false);
            backDownArm.SetActive(false);
        }
        if (absAngle <= 60 && absAngle >= 20)
        {
            frontForwardArm.SetActive(false);
            backForwardArm.SetActive(false);
            frontUpForwardArm.SetActive(true);
            backUpForwardArm.SetActive(true);
            frontDownForwardArm.SetActive(false);
            backDownForwardArm.SetActive(false);
            frontUpArm.SetActive(false);
            backUpArm.SetActive(false);
            frontDownArm.SetActive(false);
            backDownArm.SetActive(false);
        }
        if (absAngle <= 20)
        {
            frontForwardArm.SetActive(false);
            backForwardArm.SetActive(false);
            frontUpForwardArm.SetActive(false);
            backUpForwardArm.SetActive(false);
            frontDownForwardArm.SetActive(false);
            backDownForwardArm.SetActive(false);
            frontUpArm.SetActive(true);
            backUpArm.SetActive(true);
            frontDownArm.SetActive(false);
            backDownArm.SetActive(false);
        }
        if (absAngle <= 160 && absAngle >= 130)
        {
            frontForwardArm.SetActive(false);
            backForwardArm.SetActive(false);
            frontUpForwardArm.SetActive(false);
            backUpForwardArm.SetActive(false);
            frontDownForwardArm.SetActive(true);
            backDownForwardArm.SetActive(true);
            frontUpArm.SetActive(false);
            backUpArm.SetActive(false);
            frontDownArm.SetActive(false);
            backDownArm.SetActive(false);
        }
        if (absAngle >= 160)
        {
            frontForwardArm.SetActive(false);
            backForwardArm.SetActive(false);
            frontUpForwardArm.SetActive(false);
            backUpForwardArm.SetActive(false);
            frontDownForwardArm.SetActive(false);
            backDownForwardArm.SetActive(false);
            frontUpArm.SetActive(false);
            backUpArm.SetActive(false);
            frontDownArm.SetActive(true);
            backDownArm.SetActive(true);
        }
    }
    public float convertToPolarCoordinates(float originalAngle)
    {
        if(originalAngle >= -180 && originalAngle <= -90)
        {
            return -270-originalAngle;
        }
        return 90 - originalAngle;
    }
    public void startDeath()
    {
        this.gameObject.SetActive(false);
    }
    public void turnFrontArmBlue()
    {
        Sprite[] ArmSprites = Resources.LoadAll<Sprite>("PlayerAssets/Player_Gun_Rotation_Front_Arm");
        frontDownArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[0];
        frontDownForwardArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[1];
        frontForwardArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[2];
        frontUpForwardArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[3];
        frontUpArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[4];
    }
    public void turnFrontArmRed()
    {
        Sprite[] ArmSprites = Resources.LoadAll<Sprite>("PlayerAssets/Player_Gun_Rotation_Front_Arm_Red");
        frontDownArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[0];
        frontDownForwardArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[1];
        frontForwardArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[2];
        frontUpForwardArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[3];
        frontUpArm.GetComponent<SpriteRenderer>().sprite = ArmSprites[4];
    }
}
