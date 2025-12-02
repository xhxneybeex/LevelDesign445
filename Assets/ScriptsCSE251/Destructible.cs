using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject crateDestroyed = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyCrate()
    {
        // destructible
        Instantiate(crateDestroyed, transform.position, Quaternion.identity);
        
        // original crate
        Destroy(gameObject);
    }
}
