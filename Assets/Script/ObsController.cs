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
        //������ �̹��� ����
        spriteRenderer.sprite = img[Random.Range(0,img.Length)];

    }
    // Update is called once per frame
    void Update()
    {
        //�����Ǹ� ���� ���� �ӵ��� ��������
        obsSpeed = director.GetComponent<SystemManager>().speed;
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * obsSpeed * Time.deltaTime;
        transform.position = curPos + nextPos;

        //ȭ�� ������ ����� ����
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

}
