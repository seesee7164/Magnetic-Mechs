using UnityEngine;

public class ChangeMechColorScript : MonoBehaviour
{
    [Header("Components")]
    public PlayerAnimationManagerScript playerAnimationManagerScript;
    public SpriteRenderer torsoSprite;
    public DeathAnimatorScript deathAnimatorScript;
    public BulletSpawnerScript bulletSpawnerScript;
    [Header("Variables")]
    public bool isRed = false;
    private void Awake()
    {
        if(isRed) ChangeToRed();
    }
    private void ChangeToRed()
    {
        isRed = true;
        ChangeTorsoToRed();
        deathAnimatorScript.isRed = isRed;
        playerAnimationManagerScript.turnFrontArmRed();
        bulletSpawnerScript.ChangeToRed();
    }
    private void ChangeTorsoToRed()
    {
        Sprite[] TorsoSprites = Resources.LoadAll<Sprite>("PlayerAssets/Player_Walk_Body_Red");
        torsoSprite.sprite = TorsoSprites[0];
    }
    public void ChangeToBlue()
    {
        isRed = false;
        ChangeTorsoToBlue();
        deathAnimatorScript.isRed = isRed;
        playerAnimationManagerScript.turnFrontArmBlue();
        bulletSpawnerScript.ChangeToBlue();
    }
    private void ChangeTorsoToBlue()
    {
        Sprite[] TorsoSprites = Resources.LoadAll<Sprite>("PlayerAssets/Player_Walk_Body");
        torsoSprite.sprite = TorsoSprites[0];
    }
}
