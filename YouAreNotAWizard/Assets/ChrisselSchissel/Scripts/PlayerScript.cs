using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Camera cam;
    public GameObject hitBox;
    public DetectionScript realOne;
    private DetectionScript clone;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HitBox();
    }
    void HitBox()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 direction = Vector3.up.normalized;
        Quaternion directionAngle = Quaternion.LookRotation(direction);
        if (Physics.Raycast(ray, out hit))
        {
            direction = (hit.point - transform.position).normalized;
            directionAngle = Quaternion.LookRotation(direction);
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Yes");   
            clone = Instantiate(realOne, transform.GetChild(0).position + direction*2, directionAngle);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //direction = (hit.point - transform.position).normalized;
            direction = new Vector3(direction.x, 0.0f, direction.z);
            clone.transform.position = transform.GetChild(0).position + direction * clone.transform.localScale.z;
            directionAngle = Quaternion.LookRotation(direction);
            clone.transform.rotation = directionAngle;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            clone.DestroyHitBox();
        }
    }
}