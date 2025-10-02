using UnityEngine;

public class AimArrowController : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField] float orbitRadius;          // how far from player center
    [SerializeField] float degreesPerSecond = 180f;     // sweep speed along the arc
    [SerializeField] bool bounceAtDeadZone = true;      // turn back at the gray sector

    [Header("Dead Zone (the gray sector at the bottom)")]
    [Tooltip("Center of the dead zone in DEGREES. 270 = straight down.")]
    [SerializeField] float deadZoneCenterDeg = 270f;
    [Tooltip("Width of the dead zone in degrees (your 'gray' arc).")]
    [SerializeField] float deadZoneWidthDeg = 90f;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer arrowSprite;
    [SerializeField] float dimAlphaInDeadZone = 0.35f;  // only used if not bouncing

    // Runtime
    float currentAngleDeg = 0f;  // 0=right, 90=up, 180=left, 270=down
    int sweepDir = 1;            // +1/-1 along the arc
    bool aimEnabled = true;

    public bool CanShoot { get; private set; } = true;
    public float CurrentAngleDeg => currentAngleDeg;

    void Awake()
    {
        orbitRadius = 1;
        if (!arrowSprite) arrowSprite = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        ApplyTransforms(); // position & rotate at start
    }

    void Update()
    {
        if (!aimEnabled) return;

        float next = Normalize360(currentAngleDeg + sweepDir * degreesPerSecond * Time.deltaTime);

        float halfDead = deadZoneWidthDeg * 0.5f;
        float relNext = Mathf.DeltaAngle(next, deadZoneCenterDeg);          // [-180..180]
        bool wouldEnterDeadZone = Mathf.Abs(relNext) < halfDead;

        if (bounceAtDeadZone && wouldEnterDeadZone)
        {
            // Clamp to boundary and reverse direction
            float boundary = deadZoneCenterDeg + Mathf.Sign(relNext) * halfDead;
            currentAngleDeg = Normalize360(boundary);
            sweepDir *= -1;
        }
        else
        {
            currentAngleDeg = next;
        }

        // If bouncing, we should never be inside the dead zone; keep this anyway for safety/visuals.
        float rel = Mathf.DeltaAngle(currentAngleDeg, deadZoneCenterDeg);
        bool inDeadZone = Mathf.Abs(rel) <= halfDead;

        CanShoot = !inDeadZone;
        UpdateVisual(inDeadZone);
        ApplyTransforms();
    }

    void ApplyTransforms()
    {
        // ORBIT around the player: set a circular localPosition
        float rad = currentAngleDeg * Mathf.Deg2Rad;
        Vector3 localPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;
        transform.localPosition = localPos;

        // Point the arrow OUTWARD along the orbit (assumes the sprite points UP by default)
        transform.localEulerAngles = new Vector3(0f, 0f, currentAngleDeg - 90f);
    }

    void UpdateVisual(bool inDeadZone)
    {
        if (!arrowSprite) return;
        var c = arrowSprite.color;
        c.a = (inDeadZone && !bounceAtDeadZone) ? dimAlphaInDeadZone : 1f;
        arrowSprite.color = c;
    }

    static float Normalize360(float ang)
    {
        ang %= 360f;
        if (ang < 0f) ang += 360f;
        return ang;
    }

    // Called by TurnManager on turn start for fairness
    public void ResetAngle(float angleDeg)
    {
        currentAngleDeg = Normalize360(angleDeg);
        ApplyTransforms();
    }

    public void SetAimEnabled(bool enabled)
    {
        aimEnabled = enabled;
        if (arrowSprite) arrowSprite.enabled = enabled;
    }

#if UNITY_EDITOR
    // Optional: draw the dead-zone wedge from the player's center (parent)
    void OnDrawGizmosSelected()
    {
        var origin = transform.parent ? transform.parent.position : transform.position;
        float half = deadZoneWidthDeg * 0.5f;
        float a0 = (deadZoneCenterDeg - half) * Mathf.Deg2Rad;
        float a1 = (deadZoneCenterDeg + half) * Mathf.Deg2Rad;

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.25f);
        Vector3 p0 = origin + new Vector3(Mathf.Cos(a0), Mathf.Sin(a0), 0f) * orbitRadius;
        Vector3 p1 = origin + new Vector3(Mathf.Cos(a1), Mathf.Sin(a1), 0f) * orbitRadius;
        Gizmos.DrawLine(origin, p0);
        Gizmos.DrawLine(origin, p1);
        const int steps = 20;
        Vector3 prev = p0;
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            float a = Mathf.Lerp(a0, a1, t);
            Vector3 p = origin + new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * orbitRadius;
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
    }
    public Vector2 GetAimDirection()
    {
        float rad = currentAngleDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }

#endif
}
