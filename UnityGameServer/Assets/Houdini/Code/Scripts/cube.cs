using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cube : MonoBehaviour
{
    float speed = 10f;
    float turnSpeed = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("w"))
        {
            transform.Translate(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey("a"))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }
        if (Input.GetKey("d"))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(-speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Circuito cerrado");
        }
    }
}
