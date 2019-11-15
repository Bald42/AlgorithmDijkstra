using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Поиск пути
/// </summary>
public class FindPath : MonoBehaviour
{
    public delegate void PoinEventHandler(GameObject newPoint, ColorMaterials color);
    public static event PoinEventHandler OnPoint = delegate { };

    public delegate void ViewTextHandler (string info);
    public static event ViewTextHandler OnViewText = delegate { };

    [SerializeField]
    private Camera mainCamera = null;

    [SerializeField]
    private GameObject point0 = null;

    [SerializeField]
    private GameObject point1 = null;

    [SerializeField]
    private List<DijkstraGraphPoint> graphPoints = new List<DijkstraGraphPoint> ();

    [SerializeField]
    private List<GameObject> dijkstraPath = new List<GameObject>();

    private List<GameObject> tempPath = new List<GameObject>();

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
            OnPoint(point0, ColorMaterials.Red);
        }
        else
        {
            if (point0 == _point)
            {
                OnViewText("");
                OnPoint(point0, ColorMaterials.White);
                ClearPath();
                point0 = null;
                point1 = null;
            }
            else
            {
                if (point1 != _point)
                {   
                    if (point1)
                    {
                        OnPoint(point1, ColorMaterials.White);
                    }
                    point1 = _point;                    
                    GetPath();
                    ViewPath(ColorMaterials.Green);
                }
            }
        }
    }

    /// <summary>
    /// Ищем путь
    /// </summary>
    private void GetPath()
    {
        NewListDijkstra();
        GameObject newPoint = point0;
        int weightPath = 0;
        bool isFind = false;

        ClearPath();

        while (!isFind)
        {
            for (int i = 0; i < graphPoints.Count; i++)
            {
                if (Graph.Instance.Points[i].Point == newPoint)
                {
                    for (int j = 0; j < Graph.Instance.Points[i].Edges.Count; j++)
                    {
                        for (int k = 0; k < Graph.Instance.Points.Count; k++)
                        {
                            if (Graph.Instance.Points[i].Edges[j].PointEdge == Graph.Instance.Points[k].Point)
                            {
                                if (Graph.Instance.Points[i].Edges[j].Weight + weightPath < graphPoints[k].Weight)
                                {
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
                OnViewText("нет пути " + point0.name + "-" + point1.name);
                return;
            }

            graphPoints[minIndex].isActive = true;
            weightPath = min;

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
        CreatePath(point0,point1);        
    }
    
    /// <summary>
    /// Заполняем новый лист графа для поиска пути
    /// </summary>
    private void NewListDijkstra ()
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
    /// Очищаем путь
    /// </summary>
    private void ClearPath()
    {
        for (int i = 1; i < dijkstraPath.Count; i++)
        {
            OnPoint(dijkstraPath[i], ColorMaterials.White);
        }
        dijkstraPath.Clear();
    }

    /// <summary>
    /// Заполняем лист пути
    /// </summary>    
    private void CreatePath(GameObject _point0, GameObject _point1)
    {
        GameObject _currentPoint = _point1;
        tempPath.Clear();
        string _textPath = "";

        while (_currentPoint != _point0)
        {
            tempPath.Add(_currentPoint);
            for (int i = 0; i < graphPoints.Count; i++)
            {
                if (graphPoints[i].Point == _currentPoint)
                {
                    _currentPoint = graphPoints[graphPoints[i].index].Point;
                    break;
                }
            }
        }

        tempPath.Add(_point0);

        for (int i = tempPath.Count - 1; i >= 0; i--)
        {
            dijkstraPath.Add(tempPath[i]);
            _textPath += (i == tempPath.Count - 1 ? "" : " - ") + tempPath[i].name;
        }
        OnViewText(_textPath);
    }

    /// <summary>
    /// Показываем путь
    /// </summary>
    private void ViewPath (ColorMaterials _color)
    {
        for (int i = 1; i < dijkstraPath.Count; i++)
        {
            OnPoint(dijkstraPath[i], _color);
        }

        if (dijkstraPath.Count == 0)
        {
            OnPoint(point1, ColorMaterials.Red);
        }
    }
}

/// <summary>
/// Параметры точки массива
/// </summary>
[System.Serializable]
public class DijkstraGraphPoint
{
    public int Weight = int.MaxValue;
    public GameObject Point = null;
    public bool isActive = false;
    public int index = 0;
}