using UnityEngine;

public class Zombie : MonoBehaviour
{
    Transform _playerTransform;
    float _moveSpeed = 1f;
    bool _isStunned = false;
    float _stunDuration = 3f;
    float _stunTimer = 0f;

    SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_isStunned)
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0)
            {
                _isStunned = false;
                _moveSpeed = 1f;
                _spriteRenderer.color = Color.red;
            }
            return;
        }

        // 플레이어 쪽으로 느리게 이동
        Vector3 direction = (_playerTransform.position - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;

        //// 플레이어와 거리 체크
        //float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
        //if (distanceToPlayer <= _qteTriggerDistance)
        //{
        //    QTESystem qteSystem = FindFirstObjectByType<QTESystem>();
        //    if (qteSystem != null && !qteSystem.IsQTEActive())
        //    {
        //        qteSystem.StartQTEForStage(qteSystem.GetCurrentStage());
        //    }
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            _isStunned = true;
            _stunTimer = _stunDuration;
            _moveSpeed = 0f;
            _spriteRenderer.color = Color.gray;
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            QTESystem qteSystem = FindFirstObjectByType<QTESystem>();
            if (qteSystem != null && !qteSystem.IsQTEActive())
            {
                qteSystem.StartQTEForStage(qteSystem.GetCurrentStage());
            }
        }
    }
}
