using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
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
            Debug.Log("Press E to purchase a weapon.");

            if(Input.GetKeyDown(KeyCode.E))
            {
                Player p = other.GetComponent<Player>();

                if(p != null)
                {
                    if(p.hasCoin)
                    {
                        p.hasCoin = false;

                        if(UI != null)
                            UI.RemoveCoin();

                        AudioSource audio = GetComponent<AudioSource>();
                        if (audio != null)
                            audio.Play();

                        p.EnableWeapon();
                    }
                }
            }
        }
    }
}
