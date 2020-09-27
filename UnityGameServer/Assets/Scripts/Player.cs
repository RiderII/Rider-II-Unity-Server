using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public int points = 0;
    public CharacterController controller;
    public Rigidbody player;
    public float gravity = -9.81f;
    public int collisions = 0;
    public float traveled_kilometers = 0;
    public float burned_calories = 0;
    public List<string> steps = new List<string>();
    private int previouSteps = 0;

    private bool[] inputs;
    private float yVelocity = 0;
    public float acceleration = 0.6f;
    public float maximunSpeed = 9f;
    public float obstacleSlowDown = 0.25f;
    public float speed = 0f;
    public bool surpassSpeed = false;
    private string currentSceneName;
    public bool reachedFinishLine = false;

    private float distanceTimer;
    private float nextUpdate = 1f;
    private bool sent_from_middleware = false;

    public void Initialize(int _id, string _username, float positionx, Player player)
    {
        id = _id;
        username = _username;
        currentSceneName = NetworkManager.instance.sceneName;

        if (currentSceneName == "Vaquita")
        {
            transform.position += transform.right * (player.transform.position.x + positionx - 1.0f) + transform.forward * player.transform.position.y + transform.up * 1.0f;
        }
        else
        {
            transform.position = new Vector3(positionx, player.transform.position.y, player.transform.position.z);
        }
        

        inputs = new bool[4];
        Time.timeScale = 1;
    }

    public void FixedUpdate() //in the server console application this wass called every tick, now it's called every frame
    {
        distanceTimer += Time.deltaTime;

        if (Server.clients[id].hasMiddleware && Time.time >= nextUpdate)
        {
            nextUpdate = Mathf.FloorToInt(Time.time) + 0.7f;
            if (acceleration > 0)
            {
                acceleration -= 0.3f;
            }
            if (acceleration <= 0)
            {
                acceleration = 0;
            }
        }

        if ((currentSceneName == "Vaquita" && controller && controller.enabled) ||
            (currentSceneName != "Vaquita" && player))
        {
            if (currentSceneName == "Vaquita")
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
            }

            Vector3 direction = new Vector3();

            if (currentSceneName == "Vaquita")
            {
                direction = new Vector3(controller.transform.forward.x, 0, controller.transform.forward.z);
            }
            else
            {
                if (distanceTimer >= 5f)
                {
                    int stepsPassed = steps.Count - previouSteps;
                    CalculatePoints(stepsPassed);
                    PacketSend.UpdatePlayerPoints(this);
                    previouSteps = steps.Count;
                    distanceTimer = 0f;
                }
            }

            if (Server.clients[id].hasMiddleware)
            {
                speed = acceleration * Time.fixedDeltaTime;
            }

            else
            {
                speed += acceleration * Time.fixedDeltaTime;
            }

            if (speed > maximunSpeed && !surpassSpeed)
            {
                StartCoroutine(SlowDown());
            }

            if (speed < 0)
            {
                speed = 0;
            }

            if (!Server.clients[id].hasMiddleware && inputs[0] && !reachedFinishLine)
            {
                if (acceleration < 0)
                {
                    acceleration *= -1;
                }
            }

            if (currentSceneName == "Vaquita")
            {
                controller.Move(direction.normalized * speed * Time.fixedDeltaTime);
            }
            else
            {
                player.transform.Translate(speed * Time.deltaTime, 0f, 0f);
            }

            if (!Server.clients[id].hasMiddleware && !inputs[0] && !reachedFinishLine)
            {
                if (acceleration > 0)
                {
                    acceleration *= -1;
                }
            }

            if (!Server.clients[id].hasMiddleware && reachedFinishLine && acceleration > 0)
            {
                if (speed > 7)
                {
                    acceleration *= -5;
                }
                else if (speed > 5)
                {
                    acceleration *= -3;
                }
                else
                {
                    acceleration *= -1;
                }
            }

            //stay in the route

            if (currentSceneName == "Vaquita")
            {
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
            }

            if (sent_from_middleware)
            {
                PacketSend.PlayerPosition(this);
                PacketSend.PlayerRotation(this); //client is authorative in rotation
            }
        }
    }

    IEnumerator SlowDown()
    {
        if (speed > maximunSpeed && !surpassSpeed)
        {
            speed -= 0.1f;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SetInput(int _fromClient, bool[] _inputs, Quaternion? _rotation = null, bool? middleware = false)
    {
        if (middleware != null) sent_from_middleware = (bool)middleware;
        if (Server.clients[_fromClient].hasMiddleware == false)
        {
            if (Server.clients[_fromClient].username == "Middleware")
            {
                if (!reachedFinishLine)
                {
                    acceleration += 0.6f;
                }
            }
            inputs = _inputs;
        }
        if (_rotation != null) transform.rotation = (Quaternion)_rotation;
    }

    private void CalculatePoints(int stepsPassed)
    {
        switch (stepsPassed)
        {
            case var _ when stepsPassed >= 12: points += 250; break;
            case var _ when stepsPassed >= 10: points += 200; break;
            case var _ when stepsPassed >= 8: points += 150; break;
            case var _ when stepsPassed >= 6: points += 100; break;
        }
    }
}
