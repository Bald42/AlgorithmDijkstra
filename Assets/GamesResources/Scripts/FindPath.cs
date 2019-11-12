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
    private List<DijkstraGraphPoint> graphPoints = new List<DijkstraGraphPoint> ();
    //private List<DijkstraGraphPoint> graphPoints = new List<DijkstraGraphPoint>();

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
        for (int i=0; i < Graph.Instance.Points.Count; i++)
        {
            DijkstraGraphPoint newGraphPoint = new DijkstraGraphPoint();
            newGraphPoint.Point = Graph.Instance.Points[i].Point;
            newGraphPoint.NextPoints = new List<DijkstraGraphPoint>();

            for (int j = 0; j < Graph.Instance.Points[i].Edges.Count; j ++)
            {
                DijkstraGraphPoint newGraphPoint2 = new DijkstraGraphPoint();
                newGraphPoint2.Weight = Graph.Instance.Points[i].Edges[j].Weight;                
                newGraphPoint2.Point = Graph.Instance.Points[i].Edges[j].PointEdge;
                newGraphPoint.NextPoints.Add(newGraphPoint2);
            }
            graphPoints.Add(newGraphPoint);
        }
        */
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
                }

                if (point1 == _point)
                {                    
                    point1 = null;
                }
                else
                {
                    point1 = _point;
                    OnPoint(point1, ColorMaterials.Color.Red);
                    GetPath();
                }
                /*
                //TODO исправить на отрицание
                if (IsFindPathDijkstra())
                {
                    OnPoint(point1, ColorMaterials.Color.Red);
                }
                else
                {
                    OnPoint(point1, ColorMaterials.Color.Green);
                }
                */
            }
        }
    }


    private void GetPath()
    {
        NewListDijlstra();
        GameObject newPoint = point0;
        int weightPath = 0;
        bool isFind = false;

        while (!isFind)
        {
            for (int i = 0; i < graphPoints.Count; i++)
            {
                if (Graph.Instance.Points[i].Point == newPoint)
                {
                    Debug.LogError("0");
                    for (int j = 0; j < Graph.Instance.Points[i].Edges.Count; j++)
                    {
                        for (int k = 0; k < Graph.Instance.Points.Count; k++)
                        {
                            if (Graph.Instance.Points[i].Edges[j].PointEdge == Graph.Instance.Points[k].Point)
                            {
                                Debug.LogError("1");
                                Debug.LogError("Graph.Instance.Points[k].Point = " + Graph.Instance.Points[k].Point);
                                if (Graph.Instance.Points[i].Edges[j].Weight < graphPoints[k].Weight)
                                {
                                    Debug.LogError("2");
                                    Debug.LogError("graphPoints[k].Weight = " + graphPoints[k].Weight);
                                    graphPoints[k].Weight = Graph.Instance.Points[i].Edges[j].Weight + weightPath;
                                    graphPoints[k].index = i;
                                }
                            }
                        }
                    }
                    break;
                }
            }

            int min = int.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < graphPoints.Count; i++)
            {
                if (!graphPoints[i].isActive && graphPoints[i].Weight < min)
                {
                    min = graphPoints[i].Weight;
                    minIndex = i;
                }
            }            

            if (minIndex == -1)
            {
                Debug.LogError("NotFind");
                return;
            }

            graphPoints[minIndex].isActive = true;
            weightPath += min;

            if (Graph.Instance.Points[minIndex].Point == point1)
            {
                Debug.LogError("find");                
                isFind = true;
            }
            else
            {
                newPoint = graphPoints[minIndex].Point;
            }
        }



    }



    private void NewListDijlstra ()
    {
        graphPoints.Clear();        

        for (int i = 0; i < Graph.Instance.Points.Count; i++)
        {
            DijkstraGraphPoint newPoint = new DijkstraGraphPoint();

            if (Graph.Instance.Points[i].Point == point0)
            {
                newPoint.Weight = 0;
                newPoint.isActive = true;
            }
            
            newPoint.Point = Graph.Instance.Points[i].Point;
            graphPoints.Add(newPoint);
        }        
    }

    /// <summary>
    /// Поиск пути
    /// </summary>
    /*private void GetPath()
    {
        DijkstraGraphPoint p1 = new DijkstraGraphPoint();
        DijkstraGraphPoint p2 = new DijkstraGraphPoint();
        List<DijkstraGraphPoint> notVisited = new List<DijkstraGraphPoint>();

        for (int i = 0; i < Graph.Instance.Points.Count; i++)
        {
            if (Graph.Instance.Points[i].Point == point0)
            {
                p1 = graphPoints[i];
            }

            if (Graph.Instance.Points[i].Point == point1)
            {
                p2 = graphPoints[i];
            }

            notVisited.Add(graphPoints[i]);
        }

        Dictionary<DijkstraGraphPoint, DijkstraPoint> path = new Dictionary <DijkstraGraphPoint, DijkstraPoint> ();

        path.Add(p1, new DijkstraPoint() { PrevPoint = null, Weight = 0 });

        Debug.LogError("0");

        while (true)
        {
            DijkstraGraphPoint currentPoint = null;
            int bestWeight = int.MaxValue;
            Debug.LogError("1");
            foreach (var p in notVisited)
            {
                if (path.ContainsKey(p) && path[p].Weight < bestWeight)
                {
                    
                    currentPoint = p;
                    bestWeight = path[p].Weight;
                    Debug.LogError("2");
                    Debug.LogError("currentPoint = " + currentPoint);
                }
            }

            if (currentPoint == null)
            {
                //нет пути
                Debug.LogError("6");
                return;
            }

            if (currentPoint == p2)
            {
                Debug.LogError("7");
                break;
            }

            foreach (var next in currentPoint.NextPoints)
            {
                Debug.LogError("3");
                int w = path[currentPoint].Weight + next.Weight;
                Debug.LogError("path[currentPoint].Weight = " + path[currentPoint].Weight);
                Debug.LogError("next.Weight = " + next.Weight);

                if (!path.ContainsKey(next) || path[next].Weight > w)
                {
                    Debug.LogError("333");
                    path[next] = new DijkstraPoint() { PrevPoint = currentPoint, Weight = w };
                }
            }

            notVisited.Remove(currentPoint);
        }

        List<DijkstraGraphPoint> ret = new List<DijkstraGraphPoint>();

        var end = p2;

        Debug.LogError("4");
        while (end != null)
        {
            Debug.LogError("5");
            ret.Add(end);
            end = path[end].PrevPoint;
            OnPoint(end.Point, ColorMaterials.Color.Red);
        }
    }*/


}

[System.Serializable]
public class DijkstraGraphPoint
{
    public int Weight = int.MaxValue;
    public GameObject Point = null;
    public bool isActive = false;
    public int index = 0;
}

/*
[System.Serializable]
public class DijkstraGraphPoint
{
    public int Weight = 0;
    public GameObject Point = null;
    public List<DijkstraGraphPoint> NextPoints;
}

[System.Serializable]
public class DijkstraPoint
{
    public int Weight = 0;
    public DijkstraGraphPoint PrevPoint;
}
*/

