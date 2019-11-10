using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Создаём граф
/// </summary>
public class Graph : MonoBehaviour
{
    public static Graph Instance = null;

    public List<GameObject> Points = new List<GameObject>();

    public List<Edge> Edges = new List<Edge>();

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Init ()
    {
        ApplyInstance(this);
    }

    private void OnDestroy()
    {
        ApplyInstance(null);
    }

    /// <summary>
    /// Применяем синглтон
    /// </summary>
    /// <param name="graph"></param>
    private void ApplyInstance (Graph graph)
    {
        Instance = graph;
    }
}

/// <summary>
/// Ребро
/// </summary>
[System.Serializable]
public class Edge
{
    public GameObject Point0 = null;

    public GameObject Point1 = null;

    public int Weight = 0;
}