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
        //level에 맞는 이미지를 불러옴
        anim.SetInteger("Level", level);
    }
    void OnDisable()
    {
        //모든 속성 초기화
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
        //드래그 상태일 때
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //스크린 좌표를 월드 좌표로

            //x축 경계 설정(공을 화면 밖으로 나가지 않게)
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

            //y축, z축 고정
            mousePos.y = 7;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.3f);
        }

    }
    //드래그 상태를 나타내는 함수
    public void Drag()
    {
        isDrag = true;
    }
    //드래그를 끝내는 함수 - 물리 적용되게
    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }
    //공끼리 합쳐질 때
    void OnCollisionStay2D(Collision2D collision)
    {
        //공 오브젝트와 부딪힐 때 - 콜라이더, 태그 이용 
        if (collision.gameObject.tag == "Ball")
        {
            Ball other = collision.gameObject.GetComponent<Ball>();

            //레벨이 같으면 합치기
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // 1. 내가 아래 있을 때
                // 2. 동일한 높이일 때, 내가 오른쪽일 때
                if (meY < otherY || (meY == otherY && meX > otherX))
                {
                    //상대 숨기기
                    other.Hide(transform.position);
                    //나는 레벨업
                    LevelUp();
                }

            }
        }
    }
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        //물리효과 비활성화
        rigid.simulated = false;
        circle.enabled = false;

        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        StartCoroutine(HideRoutine(targetPos));
    }
    //상대에게 이동하며 숨기기
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        //frameCount를 통해 Update처럼 활용
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
        //점수 추가
        manager.score += (int)Mathf.Pow(2, level);

        isMerge = false;
        gameObject.SetActive(false);
    }
    void LevelUp()
    {
        isMerge = true;

        //충돌, 흡수에 방해되지 않도록 물리속도 제거
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());
    }
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.08f);

        //일정시간 딜레이 후 레벨 올리기
        anim.SetInteger("Level", level + 1);    //애니메이션을 통해 큰 이미지로 변화
        EffectPlay();
        manager.SfxPlay(GameManager.Sfx.LevelUp);

        yield return new WaitForSeconds(0.08f);
        level++;

        //큰 공이 나오면 새로 생성되는 공도 큰 공이 되도록 maxLevel 업데이트
        //레벨이 맥스레벨보다 크면 맥스레벨에 현재 레벨을 대입
        manager.maxLevel = Mathf.Max(level, manager.maxLevel);

        isMerge = false;
    }
    void EffectPlay()
    {
        effect.transform.position = transform.position; //이펙트 위치 = 나의 위치
        effect.transform.localScale = transform.localScale; //크기 조정
        effect.Play();
    }
    //경계선에 닿을 때 게임오버 처리
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            //경계선에 닿는 시간 체크
            deadTime += Time.deltaTime;

            if (deadTime > 2)
            {
                //deadTime이 2보다 커지면 공을 빨간색으로 표시(경고)
                spriteRenderer.color = new Color(0.9f, 0.2f, 0.2f);
            }
            if (deadTime > 5)
            {
                //deadTime이 5보다 커지면 게임오버 처리
                manager.GameOver();
            }
        }
    }
    //경계선에서 벗어나면 deadTime 초기화, 공 색깔은 원래대로 돌리기
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }
}

