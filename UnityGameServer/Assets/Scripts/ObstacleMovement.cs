using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    Rigidbody self;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!self.IsSleeping())
        {

            PacketSend.ObstacleMovement(int.Parse(name), transform.position, transform.rotation);
        }
    }
}
