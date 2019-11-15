using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображаем граф
/// </summary>
public class ViewGraph : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera = null;

    [Header("Names")]
    [SerializeField]
    private Transform parentNames = null;

    [SerializeField]
    private GameObject prefabName = null;

    [Header("Edges")]
    [SerializeField]
    private Transform parentEdges = null;

    [SerializeField]
    private GameObject prefabEdge = null;

    [SerializeField]
    private Transform parentArrows = null;

    [SerializeField]
    private GameObject prefabArrow = null;

    [Header("Weights")]
    [SerializeField]
    private Transform parentWeights = null;

    [SerializeField]
    private GameObject prefabWeight = null;

    [Header("Materials")]
    [SerializeField]
    private Material materialWhite = null;

    [SerializeField]
    private Material materialGreen = null;

    [SerializeField]
    private Material materialRed = null;

    [Header("Text")]
    [SerializeField]
    private Text textInfo = null;

    private Vector3 editorVector = Vector3.zero;

    private float distanceBetweenPoints = 0f;

    private GameObject point0 = null;
    private GameObject point1 = null;

    private List<MeshRenderer> pointsMesh = new List<MeshRenderer>();
    private List<EdgeMesh> edgeMeshs = new List<EdgeMesh>();
    private EdgeMesh newEdgeMesh = new EdgeMesh ();

    #region Subscribes / UnSubscribes
    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    /// <summary>Подписки</summary>
    private void Subscribe()
    {
        FindPath.OnViewArrows += OnViewArrows;
        FindPath.OnViewPoint += OnViewPoint;
        FindPath.OnViewText += OnViewText;
    }

    /// <summary>Отписки</summary>
    private void UnSubscribe()
    {
        FindPath.OnViewArrows -= OnViewArrows;
        FindPath.OnViewPoint -= OnViewPoint;
        FindPath.OnViewText -= OnViewText;
    }

    /// <summary>
    /// Обработчик события вывода информации
    /// </summary>
    /// <param name="_info"></param>
    private void OnViewText (string _info)
    {
        textInfo.text = _info;
    }

    /// <summary>
    /// Обработчик события выделения вершины графа
    /// </summary>
    private void OnViewPoint (GameObject _newPoint, ColorMaterials _color)
    {
        for (int i=0; i < pointsMesh.Count; i++)
        {
            if (pointsMesh[i].gameObject == _newPoint)
            {
                ApplyColor(pointsMesh[i], _color);
                break;
            }
        }
    }

    /// <summary>
    /// Обработчик события отрисовки рёбер
    /// </summary>
    /// <param name="_listPoints"></param>
    /// <param name="_color"></param>
    private void OnViewArrows (List <GameObject> _listPoints, ColorMaterials _color)
    {
        for (int i=0; i < _listPoints.Count - 1; i++)
        {
            string _newName = _listPoints[i].name + "-" + _listPoints[i + 1].name;            

            for (int j=0; j < edgeMeshs.Count; j++)
            {
                if (_newName == edgeMeshs[j].NameEdge)
                {
                    for (int k = 0; k < edgeMeshs[j].Mesh.Count; k++)
                    {
                        ApplyColor(edgeMeshs[j].Mesh[k], _color);
                    }
                    break;
                }
            }
        }
    }
    #endregion Subscribes / UnSubscribes 

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Инициалазиция
    /// </summary>
    private void Init ()
    {
       StartCoroutine(StartView());
    }

    #region StartViewGraph
    /// <summary>
    /// Показываем граф на старте
    /// </summary>
    private IEnumerator StartView ()
    {
        while (!Graph.Instance)
        {
            yield return null;
        }
        ViewNames();
        ViewEdges();
    }

    /// <summary>
    /// Показываем имена вершин
    /// </summary>
    private void ViewNames ()
    {
        for (int i = 0; i < Graph.Instance.Points.Count; i++)
        {
            GameObject newNameObject = Instantiate(prefabName, 
                                                   PositionInCanvas(Graph.Instance.Points[i].Point.transform.position), 
                                                   Quaternion.identity, 
                                                   parentNames);

            string namePoint = Graph.Instance.Points[i].Point.name;
            newNameObject.name = "Interface_" + namePoint;
            newNameObject.GetComponent<ViewText>().View(namePoint);
            pointsMesh.Add(Graph.Instance.Points[i].Point.GetComponent<MeshRenderer>());
        }
    }

    /// <summary>
    /// Находим координату на канвасе
    /// </summary>
    /// <param name="_positionPoint"></param>
    /// <returns></returns>
    private Vector3 PositionInCanvas(Vector3 _positionPoint)
    {
        editorVector.x = mainCamera.WorldToViewportPoint(_positionPoint).x * Screen.width;
        editorVector.y = mainCamera.WorldToViewportPoint(_positionPoint).y * Screen.height;
        editorVector.z = 0;

        return editorVector;
    }

    /// <summary>
    /// Отрисовываем рёбра
    /// </summary>
    private void ViewEdges ()
    {
        for (int i=0; i < Graph.Instance.Points.Count; i ++)
        {            
            point0 = Graph.Instance.Points[i].Point;
            for (int j = 0; j < Graph.Instance.Points[i].Edges.Count; j++)
            {
                newEdgeMesh = new EdgeMesh();

                point1 = Graph.Instance.Points[i].Edges[j].PointEdge;
                int _weight = Graph.Instance.Points[i].Edges[j].Weight;

                distanceBetweenPoints = (point0.transform.position -
                                         point1.transform.position).magnitude;

                PointOnSegment(point0.transform.position, point1.transform.position, 0.5f);

                GameObject newEdge = Instantiate(prefabEdge,
                                                 editorVector,
                                                 Quaternion.identity,
                                                 parentEdges);

                newEdge.transform.LookAt(point1.transform);

                newEdge.name = "Edge_" + point0.name + "_" + point1.name;

                editorVector = newEdge.transform.localScale;
                editorVector.z = distanceBetweenPoints;
                newEdge.transform.localScale = editorVector;

                newEdgeMesh.Mesh.Add(newEdge.GetComponent<MeshRenderer>());
                newEdgeMesh.NameEdge = point0.name + "-" + point1.name;

                ViewArrow();
                ViewWeights(_weight);

                edgeMeshs.Add(newEdgeMesh);
            }
        }
    }

    /// <summary>
    /// Отрисовываем направление рёбер
    /// </summary>
    private void ViewArrow ()
    {
        PointOnSegment(point0.transform.position, point1.transform.position, 0.8f);

        GameObject newArrow = Instantiate(prefabArrow,
                                          editorVector,
                                          Quaternion.identity,
                                          parentArrows);

        newArrow.transform.LookAt(point1.transform);
        newArrow.name = "ArrowEdge_" + point0.name + "_" + point1.name;

        MeshRenderer [] newArrowMeshs = newArrow.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < newArrowMeshs.Length; i++)
        {
            newEdgeMesh.Mesh.Add(newArrowMeshs[i]);
        }
    }

    /// <summary>
    /// Находим координату точки на отрезке
    /// </summary>
    private void PointOnSegment (Vector3 _point0, Vector3 _point1, float _distance)
    {
        editorVector = _point0 + (_point1 - _point0) * ((distanceBetweenPoints * _distance) / distanceBetweenPoints);
        editorVector.y = 0;
    }

    /// <summary>
    /// Показываем веса рёбер
    /// </summary>
    private void ViewWeights(int _weight)
    {
        GameObject newWeightsObject = Instantiate(prefabWeight,
                                                  PositionInCanvas(editorVector),
                                                  Quaternion.identity,
                                                  parentWeights);

        string nameEdge = point0.name + "_" + point1.name;
        newWeightsObject.name = "InterfaceWeights_" + nameEdge;
        newWeightsObject.GetComponent<ViewText>().View(_weight.ToString());
    }
    #endregion StartViewGraph

    /// <summary>
    /// Применяем цвет на объектах
    /// </summary>
    /// <param name="_objectMaterial"></param>
    /// <param name="_color"></param>
    private void ApplyColor (MeshRenderer _mesh, ColorMaterials _newColor)
    {
        switch (_newColor)
        {
            case ColorMaterials.White:
                {
                    _mesh.material = materialWhite;
                    break;
                }
            case ColorMaterials.Green:
                {
                    _mesh.material = materialGreen;
                    break;
                }
            case ColorMaterials.Red:
                {
                    _mesh.material = materialRed;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}

/// <summary>
/// Меши ребра
/// </summary>
[System.Serializable]
public class EdgeMesh
{
    public string NameEdge = "";
    public List <MeshRenderer> Mesh = new List<MeshRenderer> ();
}