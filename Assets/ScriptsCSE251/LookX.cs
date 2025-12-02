using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookX : MonoBehaviour
{
    private float sensitivity = 0.75f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        // -1 to + 1, left to right mouse

        //transform.localEulerAngles = new Vector3(
        //    transform.localEulerAngles.x,
        //    transform.localEulerAngles.y + (mouseX * sensitivity),  // L/R rotation
        //    transform.localEulerAngles.z);

        Vector3 newRotation = transform.localEulerAngles;
        newRotation.y += (mouseX * sensitivity);
        transform.localEulerAngles = newRotation;

    }
}
