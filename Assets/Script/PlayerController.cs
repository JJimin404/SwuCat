using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    CapsuleCollider2D capsule;
    Animation anim;
    Transform playerPos;
    SpriteRenderer spriteRenderer;
    public SystemManager director;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animation>();
        playerPos = GetComponent<Transform>();
        playerPos.position = new Vector3(0, -3, 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    //왼쪽으로 한칸 움직이기
    public void moveLeft()
    {
        //맨 왼쪽에 있을 때 이동 x
        if (playerPos.position.x == -1) return;
        else
        {
            playerPos.Translate(-1, 0, 0);
        }
        
    }
    //오른쪽으로 한칸 움직이기
    public void moveRight()
    {
        //맨 오른쪽에 있을 때 이동 x
        if (playerPos.position.x == 1) return;
        else
        {
            playerPos.Translate(1, 0, 0);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //장애물에 부딪히면 hp 감소
        if (collision.gameObject.tag == "Obstacle")
        {
            director.sfxPlay(SystemManager.Sfx.Hit);
            StartCoroutine("Damaged");
            //감독 스크립트에 플레이어와 장애물이 충돌했다고 전달
            director.DecreaseHP();
        }
        //하트에 부딪히면 hp 5 증가
        else if (collision.gameObject.tag == "Heart")
        {
            director.sfxPlay(SystemManager.Sfx.Heart);
            //감독 스크립트에 플레이어와 아이템이 충돌했다고 전달
            director.increaseHP();
        }
    }
    IEnumerator Damaged()
    {
        //장애물에 부딪히면 깜박거리는 효과 주기
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }


}
