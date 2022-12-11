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
    //�������� ��ĭ �����̱�
    public void moveLeft()
    {
        //�� ���ʿ� ���� �� �̵� x
        if (playerPos.position.x == -1) return;
        else
        {
            playerPos.Translate(-1, 0, 0);
        }
        
    }
    //���������� ��ĭ �����̱�
    public void moveRight()
    {
        //�� �����ʿ� ���� �� �̵� x
        if (playerPos.position.x == 1) return;
        else
        {
            playerPos.Translate(1, 0, 0);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //��ֹ��� �ε����� hp ����
        if (collision.gameObject.tag == "Obstacle")
        {
            director.sfxPlay(SystemManager.Sfx.Hit);
            StartCoroutine("Damaged");
            //���� ��ũ��Ʈ�� �÷��̾�� ��ֹ��� �浹�ߴٰ� ����
            director.DecreaseHP();
        }
        //��Ʈ�� �ε����� hp 5 ����
        else if (collision.gameObject.tag == "Heart")
        {
            director.sfxPlay(SystemManager.Sfx.Heart);
            //���� ��ũ��Ʈ�� �÷��̾�� �������� �浹�ߴٰ� ����
            director.increaseHP();
        }
    }
    IEnumerator Damaged()
    {
        //��ֹ��� �ε����� ���ڰŸ��� ȿ�� �ֱ�
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }


}
