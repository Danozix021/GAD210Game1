using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Shooter2D : MonoBehaviour
{
    public PlayerMovement.PlayerId playerId = PlayerMovement.PlayerId.P1;

    [Header("Input (set in Inspector for each player)")]
    [SerializeField] KeyCode[] shootKeys = { KeyCode.T };


    [Header("Shooting")]
    [SerializeField] Bullet2D bulletPrefab;
    [SerializeField] Transform muzzle;
    [SerializeField] float spawnOffset = 0.45f;
    [SerializeField] int shotsPerTurn = 2;

    [Header("References")]
    [SerializeField] AimArrowController aim;

    int shotsLeft;
    PlayerMovement pc;

    void Awake()
    {
        pc = GetComponent<PlayerMovement>();
        if (!aim) aim = GetComponentInChildren<AimArrowController>(true);
    }

    void OnEnable() => shotsLeft = shotsPerTurn;

    void Update()
    {
        if (!pc.IsActive()) return;

        if (ShootPressed())
            TryShoot();
    }

    bool ShootPressed()
    {

        if (shootKeys == null) return false;
        foreach (var k in shootKeys)
            if (k != KeyCode.None && Input.GetKeyDown(k))
                return true;
        return false;

    }
    void TryShoot()
    {
        if (shotsLeft <= 0) return;
        if (!aim || !aim.CanShoot) return;

        Vector2 dir = aim.GetAimDirection();
        Vector3 pos = muzzle ? muzzle.position : transform.position + (Vector3)(dir * spawnOffset);

        Instantiate(bulletPrefab, pos, Quaternion.identity).Fire(dir, transform);
        shotsLeft--;
    }

    public void ResetForTurn() => shotsLeft = shotsPerTurn;
}
