using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int type;
    public bool isCheck;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    //������Ʈ�� Ȱ��ȭ�� ��
    void OnEnable()
    {
        //type�� �´� �̹����� �ҷ���
        anim.SetInteger("Type", type);

    }
    //������Ʈ�� ��Ȱ��ȭ�� ��
    void OnDisable()
    {
        //������ �Ӽ� �ʱ�ȭ
        type = 0;
        isCheck = false;
    }
    public void Hide()
    {
        isCheck = true;
        //����� ��ƾ ����
        StartCoroutine("HideRoutine");
    }
    IEnumerator HideRoutine()
    {
        //�����ð� �����̸� �ֱ� ���� ��ƾ
        yield return new WaitForSeconds(0.2f);
        isCheck = false;
        gameObject.SetActive(false);
    }

}


