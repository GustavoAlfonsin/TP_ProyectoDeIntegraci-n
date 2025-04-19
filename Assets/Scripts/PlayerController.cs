using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //PLAYER
    private float movementSpeed = 5.0f;
    private float gravity = -9.81f;
    private float jumpHeight = 2.0f;
    private CharacterController controller;
    private Vector3 playerSpeed;

    //CAMARA
    private float mouseSensitivity = 5f;
    private float verticalRotation = 0f;
    [SerializeField] private float verticalRotationLimit = 45.0f;

    //MIRA
    private Vector2 originalScale, zoomScale;
    [SerializeField] private Image normalPoint, aimPoint;
    [SerializeField] private float zoomSpeed, zoomCapacity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        moveCharacter();
        moveCamera();
        apuntarYDesapuntar();
    }

    private void apuntarYDesapuntar()
    {
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomCapacity,
                                        Time.deltaTime * zoomSpeed);
            normalPoint.gameObject.SetActive(false);
            aimPoint.gameObject.SetActive(true);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60f, 
                                        Time.deltaTime * zoomSpeed);
            normalPoint.gameObject.SetActive(true);
            aimPoint.gameObject.SetActive(false);
        }
    }

    private void moveCamera()
    {
        // Rotación horizontal
        float horizontalMouseRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0,horizontalMouseRotation,0);

        // Rotación Vertical
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation,-verticalRotationLimit, verticalRotationLimit);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation,0,0);
    }

    private void moveCharacter()
    {
        float XMovement = Input.GetAxis("Horizontal");
        float ZMovement = Input.GetAxis("Vertical");
        Vector3 movement = transform.right * XMovement + transform.forward * ZMovement;
        controller.Move(movement * movementSpeed * Time.deltaTime);

        if (controller.isGrounded && playerSpeed.y < 0)
        {
            playerSpeed.y = 0f;
        }
        playerSpeed.y += gravity * Time.deltaTime;
        controller.Move(playerSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            playerSpeed.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
