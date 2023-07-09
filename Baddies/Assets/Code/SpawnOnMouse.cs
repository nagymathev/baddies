using System.Collections;
using System.Collections.Generic;
using Code.GameState;
using UnityEngine;

public class SpawnOnMouse : MonoBehaviour
{
	public Camera _camera;
	public Vector2 _mousePos;

    [Tooltip("The distance for the raycast hit, or other debugging lines")]
    public float _pointDistance = 100f;

    [Tooltip("The amount of cooldown between spawning enemies")]
    [SerializeField]
	public float _spawnCooldown = 1f;
    public float cooldownTimer;
    [SerializeField]
	public bool _isOnCoolDown;

    [SerializeField]
	public Enemy _enemyPrefab;

	public GameObject playerInstance;

    [Tooltip("The audio that plays when the player can spawn enemies")]
    [SerializeField]
	public GameObject _canSpawnAudio;
    [Tooltip("The audio that plays when the player cannot spawn enemies")]
    [SerializeField]
	public GameObject _cannotSpawnAudio;
    [Tooltip("The audio that plays when the spawning is on cooldown")]
    [SerializeField]
	public GameObject _onCooldownAudio;

	public bool hitValid;
	public RaycastHit hit;
	public bool canSpawnThere;
	public bool notInView;

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

		// ALWAYS do the raycast and render a "cursor" to show whether you could or couldn't spawn there (and why)
		// (and a cooldown timer when on cooldown)

		playerInstance = !Gameplay.singleton ? null :
						Gameplay.singleton.players.Count == 0 ? null :
						Gameplay.singleton.players[0];

		var direction = worldPoint - transform.position;
		hitValid = Physics.Raycast(transform.position, direction.normalized, out hit, _pointDistance);
		if (hitValid)
		{
			canSpawnThere = hit.transform.gameObject.CompareTag("CanSpawnEnemiesHere");
			//ToDo: also check line of sight to the player; spawning in view is cheating
			if (playerInstance)
			{
				Vector3 playerEye = playerInstance.transform.position + Vector3.up;
				Vector3 spawnPos = hit.point + Vector3.up;
				notInView = Physics.Raycast(playerEye, spawnPos - playerEye, (spawnPos - playerEye).magnitude - 1.0f);
			}
		}

		if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

		_isOnCoolDown = cooldownTimer > 0;

        if (Input.GetMouseButtonDown(0))
        {
			if (_isOnCoolDown)
			{
				PlayAudio(_onCooldownAudio);
			} else
			if (!hitValid || !canSpawnThere || !notInView)
			{
				PlayAudio(_cannotSpawnAudio);
			} else
            {
				SpawnEnemy(hit.point);
				cooldownTimer = _spawnCooldown;
           }
        }
    }

    void SpawnEnemy(Vector3 spawnPoint)
    {
        if(!StateBehaviour.Instance.CanSpawnMinion(this, _enemyPrefab)) return;

		var obj = Instantiate(_enemyPrefab, spawnPoint, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));// gameObject.transform.rotation);
        EventManager.Instance.MinionSpawned(this, obj);
        obj.gameObject.SetActive(true);
        PlayAudio(_canSpawnAudio);
    }

    void PlayAudio(GameObject audioPrefab)
    {
		if (!audioPrefab) return;

        Instantiate(audioPrefab, transform.position, transform.rotation, transform);
    }
}
