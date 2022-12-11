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
    public float playerHp;  //����ü��
    public float lostHp;    //����ü��
    public bool isOver;
    public float speed;
    public float maxSpawnDelay; //�����̵� �ð��� �ִ�(���� ��Ÿ��)
    public float curSpawnDelay; //���� �����̵� �ð�

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
    //�ð��� ���� ���ǵ�, ü�� ������ �ϱ�

    void Awake()
    {
        bgm.Play();
        //�ְ��� �����ϱ�
        //����� ���ٸ� 0������ �ʱ�ȭ
        if (!PlayerPrefs.HasKey("MaxScore3"))
        {
            PlayerPrefs.SetInt("MaxScore3", 0);
        }
        maxscoreText.text = "���: " + PlayerPrefs.GetFloat("MaxScore3").ToString() + "m";
        anim = textBox.GetComponent<Animator>();
        //���� �� �ؽ�Ʈ �ִϸ��̼� ����
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
            //���� ü���� 0���� �۾����� ���ӿ���
            playerHp = 0;
            GameOver();
        }
        //ü���� ����϶�
        else
        {
            time += Time.deltaTime;

            //����, ��ŸƮ �ߴ� ������ 2��
            if (time > 2)
            {
                //�Ÿ�(����) = �ð� * �� �̵��ӵ� (-3�� ������ ����Ͽ� ����)
                score = Mathf.Ceil(time * speed * 0.5f) - 3;

                //ü��ǥ��
                playerHp = Mathf.Ceil(102.0f - time) - lostHp;
                hpGauge.GetComponent<Image>().fillAmount = playerHp * 0.01f;

                //���� ü�¿� ���� ��, ��ֹ� �ӵ� ��ȭ
                if (playerHp > 60)
                {
                    speed = 3;
                }
                else if (playerHp > 50)
                {
                    speed = 3.5f;
                }
                //ü���� ���� ������ �� 10~15�ʿ� �ѹ��� ü�� ������ ����
                else
                {
                    curSpawnDelay += Time.deltaTime;

                    if (curSpawnDelay > maxSpawnDelay)
                    {
                        Spawnitem();
                        maxSpawnDelay = Random.Range(10f, 15f);
                        curSpawnDelay = 0;
                    }
                    //ü���� �� �پ��� ���ǵ� up
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
        scoreText.text = "����: " + score.ToString() + "m";
    }
    //��ֹ� �浹 �� hp ����
    public void DecreaseHP()
    {
        lostHp += 10;
        hpGauge.GetComponent<Image>().fillAmount -= 0.1f;

    }
    //��Ʈ ȹ�� �� hp ����
    public void increaseHP()
    {
        lostHp -= 5;
        hpGauge.GetComponent<Image>().fillAmount += 0.05f;

    }
    //��ֹ�, �������� ���� ����
    void Spawnitem()
    {
        int ranPoint = Random.Range(0, 3);  //��ֹ� ��ġ
        Instantiate(itemPrefab, spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
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
        sfxPlay(Sfx.GameOver);
        //1�� ��ٸ���
        yield return new WaitForSeconds(0.5f);
        //��, ��ֹ� ���߱�
        speed = 0;
        //�ְ����� ����
        float maxScore3 = Mathf.Max(score, PlayerPrefs.GetFloat("MaxScore3"));
        PlayerPrefs.SetFloat("MaxScore3", maxScore3);
        //���ӿ��� UI ǥ��
        gameoverUI.SetActive(true);
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
        SceneManager.LoadScene("Game3");
    }
    public void sfxPlay(Sfx type)
    {
        //��Ȳ�� ���� ȿ���� ����
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
