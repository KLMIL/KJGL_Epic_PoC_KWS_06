using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Gun Gun;
    QTESystem _qteSystem;

    Rigidbody2D _rb;
    float _moveSpeed = 5f;

    
    [SerializeField] List<GameObject> _nearZombies = new List<GameObject>(); // QTE 사거리 내 좀비
    [SerializeField] List<GameObject> _stunnedZombies = new List<GameObject>(); // 무력화 좀비
    [SerializeField] HashSet<Zombie> _subscribedZombies = new HashSet<Zombie>(); // 구독 추적


    // Get 함수
    //public List<GameObject> GetNearZombies() => _nearZombies;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _qteSystem = QTESystem.Instance;
    }

    private void OnDestroy()
    {
        foreach (var zombie in _subscribedZombies)
        {
            if (zombie != null) zombie.OnStunned -= HandleZombieStunned;
        }
    }

    private void Update()
    {
        // A, D로 플레이어 이동
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        _rb.linearVelocity = new Vector2(moveInput * _moveSpeed, _rb.linearVelocity.y);

        // QTE 트리거 : 근처 stun 좀비가 있고, QTE 비활성화 상태
        if (_stunnedZombies.Count > 0 && !_qteSystem.IsQTEActive())
        {
            GameObject closestZombie = null;
            float minDistance = float.MaxValue;
            foreach (var zombie in _stunnedZombies)
            {
                float distance = Vector3.Distance(transform.position, zombie.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestZombie = zombie;
                }
            }
            _qteSystem.StartQTE(closestZombie);
        }
    }


    // 좀비가 트리거에 진입하면 리스트에 추가
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            Zombie zombieScript = collision.GetComponent<Zombie>();

            SubscribeToStunnedEvent(zombieScript);

            if (zombieScript.IsStunned())
            {
                if (!_stunnedZombies.Contains(collision.gameObject))
                {
                    _stunnedZombies.Add(collision.gameObject);
                }
            }
            else
            {
                if (!_nearZombies.Contains(collision.gameObject))
                {
                    _nearZombies.Add(collision.gameObject);
                }
            }
        }
    }

    // 좀비가 트리거에서 나가면 리스트에서 제거
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            _nearZombies.Remove(collision.gameObject);
            _stunnedZombies.Remove(collision.gameObject);
        }
    }

    public void SubscribeToStunnedEvent(Zombie zombie)
    {
        if (zombie != null && !_subscribedZombies.Contains(zombie))
        {
            zombie.OnStunned += HandleZombieStunned;
            _subscribedZombies.Add(zombie);
        }
    }

    private void HandleZombieStunned(GameObject zombie, bool isStunned)
    {
        if (isStunned)
        {
            if (_nearZombies.Contains(zombie))
            {
                _nearZombies.Remove(zombie);
                if (!_stunnedZombies.Contains(zombie))
                {
                    _stunnedZombies.Add(zombie);
                }
            }
        }
        else
        {
            if (_stunnedZombies.Contains(zombie))
            {
                _stunnedZombies.Remove(zombie);
                if (_nearZombies.Contains(zombie))
                {
                    _nearZombies.Add(zombie);
                }
            }
        }
    }
}
