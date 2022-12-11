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
        //생성되면 배경과 같은 속도로 내려오기
        itemSpeed = director.GetComponent<SystemManager>().speed;
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * itemSpeed * Time.deltaTime;
        transform.position = curPos + nextPos;

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어와 충돌하면 삭제하기
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
    
}
