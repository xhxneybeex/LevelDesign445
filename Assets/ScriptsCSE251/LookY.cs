using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookY : MonoBehaviour
{
    private float sensitivity = 0.75f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y");
        // -1 to + 1, left to right mouse

        //transform.localEulerAngles = new Vector3(
        //    transform.localEulerAngles.x + (mouseY * sensitivity),
        //    transform.localEulerAngles.y,  // L/R rotation
        //    transform.localEulerAngles.z);

        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x -= (mouseY * sensitivity);  // - to invert
        transform.localEulerAngles = newRotation;

    }
}
