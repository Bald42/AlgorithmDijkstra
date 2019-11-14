using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Выводим текст
/// </summary>
public class ViewText : MonoBehaviour
{
    private Text text = null;

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Init ()
    {
        FindText();
    }

    /// <summary>
    /// Находим компонент текст
    /// </summary>
    private void FindText ()
    {
        text = GetComponentInChildren<Text>();
    }

    /// <summary>
    /// Выводим текст
    /// </summary>
    /// <param name="_newText"></param>
    public void View (string _newText)
    {
        text.text = _newText;
    }
}