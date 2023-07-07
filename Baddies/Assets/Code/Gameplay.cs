using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Gameplay : MonoBehaviour
{
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

    public UnityEngine.UI.Text textTopLeft;
    public UnityEngine.UI.Text textTopRight;
    public UnityEngine.UI.Text textTopRight2;
    public UnityEngine.UI.Text textTopMid;
    public UnityEngine.UI.Text textMid;

    public AudioSource onWaveStart;
    public AudioSource music;
    public AudioSource onGameOver;

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
                    textTopLeft.text = string.Format("Health {0}", h.health);
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
                textTopMid.text = string.Format("incoming... {0:0}", Mathf.Clamp(waitTime - timer, 0, 999));
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
                    textTopMid.text = string.Format("Wave {0}", wave);
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
                    textTopMid.text = string.Format("incoming...", wave);
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

        textTopLeft.text = "DEAD";
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
        yield return StartCoroutine(FlashText("INFINITE", 0.6f));
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

        /*
        textMid.text = "INFINITE";
        for (float a = 1; a >= 0; a -= Time.deltaTime / 1.0f)
        {
            textMid.color = new Color(1, 1, 1, a);
            yield return new WaitForEndOfFrame();
        }

        textMid.text = "AMMO";
        for (float a = 1; a >= 0; a -= Time.deltaTime / 1.0f)
        {
            textMid.color = new Color(1, 1, 1, a);
            yield return new WaitForEndOfFrame();
        }
        */
        textMid.text = "";
    }

    public bool flashingText;   //simple semaphore
    IEnumerator FlashText(string text, float time = 1.0f)
    {
        while (flashingText)
            yield return new WaitForEndOfFrame();
        flashingText = true;
        textMid.text = text;
        for (float a = 1; a >= 0; a -= Time.deltaTime / time)
        {
            textMid.color = new Color(1, 1, 1, a);
            yield return new WaitForEndOfFrame();
        }
        textMid.color = Color.clear;
        flashingText = false;
    }

    void HardRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
