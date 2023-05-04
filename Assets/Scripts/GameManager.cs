using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] PlayerBehavior player;
    public PlayerBehavior Player => player;

    public int numbersOfDetectVectors = 12;
    public Camera mainCamera;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerBehavior>();

        float angle = 360f / numbersOfDetectVectors;
        for (int i = 0; i < numbersOfDetectVectors; i++)
        {
            var rotateVector = Quaternion.AngleAxis(angle * i, Vector3.forward) * Vector2.up;
            //Vector2 vector = Vector2.up * rotateVector * i;
            Directions.detectDirections.Add(rotateVector.normalized);
        }
        Debug.Log("Awake GameManager");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
            player = FindAnyObjectByType<PlayerBehavior>();
        }
    }
}

//public class GameManager : MonoBehaviour
//{
//    public static GameManager Instance { get; private set; }

//}
