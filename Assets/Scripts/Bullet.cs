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

    // 좀비 쪽에서 파괴시키는중
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Zombie"))
    //    {
    //        Destroy(gameObject); // 좀비 맞추면 총알 파괴
    //    }
    //}
}
