using System.Collections;
using System.Collections.Generic;
using Code.GameState;
using UnityEngine;

public class SpawnOnMouse : MonoBehaviour
{
    private Camera _camera;
    private Vector2 _mousePos;

    [Tooltip("The distance for the raycast hit, or other debugging lines")]
    private const float _pointDistance = 100f;

    [SerializeField] private Enemy _enemyPrefab;

    [Tooltip("The audio that plays when the player can spawn enemies")]
    [SerializeField] private AudioClip _canSpawnAudio;
    [Tooltip("The audio that plays when the player cannot spawn enemies")]
    [SerializeField] private AudioClip _cannotSpawnAudio;
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
        if (!Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, _pointDistance))
        {
            PlayAudio(_cannotSpawnAudio);
            return;
        }

        if (!hit.transform.gameObject.CompareTag("CanSpawnEnemiesHere"))
        {
            PlayAudio(_cannotSpawnAudio);
            return;
        }
        SpawnEnemy(hit.point);
    }

    private void SpawnEnemy(Vector3 spawnPoint)
    {
        if(!StateBehaviour.Instance.CanSpawnMinion(this, _enemyPrefab)) return;

        var obj = Instantiate(_enemyPrefab, spawnPoint, gameObject.transform.rotation);
        EventManager.Instance.MinionSpawned(this, obj);
        obj.gameObject.SetActive(true);
        AudioSource.PlayClipAtPoint(_canSpawnAudio, transform.position);
    }

    private void PlayAudio(AudioClip audio)
    {
        AudioSource.PlayClipAtPoint(audio, transform.position);
    }
}
