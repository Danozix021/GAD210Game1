using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet2D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float lifeSeconds;
    [SerializeField] int maxBounces = 1;
    [SerializeField] LayerMask bounceLayers; // e.g. "Walls"
    [SerializeField] LayerMask playerLayers; // e.g. "Player"

    Rigidbody2D rb;
    int bouncesUsed;
    float life;

    Transform owner; // shooter to ignore self-hits

    public void Fire(Vector2 dir, Transform shooter)
    {
        owner = shooter;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = dir.normalized * speed;
        life = lifeSeconds;
    }

    void Awake()
    {
        speed = 7.5f;
        lifeSeconds = 1.2f;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int hitLayer = col.gameObject.layer;

        
        if (((1 << hitLayer) & bounceLayers) != 0)
        {
            if (bouncesUsed < maxBounces)
            {
                Vector2 n = col.GetContact(0).normal;
                rb.velocity = Vector2.Reflect(rb.velocity, n);
                bouncesUsed++;
                return;
            }
        }

        
        if (((1 << hitLayer) & playerLayers) != 0)
        {
            // ignore shooter
            if (col.transform == owner) return;

           
            Debug.Log($"Player {col.transform.name} hit!");
            Destroy(gameObject);

            // TODO: Call round-end logic here
            // Example:
            // FindObjectOfType<TurnManager>().RoundOver(col.transform);
            return;
        }

        
        Destroy(gameObject);
    }
}
