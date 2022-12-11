using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameManager manager;
    public ParticleSystem effect;
    public bool isDrag;
    public int level;
    public bool isMerge;


    public Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;
    SpriteRenderer spriteRenderer;

    float deadTime;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        //level�� �´� �̹����� �ҷ���
        anim.SetInteger("Level", level);
    }
    void OnDisable()
    {
        //��� �Ӽ� �ʱ�ȭ
        level = 0;
        isDrag = false;
        isMerge = false;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        circle.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        //�巡�� ������ ��
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //��ũ�� ��ǥ�� ���� ��ǥ��

            //x�� ��� ����(���� ȭ�� ������ ������ �ʰ�)
            float leftBorder = -4.0f + transform.localScale.x / 2f;
            float rightBorder = 4.0f - transform.localScale.x / 2f;
            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }

            //y��, z�� ����
            mousePos.y = 7;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.3f);
        }

    }
    //�巡�� ���¸� ��Ÿ���� �Լ�
    public void Drag()
    {
        isDrag = true;
    }
    //�巡�׸� ������ �Լ� - ���� ����ǰ�
    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }
    //������ ������ ��
    void OnCollisionStay2D(Collision2D collision)
    {
        //�� ������Ʈ�� �ε��� �� - �ݶ��̴�, �±� �̿� 
        if (collision.gameObject.tag == "Ball")
        {
            Ball other = collision.gameObject.GetComponent<Ball>();

            //������ ������ ��ġ��
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // 1. ���� �Ʒ� ���� ��
                // 2. ������ ������ ��, ���� �������� ��
                if (meY < otherY || (meY == otherY && meX > otherX))
                {
                    //��� �����
                    other.Hide(transform.position);
                    //���� ������
                    LevelUp();
                }

            }
        }
    }
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        //����ȿ�� ��Ȱ��ȭ
        rigid.simulated = false;
        circle.enabled = false;

        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        StartCoroutine(HideRoutine(targetPos));
    }
    //��뿡�� �̵��ϸ� �����
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        //frameCount�� ���� Updateó�� Ȱ��
        int frameCount = 0;
        while (frameCount < 20)
        {
            frameCount++;
            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            }
            else if (targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }
            yield return null;
        }
        //���� �߰�
        manager.score += (int)Mathf.Pow(2, level);

        isMerge = false;
        gameObject.SetActive(false);
    }
    void LevelUp()
    {
        isMerge = true;

        //�浹, ����� ���ص��� �ʵ��� �����ӵ� ����
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());
    }
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.08f);

        //�����ð� ������ �� ���� �ø���
        anim.SetInteger("Level", level + 1);    //�ִϸ��̼��� ���� ū �̹����� ��ȭ
        EffectPlay();
        manager.SfxPlay(GameManager.Sfx.LevelUp);

        yield return new WaitForSeconds(0.08f);
        level++;

        //ū ���� ������ ���� �����Ǵ� ���� ū ���� �ǵ��� maxLevel ������Ʈ
        //������ �ƽ��������� ũ�� �ƽ������� ���� ������ ����
        manager.maxLevel = Mathf.Max(level, manager.maxLevel);

        isMerge = false;
    }
    void EffectPlay()
    {
        effect.transform.position = transform.position; //����Ʈ ��ġ = ���� ��ġ
        effect.transform.localScale = transform.localScale; //ũ�� ����
        effect.Play();
    }
    //��輱�� ���� �� ���ӿ��� ó��
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            //��輱�� ��� �ð� üũ
            deadTime += Time.deltaTime;

            if (deadTime > 2)
            {
                //deadTime�� 2���� Ŀ���� ���� ���������� ǥ��(���)
                spriteRenderer.color = new Color(0.9f, 0.2f, 0.2f);
            }
            if (deadTime > 5)
            {
                //deadTime�� 5���� Ŀ���� ���ӿ��� ó��
                manager.GameOver();
            }
        }
    }
    //��輱���� ����� deadTime �ʱ�ȭ, �� ������ ������� ������
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }
}

