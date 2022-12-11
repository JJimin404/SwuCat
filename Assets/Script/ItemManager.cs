using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    [Header("[ variable ]")]
    public int itemType;
    public int score;
    public float time;
    public float remainTime;
    public bool isOver;

    [Header("[ Object ]")]
    public GameObject itemPrefab;
    public GameObject correct;
    public GameObject wrong;
    public Transform itemGroup;
    public List<Item> itemPool;
    public GameObject textBox;
    public AudioSource bgm;
    public AudioSource sfxO;
    public AudioSource sfxX;
    public AudioSource sfxOver;
    Animator anim;


    [Range(1, 10)]
    public int poolSize;
    public int poolCursor;
    public Item lastItem;

    [Header("[ UI ]")]
    public GameObject gameoverUI;
    public Text scoreText;
    public Text maxscoreText;
    public Text timeText;
    public GameObject pause;
    public GameObject screen;
    


    // Start is called before the first frame update

    void Awake()
    {
        bgm.Play();
        //60프레임 맞추기
        Application.targetFrameRate = 60;

        //아이템 풀 리스트 초기화
        itemPool = new List<Item>();
        for (int index = 0; index < poolSize; index++)
        {
            MakeItem();
        }

        //최고기록 저장하기
        //기록이 없다면 0점으로 초기화
        if (!PlayerPrefs.HasKey("MaxScore1"))
        {
            PlayerPrefs.SetInt("MaxScore1", 0);
        }
        maxscoreText.text = "최고점수: " + PlayerPrefs.GetInt("MaxScore1").ToString();
        anim = textBox.GetComponent<Animator>();

        //시작 전 텍스트 애니메이션 실행
        StartCoroutine("startRoutine");

}
    IEnumerator startRoutine()
    {
        yield return null;
        //ready, start!
        //애니메이션으로 텍스트 이미지 표시
        anim.SetInteger("Text", 1);
        yield return new WaitForSeconds(1.5f);
        anim.SetInteger("Text", 2);
        yield return new WaitForSeconds(0.5f);

    }  
    void Start()
    {
        //점수 초기화
        score = 0;
        time = 0;
        remainTime = 30.0f;
        
        //아이템 불러오기
        NextItem();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (remainTime <= 0)
        {
            //남은 시간이 0보다 작아지면 게임오버
            remainTime = 0;
            GameOver();
        }
        //타이머 설정(남은 시간 = 30초 - 지난 시간)
        else
        {
            time += Time.deltaTime;
            timeText.text = "남은 시간: " + 30;
            //레디, 스타트 뜨는 딜레이 2초
            if (time > 2)
            {
                remainTime = Mathf.Ceil(32.0f - time);
                timeText.text = "남은 시간: " + remainTime.ToString();
            }
            
        }

        scoreText.text = "점수: " + score.ToString();
    }

    //아이템 생성
    Item MakeItem()
    {
        //아이템 그룹에 아이템 프리팹 생성
        GameObject instantItemObj = Instantiate(itemPrefab, itemGroup);
        //오브젝트 풀링
        instantItemObj.name = "Item" + itemPool.Count;
        Item instantItem = instantItemObj.GetComponent<Item>();
        itemPool.Add(instantItem);

        return instantItem;
    }
    Item GetItem()
    {
        //오브젝트 풀 탐색
        for (int index = 0; index < itemPool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % itemPool.Count;
            //그 위치의 오브젝트가 비활성화되어있다면
            if (!itemPool[poolCursor].gameObject.activeSelf)
            {
                //반환
                return itemPool[poolCursor];
            }
        }
        return MakeItem();
    }
    void NextItem()
    {
        //다음 아이템 생성(랜덤)
        if (isOver) return;
        lastItem = GetItem();
        lastItem.type = Random.Range(1, 7);
        itemType = lastItem.type;
        lastItem.gameObject.SetActive(true);
        StartCoroutine("WaitNext");
    }
    IEnumerator WaitNext()
    {
        //다음 아이템 생성까지 딜레이
        while (lastItem != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        NextItem();
    }

    //좋아하는것 버튼을 눌렀을 때
    public void LikeBtn()
    {
        //이미지가 1,2,3(좋아하는것)중에 하나이면
        if (itemType == 1 || itemType == 2 || itemType == 3)
        {
            //정답 루틴 발동, +10점
            StartCoroutine("CorrectRoutine");
            score += 10;
        }
        //이미지가 4,5,6(싫어하는것)중에 하나이면
        if (itemType == 4 || itemType == 5 || itemType == 6)
        {
            //오답 루틴 발동, -5점
            StartCoroutine("WrongRoutine");
            score -= 5;
        }
        //채점이 끝나면 아이템 숨기고 오브젝트 자리 비우기
        lastItem.Hide();
        lastItem = null;
    }
    //싫어하는것 버튼을 눌렀을 때
    public void DislikeBtn()
    {
        //이미지가 1,2,3(좋아하는것)중에 하나이면
        if (itemType == 1 || itemType == 2 || itemType == 3)
        {
            //오답 루틴 발동, -5점
            StartCoroutine("WrongRoutine");
            score -= 5;
        }
        //이미지가 4,5,6(싫어하는것)중에 하나이면
        if (itemType == 4 || itemType == 5 || itemType == 6)
        {
            //정답 루틴 발동, +10점
            StartCoroutine("CorrectRoutine");
            score += 10;
        }
        //채점이 끝나면 아이템 숨기고 오브젝트 자리 비우기
        lastItem.Hide();
        lastItem = null;
    }
    IEnumerator CorrectRoutine()
    {
        //정답 루틴: O 이미지 표시, 맞음 효과음 발동
        sfxO.Play();
        yield return null;
        correct.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        correct.SetActive(false);
    }
    IEnumerator WrongRoutine()
    {
        //정답 루틴: X 이미지 표시, 틀림 효과음 발동
        sfxX.Play();
        yield return null;
        wrong.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        wrong.SetActive(false);
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
        sfxOver.Play();
        //1초 기다리기
        yield return new WaitForSeconds(0.5f);

        //최고점수 갱신
        int maxScore1 = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore1"));
        PlayerPrefs.SetInt("MaxScore1", maxScore1);
        //게임오버 UI 표시
        gameoverUI.SetActive(true);
    }

    //UI버튼
    //일시정지
    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
        screen.SetActive(true);
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
        SceneManager.LoadScene("Game1");
    }
    //계속하기
    public void Resume()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
        screen.SetActive(false);
    }
}
