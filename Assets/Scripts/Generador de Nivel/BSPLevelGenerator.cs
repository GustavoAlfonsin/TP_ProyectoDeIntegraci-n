using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPLevelGenerator : MonoBehaviour
{
    [Header("Tamaño del mapa")]
    public int columns = 5;
    public int rows = 5;

    [Header("Tamaño de cada Habitación")]
    public float roomWidth = 10f;
    public float roomHeight = 10f;
    public int distancia = 10;

    [Header("Prefab habitación")]
    public GameObject roomPrefab;

    [Header("Colores")]
    public Color colorEntrada = Color.red;
    public Color ColorSalida = Color.green;
    public List<Color> coloresHabitacion = new List<Color>();

    [Header("Prefab del Jugador")]
    public GameObject playerPrefab;

    private void Start()
    {
        GenerarMapa();
    }

    private void GenerarMapa()
    {
        List<Vector2Int> posiciones = new List<Vector2Int>();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                posiciones.Add(new Vector2Int(x * distancia, y * distancia));
            }
        }

        Shuffle(posiciones);

        for (int i = 0; i < posiciones.Count; i++) 
        {
            Vector2Int pos = posiciones[i];

            Vector3 posicionMundo = new Vector3(
                pos.x * roomWidth,
                0,
                pos.y * roomHeight
                );

            GameObject room = Instantiate(roomPrefab, posicionMundo,Quaternion.identity);
            room.transform.parent = this.transform;

            Renderer renderer = room.GetComponent<Renderer>();
            if (i == 0)
            {
                renderer.material.color = colorEntrada;
                room.name = "Entrada";
                Instantiate(playerPrefab, posicionMundo, Quaternion.identity);
            }else if (i == posiciones.Count - 1)
            {
                renderer.material.color = ColorSalida;
                room.name = "Salida";
            }
            else
            {
                renderer.material.color = coloresHabitacion[UnityEngine.Random.Range(0, coloresHabitacion.Count)];
                room.name = "Habitación " + i;
            }
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
}
