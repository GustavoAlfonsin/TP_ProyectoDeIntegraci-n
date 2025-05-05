using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    //PLAYER
    private float movementSpeed = 3.5f;
    private float gravity = -9.81f;
    private float jumpHeight = 2.0f;
    private CharacterController controller;
    private Animator _animator;
    private Vector3 playerSpeed;
    private float maxHp, Hp, maxEnergy, energy;

    //MIRA
    private Vector2 originalScale, zoomScale;
    [SerializeField] private Image normalPoint, aimPoint;
    [SerializeField] private float zoomSpeed, zoomCapacity;
    [HideInInspector] public CinemachineVirtualCamera vCam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        maxHp = 100;
        Hp = maxHp;
        maxEnergy = 50;
        energy = maxEnergy;
        UIController.Instance.HpUpdate(Hp);
    }


    void Update()
    {
        moveCharacter();
        apuntarYDesapuntar();
        if (Input.GetKeyDown(KeyCode.Y))
        {
            getDamage(10);
        }
    }

    private void apuntarYDesapuntar()
    {
        if (Input.GetMouseButton(1))
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomCapacity,
                                        Time.deltaTime * zoomSpeed);
            normalPoint.gameObject.SetActive(false);
            aimPoint.gameObject.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(rayo, out hit))
                {
                    Debug.Log("El disparo ha impactado con algo");
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.collider.gameObject.GetComponent<ZombieController>().getDamage(10);
                    }
                }
            }
        }
        else
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60f, 
                                        Time.deltaTime * zoomSpeed);
            normalPoint.gameObject.SetActive(true);
            aimPoint.gameObject.SetActive(false);
        }
    }

    private void moveCharacter()
    {
        float XMovement = Input.GetAxis("Horizontal");
        float ZMovement = Input.GetAxis("Vertical");
        _animator.SetFloat("Movimiento X", XMovement);
        _animator.SetFloat("Movimiento Z", ZMovement);
        Vector3 movement = transform.right * XMovement + transform.forward * ZMovement;
        movement = movement.normalized;
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

    public void getDamage(float injury)
    {
        Hp -= injury;
        if (Hp <= 0)
        {
            Hp = 0;
            //mostrar animación de muerte
            //Terminar la partida
        }
        UIController.Instance.HpUpdate(Hp);
    }
}
