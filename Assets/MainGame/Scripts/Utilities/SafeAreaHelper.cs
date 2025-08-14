using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaHelper : MonoBehaviour
{   
    void Start()
    {
        UpdateSafeArea();
    }

    void Update()
    {
#if UNITY_EDITOR
        UpdateSafeArea();
#endif
    }    

    void UpdateSafeArea()
    {
        Rect safeRect = Screen.safeArea;
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 anchorPos = rect.anchoredPosition;
        anchorPos.x = safeRect.x / 2;
        Vector2 size = rect.sizeDelta;
        size.x = -safeRect.x;
        rect.anchoredPosition = anchorPos;
        rect.sizeDelta = size;
    }
}
