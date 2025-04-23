using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RutaEnemigo
{
    public Vector3[] ruta;
}

public class RutasEnemigos : MonoBehaviour
{
    [SerializeField] private RutaEnemigo[] rutasEnemigos;
    private Color[] coloresRutas;

    public static RutasEnemigos instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        coloresRutas = new Color[rutasEnemigos.Length];
        for (int i = 0; i < rutasEnemigos.Length; i++)
        {
            coloresRutas[i] = GenerarColor();
        }
    }

    private static Color GenerarColor()
    {
        float minValor = 0.5f;

        float r = UnityEngine.Random.Range(minValor, 1f);
        float g = UnityEngine.Random.Range(minValor, 1f);
        float b = UnityEngine.Random.Range(minValor, 1f);

        return new Color(r, g, b);
    }

    public Vector3[] ObtenerRutaAleatoria()
    {
        int indice = UnityEngine.Random.Range(0,rutasEnemigos.Length);
        return rutasEnemigos[indice].ruta;
    }

    public Vector3 ObtenerPosicionAleatoria()
    {
        int indiceRuta = UnityEngine.Random.Range(0, rutasEnemigos.Length);
        int indicePunto = UnityEngine.Random.Range(0, rutasEnemigos[indiceRuta].ruta.Length);

        return rutasEnemigos[indiceRuta].ruta[indicePunto];
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < rutasEnemigos.Length; i++)
            {
                Gizmos.color = coloresRutas[i];
                for (int j = 0; j < rutasEnemigos[i].ruta.Length - 1; j++)
                {
                    Gizmos.DrawLine(rutasEnemigos[i].ruta[j], rutasEnemigos[i].ruta[j + 1]);
                }
            }
        }
    }
}
