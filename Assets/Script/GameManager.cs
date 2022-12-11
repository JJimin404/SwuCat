using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("[ variable ]")]
    public bool isOver;
    public int score;
    public int maxLevel;

    [Header("[ Object ]")]
    public GameObject ballPrefab;
    public Transform ballGroup;
    public List<Ball> ballPool;
    public GameObject effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;

    public AudioSource bgm;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;

    public enum Sfx { Appear, Btn, Exit, GameOver, LevelUp, StartBtn }
    int sfxCursor;

    [Range(1, 10)]
    public int poolSize;
    public int poolCursor;
    public Ball lastBall;

    [Header("[ UI ]")]
    public GameObject gameoverUI;
    public Text scoreText;
    public Text maxscoreText;
    public GameObject pause;
    public GameObject screen;



    void Awake()
    {
        bgm.Play();
        //60������ ���߱�
        Application.targetFrameRate = 60;

        //������Ʈ, ����Ʈ Ǯ ����Ʈ �ʱ�ȭ
        ballPool = new List<Ball>();
        effectPool = new List<ParticleSystem>();
        for (int index = 0; index < poolSize; index++)
        {
            MakeBall();
        }

        //�ְ��� �����ϱ�
        //����� ���ٸ� 0������ �ʱ�ȭ
        if (!PlayerPrefs.HasKey("MaxScore2"))
        {
            PlayerPrefs.SetInt("MaxScore2", 0);
        }
        maxscoreText.text = "�ְ�����: " + PlayerPrefs.GetInt("MaxScore2").ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        NextBall();
    }

    Ball MakeBall()
    {
        //����Ʈ ����(������Ʈ Ǯ��)
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect" + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        //�� ����(������Ʈ Ǯ��)
        GameObject instantBallObj = Instantiate(ballPrefab, ballGroup);
        instantBallObj.name = "Ball" + ballPool.Count;
        Ball instantBall = instantBallObj.GetComponent<Ball>();
        instantBall.manager = this;
        instantBall.effect = instantEffect;
        ballPool.Add(instantBall);

        return instantBall;
    }

    Ball GetBall()
    {
        //������Ʈ Ǯ Ž��
        for (int index = 0; index < ballPool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % ballPool.Count;
            //�� ��ġ�� ������Ʈ�� ��Ȱ��ȭ�Ǿ��ִٸ�
            if (!ballPool[poolCursor].gameObject.activeSelf)
            {
                //��ȯ
                return ballPool[poolCursor];
            }
        }
        return MakeBall();
    }
    void NextBall()
    {
        if (isOver)
        {
            return;
        }

        //���� ������ �� ����
        lastBall = GetBall();
        lastBall.level = Random.Range(1, maxLevel);
        lastBall.gameObject.SetActive(true);

        SfxPlay(Sfx.Appear);
        StartCoroutine("WaitNext");
    }

    //���� ���� �ð��� �α�
    IEnumerator WaitNext()
    {
        //���� �������� ���� �ܰ� ���� x
        while (lastBall != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.3f);
        NextBall();
    }

    //��ũ���� ��ġ�� �� 
    public void TouchDown()
    {
        //������ �߿��� �� �巡�� �Լ� �ߵ�
        if (lastBall == null) return;
        lastBall.Drag();
    }
    public void TouchUp()
    {
        //������ ���� ���� ��� �Լ� �ߵ�
        if (lastBall == null) return;
        lastBall.Drop();
        lastBall = null;
    }
    public void GameOver()
    {
        if (isOver)
        {
            return;
        }

        isOver = true;

        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine()
    {
        bgm.Stop();

        //1. ��� �ȿ� Ȱ��ȭ�� ��� ������Ʈ ��������
        Ball[] balls = FindObjectsOfType<Ball>();
        //2. ����� ���� ��� ����ȿ�� ��Ȱ��ȭ(��ü ����)
        for (int index = 0; index < balls.Length; index++)
        {
            balls[index].rigid.simulated = false;
        }
        //3. 1���� ����� �ϳ��� �����ؼ� �����
        for (int index = 0; index < balls.Length; index++)
        {
            balls[index].Hide(Vector3.up * 100);
            SfxPlay(Sfx.LevelUp);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        SfxPlay(Sfx.GameOver);
        //��� ����
        int maxScore2 = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore2"));
        PlayerPrefs.SetInt("MaxScore2", maxScore2);
        //���ӿ��� UI ǥ��
        gameoverUI.SetActive(true);
    }

    //UI��ư
    //�Ͻ�����
    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
        screen.SetActive(false);

    }
    //����ȭ������ ���ư���
    public void ToMenu()
    {
        SceneManager.LoadScene("MainScene");

    }
    //�ٽ��ϱ�
    public void Restart()
    {
        Time.timeScale = 1;
        StartCoroutine("RestartCoroutine");
    }
    IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Game2");
    }
    //����ϱ�
    public void Resume()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
        screen.SetActive(true);

    }

    void LateUpdate()
    {
        //���� ǥ��
        scoreText.text = "����: " + score.ToString();
    }
    public void SfxPlay(Sfx type)
    {
        //��Ȳ�� ���� ȿ���� ����
        switch (type)
        {
            case Sfx.Appear:
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            case Sfx.GameOver:
                sfxPlayer[sfxCursor].clip = sfxClip[1];
                break;
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[2];
                break;
        }
        sfxPlayer[sfxCursor].Play();

    }
}
