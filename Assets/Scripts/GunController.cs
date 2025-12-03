using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private int currentAmmo = 0;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private LayerMask hitLayers;
    private bool reloading = false;
    private bool isEquipped = false;
    private float nextFireTime = 0f;

    [Header("Effects")]
    [SerializeField] private GameObject muzzleFlash = null;
    [SerializeField] private GameObject hitMarker = null;

    [Header("Audio")]
    [SerializeField] private AudioSource shootSound = null;

    [Header("UI")]
    [SerializeField] private GameObject crosshair = null;

    void Start()
    {
        currentAmmo = maxAmmo;
        if (crosshair != null)
            crosshair.SetActive(isEquipped);
        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);
    }


    void Update()
    {
        if (!isEquipped) return;

        Shoot();

        if (!reloading && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        if (currentAmmo > 0 && !reloading && Input.GetMouseButton(0))
        {
            if (muzzleFlash != null)
                muzzleFlash.SetActive(true);

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;

                if (shootSound != null && !shootSound.isPlaying)
                    shootSound.Play();

                currentAmmo--;
                Debug.Log($"Ammo: {currentAmmo}/{maxAmmo}");

                Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hitInfo;

                if (Physics.Raycast(rayOrigin, out hitInfo, 100f, hitLayers))
                {
                    Debug.DrawLine(rayOrigin.origin, hitInfo.point, Color.red, 1f);
                    Debug.Log("Hit: " + hitInfo.collider.name + " at " + hitInfo.point);

                    if (hitMarker != null)
                    {
                        GameObject hit = Instantiate(hitMarker, hitInfo.point,
                            Quaternion.LookRotation(hitInfo.normal));
                        Destroy(hit, 1.0f);
                    }

                    // Check if the hit object has a Destructible component
                    Destructible destructible = hitInfo.collider.GetComponent<Destructible>();
                    if (destructible != null)
                    {
                        destructible.DestroyCrate();
                    }
                }

            }
        }
        else
        {
            if (muzzleFlash != null)
                muzzleFlash.SetActive(false);

            if (shootSound != null)
                shootSound.Stop();
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        reloading = false;
        Debug.Log("Reload complete!");
    }

    void OnGUI()
    {
        if (!isEquipped) return;
        GUI.Label(new Rect(10, 10, 200, 30), $"Ammo: {currentAmmo}/{maxAmmo}");
        if (reloading)
            GUI.Label(new Rect(10, 40, 200, 30), "Reloading...");
    }

    public void Equip()
    {
        isEquipped = true;
        if (crosshair != null)
            crosshair.SetActive(true);
    }

    public void Unequip()
    {
        isEquipped = false;
        if (crosshair != null)
            crosshair.SetActive(false);
        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);
        if (shootSound != null)
            shootSound.Stop();
    }
}
