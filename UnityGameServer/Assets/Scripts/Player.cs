using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public float gravity = -9.81f;
    public float jumpSpeed = 5f;
    public float moveSpeed = 5f;

    private bool[] inputs;
    private float yVelocity = 0;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username, float positionx = 0)
    {
        id = _id;
        username = _username;
        transform.position += transform.right * (Vector2.zero.x + positionx) + transform.forward * Vector2.zero.y + transform.up * 1f;
        // position = _spwanPosition;
        // rotation = Quaternion.identity;

        inputs = new bool[4];
    }

    public void FixedUpdate() //in the server console application this wass called every tick, now it's called every frame
    {
        Vector2 _inputDirection = Vector2.zero; //we move only in the Y and X axis
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirecion = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirecion *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
        }
        yVelocity += gravity;

        _moveDirecion.y = yVelocity;
        controller.Move(_moveDirecion);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this); //client is authorative in rotation
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }
}
