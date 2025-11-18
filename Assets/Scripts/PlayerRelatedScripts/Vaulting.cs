using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//LICENSE, YOU ARE LEGALLY REQUIRED TO LIKE THE VIDEO IF YOU COPY THIS CODE :) (i promise its legally binding, 50 years prison minimum)
public class Vaulting : MonoBehaviour
{
    public Transform vaultOrigin; // drag chest-height empty here
    public LayerMask vaultMask;   // assign VaultLayer in Inspector
    public float vaultDistance = 2f;
    public float playerHeight = 2f;
    public float playerRadius = 0.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryVault();
        }
    }

    void TryVault()
    {
        if (Physics.Raycast(vaultOrigin.position, vaultOrigin.forward, out var firstHit, vaultDistance, vaultMask))
        {
            Debug.Log("Vaultable in front");

            if (Physics.Raycast(firstHit.point + (vaultOrigin.forward * playerRadius) + (Vector3.up * 0.6f * playerHeight),
                                Vector3.down, out var secondHit, playerHeight))
            {
                Debug.Log("Found place to land");
                StartCoroutine(LerpVault(secondHit.point, 0.5f));
            }
        }
    }

    IEnumerator LerpVault(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
