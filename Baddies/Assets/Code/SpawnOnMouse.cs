using System.Collections;
using System.Collections.Generic;
using Code.GameState;
using UnityEngine;

public class SpawnOnMouse : MonoBehaviour
{
    private Camera _camera;
    private Vector2 _mousePos;

    [Tooltip("The distance for the raycast hit, or other debugging lines")]
    [SerializeField] private float _pointDistance = 30f;

    [SerializeField] private GameObject _enemyPrefab;
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        var _mousePos = new Vector3(
            Mathf.Clamp(Input.mousePosition.x, 0, Screen.width),
            Mathf.Clamp(Input.mousePosition.y, 0, Screen.height),
            _pointDistance); // Z distance from camera

        var worldPoint = _camera.ScreenToWorldPoint(_mousePos);
        Debug.DrawLine(transform.position, worldPoint, Color.magenta, 0.1f);

        if (Input.GetMouseButtonDown(0)) ShootRaycast(worldPoint);
    }

    private void ShootRaycast(Vector3 worldPoint)
    {
        var direction = worldPoint - transform.position;
        if (!Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, _pointDistance)) return;
        SpawnEnemy(hit.point);
    }

    private void SpawnEnemy(Vector3 spawnPoint)
    {
        var obj = Instantiate(_enemyPrefab, spawnPoint, gameObject.transform.rotation);
        EventManager.Instance.MinionSpawned(this, obj.GetComponent<Enemy>());
        obj.SetActive(true);
    }
}
