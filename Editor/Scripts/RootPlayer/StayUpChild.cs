using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayUpChild : MonoBehaviour
{

    private float lastParentRotation=0;

   
    // Update is called once per frame
    void Update()
    {
        if(lastParentRotation != transform.parent.eulerAngles.x)
        {
            lastParentRotation = transform.parent.eulerAngles.x;

            foreach (Transform child in transform)
            {
                //child.eulerAngles = new Vector3(-lastParentRotation, child.eulerAngles.y, child.eulerAngles.z);
                child.LookAt(Vector3.up);
            }
        }
    }
}
