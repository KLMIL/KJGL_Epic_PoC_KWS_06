using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public GameObject DieParticlePrefab;

    Transform _playerTransform;
    float _moveSpeed = 1f;
    bool _isStunned = false;
    float _stunDuration = 3f;
    float _stunTimer = 0f;


    public bool IsStunned() => _isStunned;
    public event Action<GameObject, bool> OnStunned; // 무력화 이벤트
    public event Action<Zombie> OnDestroyed;

    SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        // 이벤트 구독자 모두 제거 알림
        OnDestroyed?.Invoke(this);
    }

    private void Update()
    {
        // 총에 맞은 상태라면 일정 시간동안 멈춤
        if (_isStunned)
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0)
            {
                _isStunned = false;
                _moveSpeed = 1f;
                _spriteRenderer.color = Color.red;
                OnStunned?.Invoke(gameObject, false); // 무력화 해제
            }
            return;
        }

        // 플레이어 쪽으로 느리게 이동
        Vector3 direction = (_playerTransform.position - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }

    // 총알에 맞을 때 호출되는 함수
    public void TakeDamage()
    {
        _isStunned = true;
        _stunTimer = _stunDuration;
        _moveSpeed = 0f;
        _spriteRenderer.color = Color.gray;
        OnStunned?.Invoke(gameObject, true); // 무력화 활성화
    }

    // QTE 성공했을 때 호출할 함수
    public void Die()
    {
        var particle = Instantiate(DieParticlePrefab, transform.position, Quaternion.identity);
        Destroy(particle, 0.2f);
        Destroy(gameObject);
    }
}
