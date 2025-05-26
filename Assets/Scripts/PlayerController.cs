using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Gun Gun;
    [SerializeField] QTESystem _qteSystem;

    [Header("Player Ststus")]
    [SerializeField] float _moveSpeed = 5f;

    // QTE Field
    bool _isQTEButtonNotifierShow = false;

    // Enemy Field
    [SerializeField] List<GameObject> _nearZombies = new List<GameObject>(); // QTE 사거리 내 좀비
    [SerializeField] List<GameObject> _stunnedZombies = new List<GameObject>(); // 무력화 좀비
    [SerializeField] HashSet<Zombie> _subscribedZombies = new HashSet<Zombie>(); // 좀비 상태 이벤트 구독


    private void OnDestroy()
    {
        // 이벤트 구독 해제
        foreach (var zombie in _subscribedZombies)
        {
            if (zombie != null) zombie.OnStunned -= HandleZombieStunned;
        }
    }

    private void Update()
    {
        // 플레이어 이동
        Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        Vector2 moveDirection = input.normalized;
        transform.position += (Vector3)(moveDirection * _moveSpeed * Time.deltaTime);


        // 마우스 방향으로 회전
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePos.z = 0;
        Vector3 lookDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);


        // QTE 트리거 : 근처 stun 좀비가 있고, QTE 비활성화 상태
        if (_stunnedZombies.Count > 0 && !_qteSystem.IsQteActive())
        {
            // QTE가 가능한 상태면 "F"키 노출
            if (!_isQTEButtonNotifierShow)
            {
                _isQTEButtonNotifierShow = true;
                _qteSystem.ShowQteNotifier();
            }

            // F키 눌리면 StartQTE 호출
            if (Input.GetKeyDown(KeyCode.F))
            {
                // F키 노출 제거
                _isQTEButtonNotifierShow = false;
                _qteSystem.HideQteNotifier();

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
                _qteSystem.StartQte(closestZombie);
            }
        }
        else
        {
            // F키 노출 제거
            _isQTEButtonNotifierShow = false;
            _qteSystem.HideQteNotifier();
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
            zombie.OnDestroyed += UnsubscribeFromZombie;
            _subscribedZombies.Add(zombie);
        }
    }

    private void UnsubscribeFromZombie(Zombie zombie)
    {
        if (zombie != null)
        {
            zombie.OnStunned -= HandleZombieStunned;
            zombie.OnDestroyed -= UnsubscribeFromZombie;
            _subscribedZombies.Remove(zombie);
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
