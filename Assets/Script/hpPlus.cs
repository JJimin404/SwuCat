using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpPlus : MonoBehaviour
{
    public float itemSpeed;
    public ParticleSystem effect;

    GameObject director;
    BoxCollider2D box;
    void Awake()
    {
        director = GameObject.Find("SystemManager");
    }
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //�����Ǹ� ���� ���� �ӵ��� ��������
        itemSpeed = director.GetComponent<SystemManager>().speed;
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * itemSpeed * Time.deltaTime;
        transform.position = curPos + nextPos;

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //�÷��̾�� �浹�ϸ� �����ϱ�
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
    
}
