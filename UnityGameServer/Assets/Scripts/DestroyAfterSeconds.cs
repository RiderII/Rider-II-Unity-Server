using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float seconds = 10f;

    // Update is called once per frame
    void FixedUpdate()
    {
        seconds -= Time.deltaTime;
        if (seconds <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
