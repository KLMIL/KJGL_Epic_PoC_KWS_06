using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QTESystem : MonoBehaviour
{
    public static QTESystem Instance { get; private set; }

    [Header("QTE UI")]
    public GameObject QteUi;
    public Text QteText;

    public GameObject QteNotifierUi;
    public Text QteNotifierText;

    [Header("QTE Status")]
    public float QteTime = 2f;
    public string RequiredKey = "q";
    public string RequiredKeys = "";
    int _keyIndex = 0;

    char[] _keyPool = { 'Q', 'W', 'E', 'A', 'S', 'D' };

    // 내부 QTE 진행상태용 변수
    bool _isQteActive = false;
    //float _qteTimer; // 당장엔 QTE 삭제
    GameObject _targetZombie;
    List<GameObject> _targetZombies = new List<GameObject>();

    // Getter 함수
    public bool IsQteActive() => _isQteActive;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        QteUi.SetActive(false); // 비활성화 상태로 시작
    }

    private void Update()
    {
        if (!_isQteActive) return;

        //_qteTimer -= Time.deltaTime;
        //if (_qteTimer <= 0)
        //{
        //    EndQTE(false);
        //}

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.F)) return;

            // 이번에 입력받아야 하는 키
            KeyCode expectedKey = (KeyCode)RequiredKeys[_keyIndex];

            if (Input.GetKeyDown(expectedKey))
            {
                // 정확한 키라면 index++ 후 다음 입력 대기
                _keyIndex++;

                // index가 요구 키 갯수와 같다면 성공으로 종료 -> EndQte(true)
                if (_keyIndex >= RequiredKeys.Length)
                {
                    EndQte(true);
                }
                return;

            }
            // 잘못된 키를 입력했다면 실패로 동료 -> EndQte(false)
            EndQte(false);
        }
    }

    private void DEP_UpdateQteText()
    {
        QteText.text = "Press " + RequiredKey.ToUpper();
    }

    private void UpdateQteText(int len)
    {
        RequiredKeys = "";

        for (int i = 0; i < len; i++)
        {
            char randomKey = _keyPool[Random.Range(0, _keyPool.Length)];
            RequiredKeys += randomKey;
        }

        QteText.text = "Press ";
        for (int i = 0; i < len; i++)
        {
            QteText.text += $" {RequiredKeys[i]}";
        }

        Debug.Log($"{RequiredKeys}");
    }

    public void DEP_StartQte(GameObject targetZombie)
    {
        // 불릿타임 시작
        Time.timeScale = 0.2f;

        _targetZombie = targetZombie;
        _isQteActive = true;
        //_qteTimer = QTETime;
        DEP_UpdateQteText();
        QteUi.SetActive(true);
    }

    public void StartQte(List<GameObject> targetZombies)
    {
        // 불릿타임 시작
        Time.timeScale = 0.2f;

        _targetZombies = targetZombies;
        _isQteActive = true;
        UpdateQteText(_targetZombies.Count);
        QteUi.SetActive(true);
    }

    private void EndQte(bool success)
    {
        _isQteActive = false;
        QteUi.SetActive(false);

        if (success && _targetZombie != null)
        {
            Zombie zombieScript = _targetZombie.GetComponent<Zombie>();
            zombieScript.Die();
            _targetZombie = null;
        }

        // 불릿타임 종료
        Time.timeScale = 1f;
    }


    public void ShowQteNotifier()
    {
        QteNotifierText.text = "[F] to start QTE";
        QteNotifierUi.gameObject.SetActive(true);
    }

    public void HideQteNotifier()
    {
        QteNotifierText.text = "";
        QteNotifierUi.gameObject.SetActive(false);
    }
}
