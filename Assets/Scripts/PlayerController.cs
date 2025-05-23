using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D _rb;
    float _moveSpeed = 5f;
    public Gun Gun;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float moveInput = 0f;
        if (Input.GetKey("a"))
        {
            moveInput = -1f;
        }
        if (Input.GetKey("d"))
        {
            moveInput = 1f;
        }
        _rb.linearVelocity = new Vector2(moveInput * _moveSpeed, _rb.linearVelocity.y);
    }
}
