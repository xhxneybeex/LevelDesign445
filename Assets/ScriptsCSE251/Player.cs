using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject muzzleFlash = null;
    [SerializeField] private GameObject hitMarker = null;
    [SerializeField] private AudioSource sound = null;
    [SerializeField] private GameObject weapon = null;

    private CharacterController controller = null;
    private float speed = 3.5f;
    private float gravity = 9.81f;
    [SerializeField] private int currentAmmo = 0;
    private int maxAmmo = 50;
    private bool reloading = false;
    public bool hasCoin = false;

    private UIManager UI = null;

    // Start is called before the first frame update
    void Start()
    {
        UI = GameObject.Find("UIManager").GetComponent<UIManager>();

        Cursor.visible = false;   // hide the cursor
        Cursor.lockState = CursorLockMode.Locked;  // lock to center

        controller = GetComponent<CharacterController>();

        currentAmmo = maxAmmo;  // refill ammo
        UI.UpdateAmmo(currentAmmo);
    }

    // Update is called once per frame
    void Update()  // lets physics engine act prior
    {
        Move();
        Shoot();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;   // shows the cursor
            Cursor.lockState = CursorLockMode.None;  // unlock
        }

        if(!reloading && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            currentAmmo = maxAmmo;
            UI.UpdateAmmo(currentAmmo);
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(3.0f);
        reloading = false;
    }

    private void Shoot()
    {
        if(weapon.activeSelf && currentAmmo > 0 && !reloading && Input.GetMouseButton(0))
        {
            muzzleFlash.SetActive(true);  // show flash

            if(!sound.isPlaying)
                sound.Play();

            currentAmmo--;  // decrement ammo
            UI.UpdateAmmo(currentAmmo);

            Ray rayOrigin = 
                Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
            // find middle of game window

            RaycastHit hitInfo;

            if(Physics.Raycast(rayOrigin, out hitInfo))
            {
                // if line from Player to center
                // is crossed, return true

                //Debug.Log("We hit something " + hitInfo.point);

                GameObject hit = Instantiate(hitMarker, hitInfo.point, 
                    Quaternion.LookRotation(hitInfo.normal));

                Destroy(hit, 1.0f);

                Destructible crate 
                    = hitInfo.transform.GetComponent<Destructible>();

                if(crate != null)
                {
                    crate.DestroyCrate();
                }

            }
        }
        else
        {
            muzzleFlash.SetActive(false);  // flash off
            sound.Stop();
        }
    }


    private void Move()
    {
        Vector3 direction = 
            new Vector3(Input.GetAxis("Horizontal"), 
            0, 
            Input.GetAxis("Vertical"));

        Vector3 velocity = direction * speed;

        velocity.y -= gravity;


        // convert from world space (forward is world forward)
        // to local space (forward as player faces)
        velocity = transform.TransformDirection(velocity);
        // first transform = player
        // second transform = world

        controller.Move(velocity * Time.deltaTime);
    }

    public void EnableWeapon()
    {
        weapon.SetActive(true);
    }
}
