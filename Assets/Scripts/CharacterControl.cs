using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    //Statistical variables
    private float speed = 5.0f;
    private UnityEngine.CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMove(InputValue pos) {
        //Get the input value from unity input system
        Vector2 position = pos.Get<Vector2>();
        //Character movement handle with CharacterController
        Vector3 move = new Vector3(position.x, 0, position.y) * speed * Time.deltaTime;
        controller.Move(move);
        Quaternion toRotation = Quaternion.LookRotation(move, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, speed * Time.deltaTime);
    }
}
