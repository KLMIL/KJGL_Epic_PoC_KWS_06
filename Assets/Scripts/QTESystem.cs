using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTESystem : MonoBehaviour
{
    public GameObject _player;

    public GameObject QTEUI;
    public Text QTEText;
    public float QTETime = 2f;
    public float SpaceQTETime = 0.2f;

    bool _isQTEActive = false;
    float _qteTimer;
    List<string> _requiredKeys = new List<string>();
    int _currentKeyIndex = 0;
    bool _isSpacebarQTE = false;
    float _spaceQTETimer;

    string[] _qteSequences = { "Q", "QE", "QWE", "QWEASD", "QWEASDZXC", "QWERASDFZXCV" };
    int _currentStage = 0;
    

    public bool IsQTEActive()
    {
        return _isQTEActive;
    }

    public int GetCurrentStage()
    {
        return _currentStage;
    }

    public void StartQTEForStage(int stage)
    {
        _currentStage = Mathf.Min(stage, _qteSequences.Length - 1);
        string keypool = _qteSequences[_currentStage];
        char randomKey = keypool[Random.Range(0, keypool.Length)];
        StartQTE(randomKey.ToString());
    }

    public void StartQTE(string keys)
    {
        _requiredKeys.Clear();
        foreach (char key in keys)
        {
            _requiredKeys.Add(keys.ToString().ToLower());
        }

        _currentKeyIndex = 0;
        _isQTEActive = true;
        _qteTimer = QTETime;
        QTEUI.SetActive(true);
        UpdateQTEText();
    }

    private void Update()
    {
        if (_isQTEActive)
        {
            _qteTimer -= Time.deltaTime;
            if (_qteTimer <= 0)
            {
                EndQTE(false);
            }

            if (_isSpacebarQTE)
            {
                float elapsed = Time.time - _spaceQTETimer;
                if (elapsed > SpaceQTETime)
                {
                    EndQTE(false);
                }
                if (Input.GetKeyDown("space"))
                {
                    EndQTE(true);
                }
            }
            else
            {
                if (Input.anyKeyDown)
                {
                    foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKeyDown(keyCode))
                        {
                            string inputKey = keyCode.ToString().ToLower();
                            if (inputKey == _requiredKeys[_currentKeyIndex])
                            {
                                _currentKeyIndex++;
                                if (_currentKeyIndex >= _requiredKeys.Count)
                                {
                                    StartSpacebarQTE();
                                }
                                else
                                {
                                    UpdateQTEText();
                                }
                            }
                            else
                            {
                                EndQTE(false);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    private void StartSpacebarQTE()
    {
        _isSpacebarQTE = true;
        _qteTimer = SpaceQTETime;
        _spaceQTETimer = Time.time;
        QTEText.text = "Press Space";
    }

    private void UpdateQTEText()
    {
        QTEText.text = "Press " + _requiredKeys[_currentKeyIndex];
    }

    private void EndQTE(bool success)
    {
        _isQTEActive = false;
        _isSpacebarQTE = false;
        QTEUI.SetActive(false);
        if (success)
        {
            KnockbackZombies();
            //Shelter shelter = FindFirstObjectByType<Shelter>();
            //shelter.OnQTESuccess();
            _currentStage++;
        }
    }

    private void KnockbackZombies()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject zombie in zombies)
        {
            Vector3 direction = (zombie.transform.position - _player.transform.position).normalized;
            zombie.transform.position += direction * 5f;
        }
    }
}
