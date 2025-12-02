using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // for Text

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text ammoText;
    [SerializeField] private GameObject coinImage = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAmmo(int count)  // formal parameter
    {
        ammoText.text = "Ammo: " + count;  // concatenation
    }

    public void CollectCoin()
    {
        coinImage.SetActive(true);
    }

    public void RemoveCoin()
    {
        coinImage.SetActive(false);
    }
}
