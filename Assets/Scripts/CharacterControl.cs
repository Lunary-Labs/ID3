using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    //Movement variables
    private float speed = 0.5f;
    private float rotSpeed = 200f;
    private float gravity = -9.81f; // Gravity value
    private float acceleration = 2f; // Acceleration value
    private float desceleration = 2f; // Desceleration value
    private float playerVelocity = 0;
    private float velocityValue = 1;
    private Vector3 velocity; // Velocity vector

    //Game Components
    private UnityEngine.CharacterController controller;
    [SerializeField]private Animator anim;

    //Game transform
    [SerializeField]private Transform cameraTransform;

    //Input system
    private PlayerInput playerInput;
    private InputAction moveAction;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        // Initialize move action
        moveAction = playerInput.actions["Move"];
        playerInput.actions["Sprint"].started += context => OnSprint();
        playerInput.actions["Sprint"].canceled += context => OnSprintCanceled();
        moveAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //Get the input value from unity input system
        Vector2 position = moveAction.ReadValue<Vector2>();

        // Calculate the move direction relative to the camera's rotation
        Vector3 camF = Camera.main.transform.forward;
        Vector3 camR = Camera.main.transform.right;
        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
        Vector3 move = (camF * position.y + camR * position.x) * speed * Time.deltaTime;

        //Character movement handle with CharacterController
        velocity.y += gravity * Time.deltaTime;

        Vector3 currentPos = transform.position;
        // Move the controller with gravity
        controller.Move(move + velocity * Time.deltaTime);
        
        Vector3 moveDirection = transform.position - currentPos;
        
        // If there's some input
        if (move != Vector3.zero) {
            if(playerVelocity < velocityValue){playerVelocity += Time.deltaTime * acceleration;}
            // Rotate the character to face the direction of movement
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotSpeed * Time.deltaTime);
            //fix y rotation
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }
        else {
            if(playerVelocity > 0){playerVelocity -= Time.deltaTime * desceleration;}
        }
        anim.SetFloat("Acceleration", playerVelocity);
    }

    void OnSprint() {
        velocityValue = 2;
    }

    void OnSprintCanceled() {
        velocityValue = 1;
    }
}
