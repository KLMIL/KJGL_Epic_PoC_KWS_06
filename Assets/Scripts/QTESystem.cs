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

    // 내부 QTE 진행상태용 변수
    bool _isQteActive = false;
    //float _qteTimer; // 당장엔 QTE 삭제
    GameObject _targetZombie;

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

            // 추후 여러 키 받을때를 대비해서, lower case로 변환하는 작업 유지
            string inputKey = (KeyCode.Q).ToString().ToLower();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // 이부분에서 정확한 키 받았는지 검사. 정확한 키라면 성공으로 QTE 종료
                EndQte(true);
            }
            else
            {
                // 잘못된 키라면 실패로 종료
                EndQte(false);
            }
        }
    }

    private void UpdateQteText()
    {
        QteText.text = "Press " + RequiredKey.ToUpper();
    }

    public void StartQte(GameObject targetZombie)
    {
        // 불릿타임 시작
        Time.timeScale = 0.2f;

        _targetZombie = targetZombie;
        _isQteActive = true;
        //_qteTimer = QTETime;
        UpdateQteText();
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
