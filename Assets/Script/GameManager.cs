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
        //60프레임 맞추기
        Application.targetFrameRate = 60;

        //오브젝트, 이펙트 풀 리스트 초기화
        ballPool = new List<Ball>();
        effectPool = new List<ParticleSystem>();
        for (int index = 0; index < poolSize; index++)
        {
            MakeBall();
        }

        //최고기록 저장하기
        //기록이 없다면 0점으로 초기화
        if (!PlayerPrefs.HasKey("MaxScore2"))
        {
            PlayerPrefs.SetInt("MaxScore2", 0);
        }
        maxscoreText.text = "최고점수: " + PlayerPrefs.GetInt("MaxScore2").ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        NextBall();
    }

    Ball MakeBall()
    {
        //이펙트 생성(오브젝트 풀링)
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect" + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        //공 생성(오브젝트 풀링)
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
        //오브젝트 풀 탐색
        for (int index = 0; index < ballPool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % ballPool.Count;
            //그 위치의 오브젝트가 비활성화되어있다면
            if (!ballPool[poolCursor].gameObject.activeSelf)
            {
                //반환
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

        //랜덤 레벨의 공 생성
        lastBall = GetBall();
        lastBall.level = Random.Range(1, maxLevel);
        lastBall.gameObject.SetActive(true);

        SfxPlay(Sfx.Appear);
        StartCoroutine("WaitNext");
    }

    //놓고 나서 시간차 두기
    IEnumerator WaitNext()
    {
        //놓기 전까지는 다음 단계 진행 x
        while (lastBall != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.3f);
        NextBall();
    }

    //스크린을 터치할 때 
    public void TouchDown()
    {
        //누르는 중에는 공 드래그 함수 발동
        if (lastBall == null) return;
        lastBall.Drag();
    }
    public void TouchUp()
    {
        //누르다 떼면 공의 드랍 함수 발동
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

        //1. 장면 안에 활성화된 모든 오브젝트 가져오기
        Ball[] balls = FindObjectsOfType<Ball>();
        //2. 지우기 전에 모든 물리효과 비활성화(합체 방지)
        for (int index = 0; index < balls.Length; index++)
        {
            balls[index].rigid.simulated = false;
        }
        //3. 1번의 목록을 하나씩 접근해서 지우기
        for (int index = 0; index < balls.Length; index++)
        {
            balls[index].Hide(Vector3.up * 100);
            SfxPlay(Sfx.LevelUp);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        SfxPlay(Sfx.GameOver);
        //기록 갱신
        int maxScore2 = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore2"));
        PlayerPrefs.SetInt("MaxScore2", maxScore2);
        //게임오버 UI 표시
        gameoverUI.SetActive(true);
    }

    //UI버튼
    //일시정지
    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
        screen.SetActive(false);

    }
    //메인화면으로 돌아가기
    public void ToMenu()
    {
        SceneManager.LoadScene("MainScene");

    }
    //다시하기
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
    //계속하기
    public void Resume()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
        screen.SetActive(true);

    }

    void LateUpdate()
    {
        //점수 표시
        scoreText.text = "점수: " + score.ToString();
    }
    public void SfxPlay(Sfx type)
    {
        //상황에 따른 효과음 관리
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
