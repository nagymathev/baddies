﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
	public static Gameplay singleton;

	public List<GameObject> players =  new List<GameObject>();
	public List<GameObject> monsters = new List<GameObject>();
	public List<GameObject> goals = new List<GameObject>(); //incl. pickups

	//ToDo: direct some kind of enemy waves
	public enum State
    {
        Wait,
        Wave,
        GameOver
    }

//    public Leaderboard leaderboard;

    public CameraController camCon;
    public GameObject player;
    public GameObject[] enemyTypes;
    public Spawner[] spawners;
    public float[] spawnScales;
    public float waveTime = 10.0f;  //seconds
    public float waitTime = 10.0f;  //seconds

    public State state;
    public float timer;
    public int wave;

    //public bool gameOver;

    public float enemies, kills;

	public Canvas canvas;
    public Text textTopLeft;
    public Text textTopRight;
    public Text textTopRight2;
    public Text textTopMid;
    public Text textMid;

    public AudioSource onWaveStart;
    public AudioSource music;
    public AudioSource onGameOver;

	public static void AddPlayer(GameObject go)
	{
		singleton.players.Add(go);
		EventListener.Get(go).OnDestroyDelegate += OnDestroyPlayer;
	}
	public static void AddMonster(GameObject go)
	{
		singleton.monsters.Add(go);
		EventListener.Get(go).OnDestroyDelegate += OnDestroyMonster;
	}
	public static void AddGoal(GameObject go)
	{
		singleton.goals.Add(go);
		EventListener.Get(go).OnDestroyDelegate += OnDestroyGoal;
	}

	static void OnDestroyPlayer(GameObject go)
	{
		singleton.players.Remove(go);
	}
	static void OnDestroyMonster(GameObject go)
	{
		singleton.monsters.Remove(go);
	}
	static void OnDestroyGoal(GameObject go)
	{
		singleton.goals.Remove(go);
	}


	void Awake()
	{
		if (singleton) { Destroy(this); return; }
		singleton = this;

		//quick and dirty way to connect this prefab instance to the UI in the scene 
		canvas = FindObjectOfType<Canvas>();
		if (canvas)
		{
			//Text[] texts = FindObjectsOfType<Text>();
			textTopLeft = GameObject.Find("Text_Health").GetComponent<Text>();
			textTopRight = GameObject.Find("Text_Kills").GetComponent<Text>();
			textTopRight2 = GameObject.Find("Text_Baddies").GetComponent<Text>();
			textTopMid = GameObject.Find("Text_TopCentre").GetComponent<Text>();
			textMid = GameObject.Find("Text_Centre").GetComponent<Text>();
		}
	}

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        spawners = GameObject.FindObjectsOfType<Spawner>();
        System.Array.Sort(spawners, delegate(Spawner a, Spawner b) { return a.name.CompareTo(b.name); });
        spawnScales = new float[spawners.Length];

        int i = 0;
        foreach (Spawner s in spawners)
        {
            s.gameObject.SetActive(false);
            s.OnSpawnDelegate += OnSpawnDelegate;
            spawnScales[i] = 1.0f / s.spawnTime;
            i++;
        }

        state = State.Wave;
        wave = 0;
        timer = waveTime;

        music.Play();

        StartCoroutine(Intro());
    }

    private void OnSpawnDelegate(GameObject g)
    {
        enemies++;
        EventListener.Get(g).OnDestroyDelegate += Enemy_OnDestroyDelegate;
        if (textTopRight2 != null)
            textTopRight2.text = string.Format("Enemies {0}", enemies);
    }

    private void Enemy_OnDestroyDelegate(GameObject g)
    {
        if (state == State.GameOver) return;    //too late, doesn't count

        enemies--;
        kills++;
        if (textTopRight!=null)
            textTopRight.text = string.Format("Kills {0}", kills);
        if (textTopRight2 != null)
            textTopRight2.text = string.Format("Enemies {0}", enemies);
    }

    void FixedUpdate ()
    {
        timer += Time.fixedDeltaTime;

        if (state != State.GameOver)
        {
            if (player != null)
            {
                Health h = player.GetComponent<Health>();
                if (h != null)
                {
                    if (textTopLeft) textTopLeft.text = string.Format("Health {0}", h.health);
                    if (h.health <= 0)
                    {
                        GameOver();
                    }
                } else
                {
                    GameOver();
                }
            } else
            {
                GameOver();
            }
        }

        switch (state)
        {
            case State.GameOver:
                if (timer > 3.0f
                    && (GamepadInput.GamePad.GetButton(GamepadInput.GamePad.Button.A, GamepadInput.GamePad.Index.Any)
                        || Input.GetKeyDown(KeyCode.Space)
                        )
                    )
                {
                    HardRestartGame();
                }
                break;

            case State.Wait:
				if (textTopMid) textTopMid.text = string.Format("incoming... {0:0}", Mathf.Clamp(waitTime - timer, 0, 999));
                if (timer > waitTime && enemies < 100)
                {
                    int i = 0;
                    foreach (Spawner s in spawners)
                    {
                        //s.enabled = true;
                        s.gameObject.SetActive((wave & (1+i)) != 0);    //bitmask to add some variety
                        s.spawnTime = (5.0f / (5.0f + wave)) / spawnScales[i];
                        //s.prefab = enemyTypes[wave % enemyTypes.Length];
                        i++;
                    }
                    state = State.Wave;
                    timer = 0;
					if (textTopMid) textTopMid.text = string.Format("Wave {0}", wave);
                    StartCoroutine(FlashText(string.Format("WAVE {0}", wave), 0.45f));
                    onWaveStart.pitch = 1.0f + 0.02f * wave;
                    onWaveStart.Play();
                    if (camCon && wave>1)
                        camCon.NextCamera();
                }
                break;

            case State.Wave:
                if (timer > waveTime || (timer > waveTime * 0.5f && enemies == 0))
                {
                    foreach (Spawner s in spawners)
                    {
                        s.gameObject.SetActive(false);
                    }
                    wave++;
                    state = State.Wait;
                    timer = 0;
                    if (textTopMid) textTopMid.text = string.Format("incoming...", wave);
                }
                break;
        }
    }

    void GameOver()
    {
        timer = 0;
        state = State.GameOver;
        music.Stop();
        onGameOver.Play();

		if (textTopLeft) textTopLeft.text = "DEAD";
        foreach (Spawner s in spawners)
            s.gameObject.SetActive(false);

        string playerName = System.Environment.UserName;
        System.DateTime dateTime = System.DateTime.Now;
//        if (leaderboard != null)
//        {
//            leaderboard.SetScore(playerName, kills);
//        }

        StartCoroutine(Outro());
    }

    IEnumerator Outro()
    {
        yield return StartCoroutine(FlashText("", 1.0f));
        yield return StartCoroutine(FlashText("GAME", 1.0f));
        yield return StartCoroutine(FlashText("OVER", 1.0f));
    }

    IEnumerator Intro()
    {
        yield return StartCoroutine(FlashText("", 0.1f));
/*        yield return StartCoroutine(FlashText("INFINITE", 0.6f));
        yield return StartCoroutine(FlashText("AMMO", 0.6f));
        yield return StartCoroutine(FlashText("", 0.6f));

        yield return StartCoroutine(FlashText("DON'T", 0.45f));
        yield return StartCoroutine(FlashText("SAVE", 0.45f));
        yield return StartCoroutine(FlashText("BULLETS", 0.45f));

        yield return StartCoroutine(FlashText("", 0.6f));

        yield return StartCoroutine(FlashText("USE", 0.45f));
        yield return StartCoroutine(FlashText("BOTH", 0.45f));
        yield return StartCoroutine(FlashText("HANDS", 0.45f));

        yield return StartCoroutine(FlashText("", 0.6f));

        yield return StartCoroutine(FlashText("GOOD", 0.45f));
        yield return StartCoroutine(FlashText("LUCK", 0.45f));
*/
        if (textMid) textMid.text = "";
    }

    public bool flashingText;   //simple semaphore
    IEnumerator FlashText(string text, float time = 1.0f)
    {
        while (flashingText)
            yield return new WaitForEndOfFrame();
        flashingText = true;
		if (textMid) textMid.text = text;
        for (float a = 1; a >= 0; a -= Time.deltaTime / time)
        {
			if (textMid) textMid.color = new Color(1, 1, 1, a);
            yield return new WaitForEndOfFrame();
        }
		if (textMid) textMid.color = Color.clear;
        flashingText = false;
    }

    void HardRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
