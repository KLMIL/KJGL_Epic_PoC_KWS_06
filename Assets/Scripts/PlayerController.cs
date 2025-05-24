using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public Gun Gun;
    QTESystem _qteSystem;

    Rigidbody2D _rb;
    float _moveSpeed = 5f;
    Vector2 _targetPosition;
    bool _isMoving = false;
    float _moveThreshold = 0.1f;

    bool _isQTEButtonShow = false;

    
    [SerializeField] List<GameObject> _nearZombies = new List<GameObject>(); // QTE 사거리 내 좀비
    [SerializeField] List<GameObject> _stunnedZombies = new List<GameObject>(); // 무력화 좀비
    [SerializeField] HashSet<Zombie> _subscribedZombies = new HashSet<Zombie>(); // 구독 추적


    // Get 함수
    //public List<GameObject> GetNearZombies() => _nearZombies;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _qteSystem = QTESystem.Instance;
        _targetPosition = transform.position;
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
        // 플레이어 이동
        //Vector2 input = new Vector2(
        //        Input.GetAxisRaw("Horizontal"),
        //        Input.GetAxisRaw("Vertical")
        //    );
        //Vector2 moveDirection = input.normalized;
        //transform.position += (Vector3)(moveDirection * _moveSpeed * Time.deltaTime);

        // 마우스로 플레이어 이동
        if (Input.GetMouseButtonDown(1)) {
            _targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _isMoving = true;
        }

        if (_isMoving)
        {
            Vector2 currentPosition = transform.position;
            float distance = Vector2.Distance(currentPosition, _targetPosition);
            if (distance > _moveThreshold)
            {
                Vector2 moveDirection = (_targetPosition - currentPosition).normalized;
                transform.position += (Vector3)(moveDirection * _moveSpeed * Time.deltaTime);
            }
            else
            {
                _isMoving = false;
            }
        }


        // 마우스 방향으로 회전
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 lookDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);


        // QTE 트리거 : 근처 stun 좀비가 있고, QTE 비활성화 상태
        if (_stunnedZombies.Count > 0 && !_qteSystem.IsQTEActive())
        {
            // 임시 QTE 키 노출 함수. 추후 구조조정바람
            //if (!_isQTEButtonShow)
            //{
            //    _qteSystem.ShowQTEStartText();
            //}

            if (Input.GetKeyDown(KeyCode.E)) 
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
        else
        {
            // 임시 QTE 키 노출 제거 함수. 추후 구조조정바람
            //_qteSystem.NoShowQTEStartText();
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
