using UnityEngine;

public class CameraMover : MonoBehaviour
{
	public Transform targetPos;
	public float heightOffset = 2f;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
		Vector3 offset = new Vector4(targetPos.position.x, 
		targetPos.position.y + heightOffset, targetPos.position.z);
        transform.position = offset;
    }
}
