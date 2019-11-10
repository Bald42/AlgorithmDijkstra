using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 editorVector = Vector3.zero;
    float distanceBetweenPoints = 0f;
    GameObject point0 = null;
    GameObject point1 = null;

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
                                                   PositionInCanvas(Graph.Instance.Points[i].transform.position), 
                                                   Quaternion.identity, 
                                                   parentNames);

            string namePoint = Graph.Instance.Points[i].name;
            newNameObject.name = "Interface_" + namePoint;
            newNameObject.GetComponent<ViewText>().View(namePoint);
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
        for (int i=0; i < Graph.Instance.Edges.Count; i ++)
        {
            point0 = Graph.Instance.Edges[i].Point0;
            point1 = Graph.Instance.Edges[i].Point1;

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

            ViewArrow();
            ViewWeights(i);
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
    private void ViewWeights(int _numberEdge)
    {
        GameObject newWeightsObject = Instantiate(prefabWeight,
                                                  PositionInCanvas(editorVector),
                                                  Quaternion.identity,
                                                  parentWeights);

        string nameEdge = point0.name + "_" + point1.name;
        newWeightsObject.name = "InterfacenewWeights_" + nameEdge;
        newWeightsObject.GetComponent<ViewText>().View(Graph.Instance.Edges[_numberEdge].Weight.ToString());
    }
    #endregion StartViewGraph


}