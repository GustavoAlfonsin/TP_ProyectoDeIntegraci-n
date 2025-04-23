using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    // Información del enemigo
    private float hpMax, currentHp;
    private float damage = 5;
    private float damageCountDown = 5;
    private bool justAttacked;

    // Componentes
    private NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        hpMax = 100;
        currentHp = hpMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, _player.position) < 2)
        {
            if (!justAttacked)
            {
                _player.GetComponent<PlayerController>().getDamage(damage);
                StartCoroutine(attackAgain());
                Debug.Log("recien ataco al jugador");
            }
        }
        else
        {
            _agent.SetDestination(_player.position);
        }
    }

    public void getDamage(float injury)
    {
        currentHp -= injury;
        if (currentHp <= 0)
        {
            currentHp = 0;
            //animacion de muerte
        }
        Debug.Log($"vida Zombie: {currentHp}");
    }

    IEnumerator attackAgain()
    {
        justAttacked = true;
        yield return new WaitForSeconds(damageCountDown);
        justAttacked = false;
    }
}
