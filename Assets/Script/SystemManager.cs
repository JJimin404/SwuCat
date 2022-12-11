using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    [Header("[ variable ]")]
    public float score;
    public float time;
    public float playerHp;  //남은체력
    public float lostHp;    //잃은체력
    public bool isOver;
    public float speed;
    public float maxSpawnDelay; //딜레이된 시간의 최댓값(스폰 쿨타임)
    public float curSpawnDelay; //현재 딜레이된 시간

    [Header("[ Object ]")]
    public GameObject textBox;
    public GameObject hpGauge;
    public GameObject itemPrefab;
    public Transform[] spawnPoints;
    public AudioSource bgm;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;

    public enum Sfx { Hit, GameOver, Heart }
    int sfxCursor;
    Animator anim;

    [Header("[ UI ]")]
    public GameObject gameoverUI;
    public Text scoreText;
    public Text maxscoreText;
    //시간에 따라 스피드, 체력 빠르게 하기

    void Awake()
    {
        bgm.Play();
        //최고기록 저장하기
        //기록이 없다면 0점으로 초기화
        if (!PlayerPrefs.HasKey("MaxScore3"))
        {
            PlayerPrefs.SetInt("MaxScore3", 0);
        }
        maxscoreText.text = "기록: " + PlayerPrefs.GetFloat("MaxScore3").ToString() + "m";
        anim = textBox.GetComponent<Animator>();
        //시작 전 텍스트 애니메이션 실행
        StartCoroutine("startRoutine");
    }
    IEnumerator startRoutine()
    {
        yield return null;
        //ready, start!
        anim.SetInteger("Text", 1);
        yield return new WaitForSeconds(1.5f);
        anim.SetInteger("Text", 2);
        yield return new WaitForSeconds(0.5f);

    }
// Start is called before the first frame update    
    void Start()
    {
        speed = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHp <= 0)
        {
            //남은 체력이 0보다 작아지면 게임오버
            playerHp = 0;
            GameOver();
        }
        //체력이 양수일때
        else
        {
            time += Time.deltaTime;

            //레디, 스타트 뜨는 딜레이 2초
            if (time > 2)
            {
                //거리(점수) = 시간 * 맵 이동속도 (-3은 딜레이 고려하여 보정)
                score = Mathf.Ceil(time * speed * 0.5f) - 3;

                //체력표시
                playerHp = Mathf.Ceil(102.0f - time) - lostHp;
                hpGauge.GetComponent<Image>().fillAmount = playerHp * 0.01f;

                //남은 체력에 따라 맵, 장애물 속도 변화
                if (playerHp > 60)
                {
                    speed = 3;
                }
                else if (playerHp > 50)
                {
                    speed = 3.5f;
                }
                //체력이 절반 이하일 때 10~15초에 한번씩 체력 아이템 생성
                else
                {
                    curSpawnDelay += Time.deltaTime;

                    if (curSpawnDelay > maxSpawnDelay)
                    {
                        Spawnitem();
                        maxSpawnDelay = Random.Range(10f, 15f);
                        curSpawnDelay = 0;
                    }
                    //체력이 더 줄어들면 스피드 up
                    if (playerHp > 40)
                    {
                        speed = 4;
                    }
                    else if (playerHp > 30)
                    {
                        speed = 4.5f;
                    }
                    else
                    {
                        speed = 5;
                    }
                }
            }
        }
        scoreText.text = "현재: " + score.ToString() + "m";
    }
    //장애물 충돌 시 hp 감소
    public void DecreaseHP()
    {
        lostHp += 10;
        hpGauge.GetComponent<Image>().fillAmount -= 0.1f;

    }
    //하트 획득 시 hp 증가
    public void increaseHP()
    {
        lostHp -= 5;
        hpGauge.GetComponent<Image>().fillAmount += 0.05f;

    }
    //장애물, 아이템을 랜덤 스폰
    void Spawnitem()
    {
        int ranPoint = Random.Range(0, 3);  //장애물 위치
        Instantiate(itemPrefab, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
    }
    public void GameOver()
    {
        if (isOver) return;
        isOver = true;

        //게임오버 시 루틴 시행
        StartCoroutine("GameOverRoutine");

    }
    IEnumerator GameOverRoutine()
    {
        bgm.Stop();
        sfxPlay(Sfx.GameOver);
        //1초 기다리기
        yield return new WaitForSeconds(0.5f);
        //맵, 장애물 멈추기
        speed = 0;
        //최고점수 갱신
        float maxScore3 = Mathf.Max(score, PlayerPrefs.GetFloat("MaxScore3"));
        PlayerPrefs.SetFloat("MaxScore3", maxScore3);
        //게임오버 UI 표시
        gameoverUI.SetActive(true);
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
        SceneManager.LoadScene("Game3");
    }
    public void sfxPlay(Sfx type)
    {
        //상황에 따른 효과음 관리
        switch (type)
        {
            case Sfx.Hit:
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            case Sfx.GameOver:
                sfxPlayer[sfxCursor].clip = sfxClip[1];
                break;
            case Sfx.Heart:
                sfxPlayer[sfxCursor].clip = sfxClip[2];
                break;
        }
        sfxPlayer[sfxCursor].Play();
    }
}
