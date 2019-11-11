using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Создаём граф
/// </summary>
public class Graph : MonoBehaviour
{
    public static Graph Instance = null;

    public List<GraphPoint> Points = new List<GraphPoint>();

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
/// Точка графа
/// </summary>
[System.Serializable]
public class GraphPoint
{
    public GameObject Point = null;
    public List<Edge> Edges = new List<Edge>();
}

/// <summary>
/// Ребро
/// </summary>
[System.Serializable]
public class Edge
{
    public GameObject PointEdge = null;
    public int Weight = 0;
}