using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Поиск пути
/// </summary>
public class FindPath : MonoBehaviour
{
    public delegate void PoinEventHandler(GameObject newPoint, ColorMaterials.Color color);
    public static event PoinEventHandler OnPoint = delegate { };

    [SerializeField]
    private Camera mainCamera = null;

    [SerializeField]
    private GameObject point0 = null;

    [SerializeField]
    private GameObject point1 = null;

    [SerializeField]
    private List<PointGraph> points = new List<PointGraph>();

    [SerializeField]
    private List<Dijkstra> listDijkstra = new List<Dijkstra>();

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Init ()
    {
        StartCoroutine(FindGraph());
    }

    /// <summary>
    /// Поиск  всех путей
    /// </summary>
    /// <returns></returns>
    private IEnumerator FindGraph()
    {
        while (!Graph.Instance)
        {
            yield return null;
        }
        /*
        int countEdge = Graph.Instance.Edges.Count;

        for (int i = 0; i < Graph.Instance.Edges.Count; i++)
        {
            bool isFind = false;

            for (int j = 0; j < points.Count; j++)
            {
                if (Graph.Instance.Edges[i].Point0 == points[j].Point)
                {
                    isFind = true;
                    points[j].Edges.Add(Graph.Instance.Edges[i]);
                    break;
                }
            }

            if (!isFind)
            {
                PointGraph newPointGraph = new PointGraph();
                newPointGraph.Point = Graph.Instance.Edges[i].Point0;
                newPointGraph.Edges.Add(Graph.Instance.Edges[i]);
                points.Add(newPointGraph);
                countEdge--;
            }
        }*/
    }

    private void Update()
    {
        OnMouseClick();
    }

    /// <summary>
    /// Обрабатываем клик мыши
    /// </summary>
    private void OnMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                CheckPoint(hit.transform.gameObject);
            }
        }
    }

    /// <summary>
    /// Обрабатываем в какую вершину попали
    /// </summary>
    private void CheckPoint(GameObject _point)
    {
        if (!point0)
        {
            point0 = _point;
            OnPoint(point0, ColorMaterials.Color.Green);
        }
        else
        {
            if (point0 == _point)
            {
                if (point1)
                {
                    OnPoint(point1, ColorMaterials.Color.White);
                }
                OnPoint(point0, ColorMaterials.Color.White);
                point0 = null;
            }
            else
            {
                if (point1)
                {
                    OnPoint(point1, ColorMaterials.Color.White);
                    point1 = null;
                }

                point1 = _point;

                //TODO исправить на отрицание
                if (IsFindPathDijkstra())
                {
                    OnPoint(point1, ColorMaterials.Color.Red);
                }
                else
                {
                    OnPoint(point1, ColorMaterials.Color.Green);
                }
            }
        }
    }


    private bool IsFindPathDijkstra()
    {
        listDijkstra.Clear();
        int numberStart = 0;
        int numberEnd = 0;
        /*
        for (int i = 0; i < points.Count; i++)
        {
            Dijkstra newDijkstra = new Dijkstra();
            listDijkstra.Add(newDijkstra);

            if (point0 == points[i].Point)
            {
                numberStart = i;
            }

            if (point1 == points[i].Point)
            {
                numberEnd = i;
            }
        }

        listDijkstra[0].isFind = true;
        listDijkstra[0].Point = numberStart;

        //проходим по всем вершинам
        for (int i = 0; i < points.Count; i++)
        {
            //проходим по рёбрам этой вершины
            for (int j = 0; j < points[i].Edges.Count; j++)
            {
                //находим номер смежной вершины
                for (int k = 0; k < points.Count; k++)
                {
                    
                    if (points[i].Edges[j].Point1 == points[k].Point)
                    {
                        if (points[i].Edges[j].Weight < listDijkstra[k].Distance)
                        {
                            listDijkstra[k].Distance = points[i].Edges[j].Weight;
                            listDijkstra[k].Point = i;
                        }
                    }
                    
                }
            }*/
        return true;
    }
}

/// <summary>
/// Ребро
/// </summary>
[System.Serializable]
public class PointGraph
{
    public GameObject Point = null;
    public List<Edge> Edges = new List<Edge>();
}

/// <summary>
/// Элементы массива дейкстры
/// </summary>
[System.Serializable]
public class Dijkstra
{
    public bool isFind = false;
    public int Distance = int.MaxValue;
    public int Point = -1;
}

/*
public class GraphPoint
{
    public int Weight;
    public List<GraphPoint> NextPoints;
}

public class DijkstraPoint
{
    public int Weight;
    public GraphPoint PrevPoint;
}

public List GetPath(List points, GraphPoint p1, GraphPoint p2)
{
    List notVisited = points.ToList();
    Dictionary<GraphPoint, DijkstraPoint> path = new Dictionar <GraphPoint, DijkstraPoint>();

    path.Add(p1, new DijkstraPoint() { PrevPoint = null, Weight = 0 });

    while (true)
    {
        GraphPoint currentPoint = null;
        int bestWeight = int.MaxValue;

        foreach (var p in notVisited)
        {
            if (path.ContainsKey(p) && path[p].Weight < bestWeight)
            {
                currentPoint = p;
                bestWeight = path[p].Weight;
            }
        }

        if (currentPoint == null)
        {
            return null;
        }

        if (currentPoint == p2)
        {
            break;
        }

        foreach (var next in currentPoint.NextPoints)
        {
            int w = path[currentPoint].Weight + next.Weight;
            if (!path.ContainsKey(next) || path[next].Weight > w)
            {
                path[next] = new DijkstraPoint() { PrevPoint = currentPoint, Weight = w };
            }
        }

        notVisited.Remove(currentPoint);
    }

    List ret = new List <GameObject> ();

    var end = p2;

    while (end != null)
    {
        ret.Add(end);
        end = path[end].PrevPoint;
    }

    return ret;
}
*/
