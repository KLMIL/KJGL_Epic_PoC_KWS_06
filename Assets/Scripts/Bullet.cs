using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 10f;
    public Vector3 Direction;
    public float Lifetime = 2f;

    public void SetDirection(Vector3 dir)
    {
        Direction = dir;
    }

    private void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            // 이미 stun 상태인 좀비는 지나침
            if (collision.GetComponent<Zombie>().IsStunned()) return;

            Destroy(gameObject);
            collision.gameObject.GetComponent<Zombie>().TakeDamage();
        }
    }
}
