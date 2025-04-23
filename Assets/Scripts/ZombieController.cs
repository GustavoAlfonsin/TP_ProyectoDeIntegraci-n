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
    private Vector3[] puntosRuta;
    private int puntoRutaActual = 0;
    private LineRenderer LineaAlObjetivo;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        hpMax = 100;
        currentHp = hpMax;
        puntosRuta = RutasEnemigos.instance.ObtenerRutaAleatoria();
        EstablecerPosicion();
        LineaAlObjetivo = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanciaAlPersonaje = Vector3.Distance(transform.position, _player.position);

        if (distanciaAlPersonaje < 1.5)
        {
            if (!justAttacked)
            {
                _player.GetComponent<PlayerController>().getDamage(damage);
                StartCoroutine(attackAgain());
                Debug.Log("recien ataco al jugador");
                return;
            }
        }
        if (distanciaAlPersonaje < 5f)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            if (Vector3.Distance(transform.position, puntosRuta[puntoRutaActual]) < 1)
            {
                puntoRutaActual = (puntoRutaActual + 1) % puntosRuta.Length;
            }
            _agent.SetDestination(puntosRuta[puntoRutaActual]);
        }

        DibujarRuta();
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

    private void EstablecerPosicion()
    {
        _agent.enabled = false;
        transform.position = puntosRuta[puntoRutaActual];
        _agent.enabled = true;
    }

    private void DibujarRuta()
    {
        if (_agent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            LineaAlObjetivo.positionCount = _agent.path.corners.Length;
            LineaAlObjetivo.SetPositions(_agent.path.corners);
        }
        else
        {
            LineaAlObjetivo.positionCount = 0;
        }
    }
}
