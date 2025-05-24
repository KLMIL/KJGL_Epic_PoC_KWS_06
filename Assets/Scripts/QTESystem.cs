using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QTESystem : MonoBehaviour
{
    public static QTESystem Instance { get; private set; }

    [Header("QTE UI")]
    public GameObject QTEUI;
    public Text QTEText;

    [Header("QTE Status")]
    public float QTETime = 2f;
    public string _requiredKey = "q";

    // 내부 QTE 진행상태용 변수
    bool _isQTEActive = false;
    //float _qteTimer; // 당장엔 QTE 삭제
    GameObject _targetZombie;

    // Getter 함수
    public bool IsQTEActive() => _isQTEActive;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        QTEUI.SetActive(false);
    }

    private void Update()
    {
        if (!_isQTEActive) return;

        //_qteTimer -= Time.deltaTime;
        //if (_qteTimer <= 0)
        //{
        //    EndQTE(false);
        //}

        if (Input.anyKeyDown)
        {
            // 추후 여러 키 받을때를 대비해서, lower case로 변환하는 작업 유지
            string inputKey = (KeyCode.Q).ToString().ToLower();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // 이부분에서 정확한 키 받았는지 검사. 정확한 키라면 성공으로 QTE 종료
                EndQTE(true);
            }
            else
            {
                // 잘못된 키라면 실패로 종료
                EndQTE(false);
            }
        }
    }


    public void StartQTE(GameObject targetZombie)
    {
        // 불릿타임 시작
        Time.timeScale = 0.2f;

        _targetZombie = targetZombie;
        _isQTEActive = true;
        //_qteTimer = QTETime;
        UpdateQTEText();
        QTEUI.SetActive(true);
    }
    private void UpdateQTEText()
    {
        QTEText.text = "Press " + _requiredKey;
    }

    private void EndQTE(bool success)
    {
        _isQTEActive = false;
        QTEUI.SetActive(false);

        if (success && _targetZombie != null)
        {
            Zombie zombieScript = _targetZombie.GetComponent<Zombie>();
            zombieScript.Die();
            _targetZombie = null;
        }

        // 불릿타임 종료
        Time.timeScale = 1f;
    }
}
