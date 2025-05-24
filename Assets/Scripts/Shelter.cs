using UnityEngine;

public class Shelter : MonoBehaviour
{
    public GameObject Door;
    //public QTESystem qteSystem;
    bool _isPlayerInTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInTrigger = true;
            //qteSystem.StartQTE("q");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
        }
    }

    public void OnQTESuccess()
    {
        if (_isPlayerInTrigger)
        {
            Door.SetActive(false);
            FindFirstObjectByType<Gun>().RefillAmmo();
        }
    }
}
