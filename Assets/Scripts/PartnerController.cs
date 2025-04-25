using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PartnerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private GameObject _player;
    private GameObject _target;
    private float damage = 10;
    private float damageCountDown = 2.5f;
    private bool justAttacked = false;
    private bool justShot = false;

    public GameObject firePrefab;
    public Transform fireOrigin;


    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) > 4 && !enemigosEnZona())
        {
            _agent.SetDestination(_player.transform.position);
        }
        else if (enemigosEnZona() && _target != null)
        {
            if (enZonaDeDisparo())
            {
                Disparar();
            }
            else if (estaMuyCerca())
            {
                if (!justAttacked)
                {
                    _target.GetComponent<ZombieController>().getDamage(damage);
                    StartCoroutine(attackAgain());
                    Debug.Log("Mi compañero ataco al enemigo");
                }
                
            }
            else
            {
                _agent.SetDestination(_target.transform.position);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Disparar();
        }
    }

    private bool estaMuyCerca()
    {
        return Vector3.Distance(transform.position, _target.transform.position) <= 2;
    }

    private void Disparar()
    {
        if (!justShot)
        {
            GameObject fireBall = Instantiate(firePrefab, fireOrigin.position,fireOrigin.rotation);
            fireBall.GetComponent<Rigidbody>().AddForce(transform.forward * 500f);
            StartCoroutine(shootAgain());
        }
    }

    private bool enZonaDeDisparo()
    {
        float distance = Vector3.Distance(transform.position, _target.transform.position);
        return distance > 2 && distance <= 10;
    }

    private bool enemigosEnZona()
    {
        return false;
    }

    IEnumerator attackAgain()
    {
        justAttacked = true;
        yield return new WaitForSeconds(damageCountDown);
        justAttacked = false;
    }

    IEnumerator shootAgain()
    {
        justShot = true;
        yield return new WaitForSeconds(damageCountDown);
        justShot = false;
    }
}
