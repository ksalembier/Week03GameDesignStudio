using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public GameObject startMarker;
    private Vector3 startPosition;
    [SerializeField] private float frequency = 5f;
    [SerializeField] private float magnitude = 5f;
    [SerializeField] private float offset = 0f;

    void Start()
    {
        startPosition = startMarker.transform.position;
    }

    void Update()
    {
        startPosition = startMarker.transform.position;
        transform.position = startPosition + transform.up * Mathf.Sin(Time.time * frequency + offset) * magnitude;
    }
}
