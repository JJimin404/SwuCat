using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsController : MonoBehaviour
{
    public float obsSpeed;
    public Sprite[] img;

    GameObject director;
    SpriteRenderer spriteRenderer;
    BoxCollider2D box;
    void Awake()
    {
        director = GameObject.Find("SystemManager");
    }   
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        //랜덤한 이미지 생성
        spriteRenderer.sprite = img[Random.Range(0,img.Length)];

    }
    // Update is called once per frame
    void Update()
    {
        //생성되면 배경과 같은 속도로 내려오기
        obsSpeed = director.GetComponent<SystemManager>().speed;
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * obsSpeed * Time.deltaTime;
        transform.position = curPos + nextPos;

        //화면 밖으로 벗어나면 삭제
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

}
