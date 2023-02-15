using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int numbersOfDetectVectors = 12;
    public Camera mainCamera;

    private void Awake()
    {
        float angle = 360f / numbersOfDetectVectors;
        for (int i = 0; i < numbersOfDetectVectors; i++)
        {
            var rotateVector = Quaternion.AngleAxis(angle * i, Vector3.forward) * Vector2.up;
            //Vector2 vector = Vector2.up * rotateVector * i;
            Directions.detectDirections.Add(rotateVector.normalized);
        }
    }
}
