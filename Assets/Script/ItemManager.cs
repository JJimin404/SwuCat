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
        //60������ ���߱�
        Application.targetFrameRate = 60;

        //������ Ǯ ����Ʈ �ʱ�ȭ
        itemPool = new List<Item>();
        for (int index = 0; index < poolSize; index++)
        {
            MakeItem();
        }

        //�ְ��� �����ϱ�
        //����� ���ٸ� 0������ �ʱ�ȭ
        if (!PlayerPrefs.HasKey("MaxScore1"))
        {
            PlayerPrefs.SetInt("MaxScore1", 0);
        }
        maxscoreText.text = "�ְ�����: " + PlayerPrefs.GetInt("MaxScore1").ToString();
        anim = textBox.GetComponent<Animator>();

        //���� �� �ؽ�Ʈ �ִϸ��̼� ����
        StartCoroutine("startRoutine");

}
    IEnumerator startRoutine()
    {
        yield return null;
        //ready, start!
        //�ִϸ��̼����� �ؽ�Ʈ �̹��� ǥ��
        anim.SetInteger("Text", 1);
        yield return new WaitForSeconds(1.5f);
        anim.SetInteger("Text", 2);
        yield return new WaitForSeconds(0.5f);

    }  
    void Start()
    {
        //���� �ʱ�ȭ
        score = 0;
        time = 0;
        remainTime = 30.0f;
        
        //������ �ҷ�����
        NextItem();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (remainTime <= 0)
        {
            //���� �ð��� 0���� �۾����� ���ӿ���
            remainTime = 0;
            GameOver();
        }
        //Ÿ�̸� ����(���� �ð� = 30�� - ���� �ð�)
        else
        {
            time += Time.deltaTime;
            timeText.text = "���� �ð�: " + 30;
            //����, ��ŸƮ �ߴ� ������ 2��
            if (time > 2)
            {
                remainTime = Mathf.Ceil(32.0f - time);
                timeText.text = "���� �ð�: " + remainTime.ToString();
            }
            
        }

        scoreText.text = "����: " + score.ToString();
    }

    //������ ����
    Item MakeItem()
    {
        //������ �׷쿡 ������ ������ ����
        GameObject instantItemObj = Instantiate(itemPrefab, itemGroup);
        //������Ʈ Ǯ��
        instantItemObj.name = "Item" + itemPool.Count;
        Item instantItem = instantItemObj.GetComponent<Item>();
        itemPool.Add(instantItem);

        return instantItem;
    }
    Item GetItem()
    {
        //������Ʈ Ǯ Ž��
        for (int index = 0; index < itemPool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % itemPool.Count;
            //�� ��ġ�� ������Ʈ�� ��Ȱ��ȭ�Ǿ��ִٸ�
            if (!itemPool[poolCursor].gameObject.activeSelf)
            {
                //��ȯ
                return itemPool[poolCursor];
            }
        }
        return MakeItem();
    }
    void NextItem()
    {
        //���� ������ ����(����)
        if (isOver) return;
        lastItem = GetItem();
        lastItem.type = Random.Range(1, 7);
        itemType = lastItem.type;
        lastItem.gameObject.SetActive(true);
        StartCoroutine("WaitNext");
    }
    IEnumerator WaitNext()
    {
        //���� ������ �������� ������
        while (lastItem != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        NextItem();
    }

    //�����ϴ°� ��ư�� ������ ��
    public void LikeBtn()
    {
        //�̹����� 1,2,3(�����ϴ°�)�߿� �ϳ��̸�
        if (itemType == 1 || itemType == 2 || itemType == 3)
        {
            //���� ��ƾ �ߵ�, +10��
            StartCoroutine("CorrectRoutine");
            score += 10;
        }
        //�̹����� 4,5,6(�Ⱦ��ϴ°�)�߿� �ϳ��̸�
        if (itemType == 4 || itemType == 5 || itemType == 6)
        {
            //���� ��ƾ �ߵ�, -5��
            StartCoroutine("WrongRoutine");
            score -= 5;
        }
        //ä���� ������ ������ ����� ������Ʈ �ڸ� ����
        lastItem.Hide();
        lastItem = null;
    }
    //�Ⱦ��ϴ°� ��ư�� ������ ��
    public void DislikeBtn()
    {
        //�̹����� 1,2,3(�����ϴ°�)�߿� �ϳ��̸�
        if (itemType == 1 || itemType == 2 || itemType == 3)
        {
            //���� ��ƾ �ߵ�, -5��
            StartCoroutine("WrongRoutine");
            score -= 5;
        }
        //�̹����� 4,5,6(�Ⱦ��ϴ°�)�߿� �ϳ��̸�
        if (itemType == 4 || itemType == 5 || itemType == 6)
        {
            //���� ��ƾ �ߵ�, +10��
            StartCoroutine("CorrectRoutine");
            score += 10;
        }
        //ä���� ������ ������ ����� ������Ʈ �ڸ� ����
        lastItem.Hide();
        lastItem = null;
    }
    IEnumerator CorrectRoutine()
    {
        //���� ��ƾ: O �̹��� ǥ��, ���� ȿ���� �ߵ�
        sfxO.Play();
        yield return null;
        correct.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        correct.SetActive(false);
    }
    IEnumerator WrongRoutine()
    {
        //���� ��ƾ: X �̹��� ǥ��, Ʋ�� ȿ���� �ߵ�
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

        //���ӿ��� �� ��ƾ ����
        StartCoroutine("GameOverRoutine");

    }
    IEnumerator GameOverRoutine()
    {
        bgm.Stop();
        sfxOver.Play();
        //1�� ��ٸ���
        yield return new WaitForSeconds(0.5f);

        //�ְ����� ����
        int maxScore1 = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore1"));
        PlayerPrefs.SetInt("MaxScore1", maxScore1);
        //���ӿ��� UI ǥ��
        gameoverUI.SetActive(true);
    }

    //UI��ư
    //�Ͻ�����
    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(true);
        screen.SetActive(true);
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
        SceneManager.LoadScene("Game1");
    }
    //����ϱ�
    public void Resume()
    {
        Time.timeScale = 1;
        pause.SetActive(false);
        screen.SetActive(false);
    }
}
