using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackScript : MonoBehaviour
{
    //script for managing the UI associated with the jetpack
    [Header("Components")]
    public Animator animator;
    [Header("Variables")]
    private float defaultHeight = 1.25f;
    private float defaultWidth = 0.85f;
    private float defaultPosition = -1.05f;
    private float modifiedPosition = -.95f;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void setJetpackDown(bool jetpackOn = false,bool downPressed = false, bool trulyOnGround = false)
    {
        if (downPressed || trulyOnGround)
        {
            animator.SetBool("JetpackOn", jetpackOn);
        }
        else
        {
            animator.SetBool("JetpackOn", true);
            if (jetpackOn)
            {
                //full jetpack
                transform.localPosition = new Vector3(-.07f, defaultPosition, 0);
                transform.localScale = new Vector3(defaultHeight, defaultWidth, 1);
            }
            else
            {
                //half jetpack
                transform.localPosition = new Vector3(-.07f, modifiedPosition, 0);
                transform.localScale = new Vector3(defaultHeight / 2, defaultWidth, 1);
            }
        }
    }
    public void setJetpackBack(bool jetpackOn = false)
    {
        animator.SetBool("JetpackOn", jetpackOn);
    }
}
