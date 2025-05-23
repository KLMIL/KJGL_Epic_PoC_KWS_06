using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Zombie").Length == 0)
        {
            Debug.Log("Stage Clear");
            Time.timeScale = 0f;
        }
    }
}
