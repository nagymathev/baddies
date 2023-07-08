using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowWhenSpawned : MonoBehaviour
{
    [Tooltip("The duration it takes for the GameObject to grow to full scale")]
    [SerializeField] private float _growDuration = 0.250f;
    private float _startTime;
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        _startTime = Time.time;
    }

    void Update()
    {
        float t = (Time.time - _startTime) / _growDuration;
        var smoothing = Mathf.SmoothStep(0, 1, t);
        gameObject.transform.localScale = new Vector3(smoothing, smoothing, smoothing);
    }
}
