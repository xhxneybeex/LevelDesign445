using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound = null;

    UIManager UI = null;

    // Start is called before the first frame update
    void Start()
    {
        UI = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Press E to collect the coin.");

            if(Input.GetKeyDown(KeyCode.E))
            {
                Player p = other.GetComponent<Player>();

                if(p != null)
                {
                    p.hasCoin = true;   // collected

                    if(UI != null)
                        UI.CollectCoin();

                    AudioSource.PlayClipAtPoint(pickupSound, 
                        Camera.main.transform.position, 1f);  // sound

                    Destroy(gameObject);  // coin
                }
            }
        }
    }


}
