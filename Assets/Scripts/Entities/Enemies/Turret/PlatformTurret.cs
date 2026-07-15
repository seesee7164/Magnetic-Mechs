using UnityEngine;

public class PlatformTurret : Turret
{
    [Tooltip("Is the turret shooting left?")]
    [SerializeField] public bool facingLeft;
    void Start()
    {
        shootingAngle = facingLeft ? 180 : 0;
        SetUpTurret();
        if (!facingLeft) return;
        for (int i = 0; i < maxBullets; i++)
        {
            bulletsArray[i].transform.localScale = new Vector2(bulletsArray[i].transform.localScale.x, bulletsArray[i].transform.localScale.y * -1);
        }
    }
}
