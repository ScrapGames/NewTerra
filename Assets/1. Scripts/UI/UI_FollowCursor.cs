using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FollowCursor : MonoBehaviour
{
#pragma warning disable CS0649

    const float PIVOT_HEIGHT_BUFFER = 1.44f;
    [SerializeField, Range(0, 1)] private float smoothness = 0.2f;
    [SerializeField, Range(0, 1)] private float xScreenOffset;
    [SerializeField, Range(0, 1)] private float xScreenFlipThreshold, xScreenFlipReverseThreshold;
    private RectTransform rt;
    private Transform cursor;
    private int screenHeight, screenWidth, xMul;


    private Vector2 vel;
    private Vector2 targetPivot;
    private Vector2 targetPos;

    private void Update()
    {
        Follow();
    }

    private void Start()
    {
        cursor = UIManager.Instance.UI_Cursor.cursor.transform;
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        rt = transform as RectTransform;
        targetPivot = rt.pivot;
        xScreenOffset *= screenWidth;
        xMul = -1;
    }

    private void Follow()
    {
        targetPos = cursor.position;

        // Set pivot point based on cursor Y pos
        targetPivot.y = (targetPos.y / screenHeight) * PIVOT_HEIGHT_BUFFER;

        // Check if X is over flip threshold        
        float xPosNormalized = targetPos.x / screenWidth;
        if (xPosNormalized > xScreenFlipThreshold)
            xMul = -1;
        else if (xPosNormalized < xScreenFlipReverseThreshold)
            xMul = 1;

        rt.pivot = targetPivot;

        // Set x Offset
        targetPos.x += xScreenOffset * xMul;

        transform.position = Vector2.SmoothDamp(transform.position, targetPos, ref vel, smoothness);
    }
}
