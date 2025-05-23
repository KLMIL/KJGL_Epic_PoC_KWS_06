using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public int CurrentAmmo = 12;
    public int MaxAmmo = 60;
    public GameObject BulletPrefab;
    public Transform FirePoint;
    public Text AmmoText;
    
    Camera _mainCamera;




    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && CurrentAmmo > 0)
        {
            Shoot();
            UpdateAmmoTextUI();
        }

        if (Input.GetKeyDown("r"))
        {
            UpdateAmmoTextUI();
            Reload();
        }
    }


    public void Shoot()
    {
        CurrentAmmo--;
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        Vector3 direction = (mousePos - FirePoint.position).normalized;

        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }

    public void Reload()
    {
        // 로직 잘못됐는데?
        //CurrentAmmo = Mathf.Min(CurrentAmmo + (MaxAmmo - CurrentAmmo), 12);
        if (MaxAmmo >= 12)
        {
            CurrentAmmo = 12;
            MaxAmmo -= 12;
        }
    }


    public void RefillAmmo()
    {
        CurrentAmmo = 12;
        MaxAmmo = 60;
    }

    private void UpdateAmmoTextUI()
    {
        AmmoText.text = $"Ammo: {CurrentAmmo} / {MaxAmmo}";
    }
}
