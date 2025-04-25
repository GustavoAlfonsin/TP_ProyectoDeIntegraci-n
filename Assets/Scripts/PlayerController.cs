using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    private float maxHp, Hp, maxEnergy, energy;

    //CAMARA
    private float mouseSensitivity = 5f;
    private float verticalRotation = 0f;
    [SerializeField] private float verticalRotationLimit = 45.0f;

    //MIRA
    private Vector2 originalScale, zoomScale;
    [SerializeField] private Image normalPoint, aimPoint;
    [SerializeField] private float zoomSpeed, zoomCapacity;

    //COMPAÑERO 
    [SerializeField] GameObject partner;
    private bool activePartner;
    private float energyExpenditure = 2f;
    private float energyIncrease = 4f;
    private float wearTime = 1.5f;
    private float partnerTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        maxHp = 100;
        Hp = maxHp;
        maxEnergy = 50;
        energy = maxEnergy;
        UIController.Instance.HpUpdate(Hp);
        activePartner = partner.activeInHierarchy;
        partnerTimer = 0;
    }


    void Update()
    {
        moveCharacter();
        moveCamera();
        apuntarYDesapuntar();
        invocarCompañero();
        if (activePartner)
        {
            consumirMagia();
        }
        else
        {
            recargarMagia();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            getDamage(10);
        }
    }

    private void recargarMagia()
    {
        partnerTimer += Time.deltaTime;
        if (partnerTimer >= wearTime)
        {
            partnerTimer = 0;
            energy += energyIncrease;
            if (energy >= maxEnergy)
            {
                energy = maxEnergy;
            }
            UIController.Instance.EnergyUpdate(energy);
        }
    }

    private void consumirMagia()
    {
        partnerTimer += Time.deltaTime;
        if (partnerTimer >= wearTime)
        {
            partnerTimer = 0;
            energy -= energyExpenditure;
            if (energy <= 0)
            {
                energy = 0;
                partner.gameObject.SetActive(false);
                activePartner = false;
            }
            UIController.Instance.EnergyUpdate(energy);
        }
    }

    private void invocarCompañero()
    {
        if (Input.GetKeyDown(KeyCode.G) && partner != null)
        {
            if (!partner.gameObject.activeInHierarchy)
            {
                partner.transform.localPosition = transform.localPosition + Vector3.forward * 1f + Vector3.right * 1.5f;
                partner.gameObject.SetActive(true);
                activePartner = true;
            }
            else
            {
                partner.gameObject.SetActive(false);
                activePartner = false;
            }
        }
    }

    private void apuntarYDesapuntar()
    {
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomCapacity,
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
