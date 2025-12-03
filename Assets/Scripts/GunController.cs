using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private int currentAmmo = 0;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 2f;
    private bool reloading = false;
    private bool isEquipped = false; // <-- NEW FLAG

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
            crosshair.SetActive(false); // hide until equipped
    }

    void Update()
    {
        if (!isEquipped) return; // <-- only run if equipped

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

            if (shootSound != null && !shootSound.isPlaying)
                shootSound.Play();

            currentAmmo--;
            Debug.Log($"Ammo: {currentAmmo}/{maxAmmo}");

            Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hitInfo;

            if (Physics.Raycast(rayOrigin, out hitInfo))
            {
                Debug.Log("Hit: " + hitInfo.collider.name);

                if (hitMarker != null)
                {
                    GameObject hit = Instantiate(hitMarker, hitInfo.point,
                        Quaternion.LookRotation(hitInfo.normal));
                    Destroy(hit, 1.0f);
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
        if (!isEquipped) return; // only show UI when equipped
        GUI.Label(new Rect(10, 10, 200, 30), $"Ammo: {currentAmmo}/{maxAmmo}");
        if (reloading)
            GUI.Label(new Rect(10, 40, 200, 30), "Reloading...");
    }

    // Call this when player picks up the gun
    public void Equip()
    {
        isEquipped = true;
        if (crosshair != null)
            crosshair.SetActive(true);
    }

    // Call this when player drops/unequips the gun
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
