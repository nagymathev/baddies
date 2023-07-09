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

    [Tooltip("The amount of cooldown between spawning enemies")]
    [SerializeField] private float _spawnCooldown = 1f;
    private float _cooldownStartTime;
    [SerializeField] private bool _isOnCoolDown;

    [SerializeField] private Enemy _enemyPrefab;

    [Tooltip("The audio that plays when the player can spawn enemies")]
    [SerializeField] private GameObject _canSpawnAudio;
    [Tooltip("The audio that plays when the player cannot spawn enemies")]
    [SerializeField] private GameObject _cannotSpawnAudio;
    [Tooltip("The audio that plays when the spawning is on cooldown")]
    [SerializeField] private GameObject _onCooldownAudio;
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

		//ToDo: ALWAYS do the raycast and render a "cursor" to show whether you could or couldn't spawn there 
		// (and a cooldown timer when on cooldown)

        _isOnCoolDown = (Time.time - _cooldownStartTime) / _spawnCooldown <= _spawnCooldown;
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isOnCoolDown)
            {
                ShootRaycast(worldPoint);
            }
            else
            {
                PlayAudio(_onCooldownAudio);
            }
        }
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
        _cooldownStartTime = Time.time;
    }

    private void SpawnEnemy(Vector3 spawnPoint)
    {
        if(!StateBehaviour.Instance.CanSpawnMinion(this, _enemyPrefab)) return;

		var obj = Instantiate(_enemyPrefab, spawnPoint, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));// gameObject.transform.rotation);
        EventManager.Instance.MinionSpawned(this, obj);
        obj.gameObject.SetActive(true);
        PlayAudio(_canSpawnAudio);
    }

    private void PlayAudio(GameObject audioPrefab)
    {
		if (!audioPrefab) return;

        Instantiate(audioPrefab, transform.position, transform.rotation, transform);
    }
}
