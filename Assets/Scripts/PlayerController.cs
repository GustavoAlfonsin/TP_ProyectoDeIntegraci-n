using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public enum PlayerStates
{
    normal,
    running,
    crouching,
    pointing
}
[System.Serializable]
public class Weapon
{
    public WeaponItem weaponInfo;
    public GameObject model;
    public int currentAmmo;
    public bool isUnlocked = false;
}

public class PlayerController : MonoBehaviour
{
    #region propiedades Jugador

    private float crouchingSpeed = 1.5f;
    private float walkSpeed = 3.5f;
    private float runSpeed = 7f;
    private CharacterController controller;
    private Animator _animator;
    private Vector3 playerSpeed;
    private float maxHp, Hp;
    private PlayerStates _state;

    #endregion

    #region Camara y mira
    //Posicion de la camara
    public Vector3 followPointUp, followPointDown;
    [SerializeField] Transform camFollowPos;

    //MIRA
    private Vector2 originalScale, zoomScale;
    [SerializeField] private Image normalPoint, aimPoint;
    [SerializeField] private float zoomSpeed, zoomCapacity;
    [HideInInspector] public CinemachineVirtualCamera vCam;
    #endregion

    #region Manejo de las armas
    public List<Weapon> weapons;
    public int currentWeaponIndex = -1;

    #endregion
    void Start()
    {
        controller = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        camFollowPos.transform.localPosition = followPointUp;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        maxHp = 100;
        Hp = maxHp;
        _state = PlayerStates.normal;
        UIController.Instance.HpUpdate(Hp);
    }


    void Update()
    {
        startRunning();
        startCrouching();
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
            _state = PlayerStates.pointing;
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
            if(_state == PlayerStates.pointing)
                _state = PlayerStates.normal;

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
        
        Vector3 movement = transform.right * XMovement + transform.forward * ZMovement;
        movement = movement.normalized;

        switch (_state)
        {  
            case PlayerStates.running:
                controller.Move(movement * runSpeed * Time.deltaTime);
                _animator.SetBool("Corriendo", true);
                _animator.SetBool("Agachado", false);
                break;
            case PlayerStates.crouching:
                controller.Move(movement * crouchingSpeed * Time.deltaTime);
                _animator.SetBool("Corriendo", false);
                _animator.SetBool("Agachado", true);
                break;
            default:
                controller.Move(movement * walkSpeed * Time.deltaTime);
                _animator.SetBool("Corriendo", false);
                _animator.SetBool("Agachado", false);
                break;
        }
        
        _animator.SetFloat("Movimiento X", XMovement);
        _animator.SetFloat("Movimiento Z", ZMovement);
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

    private void startRunning()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _state != PlayerStates.pointing)
        {
            if (_state != PlayerStates.running)
            {
                _state = PlayerStates.running;
                Debug.Log("EL JUGADOR EMPEZO A CORRER");
            }
            else
            {
                _state = PlayerStates.normal;
                Debug.Log("EL JUGADOR DEJO DE CORRER");
            }
        }
    }

    private void startCrouching()
    {
        if (Input.GetKeyDown(KeyCode.E) && _state != PlayerStates.pointing)
        {
            if (_state != PlayerStates.crouching)
            {
                _state = PlayerStates.crouching;
                camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition,
                                                    followPointDown, 30 * Time.deltaTime);
                Debug.Log("EL JUGADOR SE AGACHO");
            }
            else
            {
                _state = PlayerStates.normal;
                camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition,
                                                    followPointUp, 30 * Time.deltaTime);
                Debug.Log("EL JUGADOR SE LEVANTO");
            }
        }
    }

    #region Métodos para el manejo de las armas
    private void handleWeaponSwitch()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? 1 : -1;
            int newIndex = GetNextUnlockedWeaponIndex(direction); 
            if (newIndex != -1 && newIndex != currentWeaponIndex)
            {
                EquipWeapon(newIndex);
            }
        }
    }

    private void handleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentWeaponIndex != -1)
        {
            Weapon currentWeapon = weapons[currentWeaponIndex];
            int needed = currentWeapon.weaponInfo.maxAmmo - currentWeapon.currentAmmo;
            if (needed > 0)
            {
                int taken = UIController.Instance.inventory.TakeAmmo(currentWeapon.weaponInfo._ammoType, needed);
                currentWeapon.currentAmmo += taken;

                UIController.Instance.updateAmmoDisplay(currentWeapon.currentAmmo, currentWeapon.weaponInfo.maxAmmo);
            }
        }
    }

    private void EquipWeapon(int newIndex)
    {
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].model.SetActive(false);
        }
        currentWeaponIndex = newIndex;
        Weapon weapon = weapons[newIndex];
        weapon.model.SetActive(true);

        UIController.Instance.updateWeaponDisplay(weapon.weaponInfo.name, weapon.currentAmmo, weapon.weaponInfo.maxAmmo);
    }

    private int GetNextUnlockedWeaponIndex(int direction)
    {
        int startIndex = currentWeaponIndex;
        int count = weapons.Count;

        for (int i = 1; 0 < count; i++)
        {
            int index = (startIndex + i * direction + count) % count;
            if (weapons[index].isUnlocked)
            {
                return index;
            }
        }
        return startIndex;
    }

    public void UnlockWeapon(string weaponName)
    {
        int index = weapons.FindIndex(w => w.weaponInfo.name == weaponName);
        if (index != -1)
        {
            weapons[index].isUnlocked = true;
            if (currentWeaponIndex == -1)
            {
                EquipWeapon(index);
            }
        }
    }
    #endregion
}
