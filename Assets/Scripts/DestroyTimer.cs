using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float waitTime;


    void Update()
    {
        if (waitTime <= 0)
        {
            if (transform.Find("Destroy Particles"))
            {
                transform.Find("Destroy Particles").gameObject.SetActive(true);
                transform.Find("Destroy Particles").transform.parent = transform.parent;
            }
            Destroy(gameObject);
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }
}
