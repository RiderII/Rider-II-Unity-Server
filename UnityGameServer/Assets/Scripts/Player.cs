using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public float gravity = -9.81f;
    public int collisions = 0;
    public float traveled_kilometers = 0;
    public float burned_calories = 0;

    private bool[] inputs;
    private float yVelocity = 0;
    public float acceleration = 0.6f;
    public float maximunSpeed = 9f;
    public float obstacleSlowDown = 0.25f;
    public float speed = 0f;

    public void Initialize(int _id, string _username, float positionx = 0)
    {
        id = _id;
        username = _username;
        transform.position += transform.right * (Vector2.zero.x + positionx - 1.0f) + transform.forward * Vector2.zero.y + transform.up * 1.0f;

        inputs = new bool[4];
    }

    public void FixedUpdate() //in the server console application this wass called every tick, now it's called every frame
    {
        if (controller && controller.enabled)
        {
            if (controller.isGrounded)
            {
                yVelocity = 0f;
            }
            else
            {
                Vector3 moveDown = new Vector3(controller.transform.forward.x, controller.transform.forward.y + gravity, controller.transform.forward.z);
                controller.Move(moveDown.normalized * speed * Time.fixedDeltaTime);
            }
            yVelocity += gravity;

            Vector3 direction = new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z);

            speed += acceleration * Time.fixedDeltaTime;

            if (speed > maximunSpeed)
            {
                speed = maximunSpeed;
            }

            if (speed < 0)
            {
                speed = 0;
            }

            if (acceleration < 0)
            {
                controller.Move(direction.normalized * speed * Time.fixedDeltaTime);
            }


            if (inputs[0])
            {
                if (acceleration < 0)
                {
                    acceleration *= -1;
                }
                controller.Move(direction.normalized * speed * Time.fixedDeltaTime);
            }
            else
            {
                if (acceleration > 0)
                {
                    acceleration *= -1;
                }
            }

            //stay in the route

            if (controller.transform.position.x < -4.0f)
            {
                controller.enabled = false;
                controller.transform.position = new Vector3(-4.0f, controller.transform.position.y, controller.transform.position.z);
                controller.enabled = true;
            }
            else if (controller.transform.position.x > 4.0f)
            {
                controller.enabled = false;
                controller.transform.position = new Vector3(4.0f, controller.transform.position.y, controller.transform.position.z);
                controller.enabled = true;

            }

            PacketSend.PlayerPosition(this);
            PacketSend.PlayerRotation(this); //client is authorative in rotation
        }
    }

    public void SetInput(bool[] _inputs, Quaternion? _rotation = null)
    {
        inputs = _inputs;
        if (_rotation != null) transform.rotation = (Quaternion)_rotation;
    }
}
