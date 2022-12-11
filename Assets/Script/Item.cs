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
    //오브젝트가 활성화될 때
    void OnEnable()
    {
        //type에 맞는 이미지를 불러옴
        anim.SetInteger("Type", type);

    }
    //오브젝트가 비활성화될 때
    void OnDisable()
    {
        //아이템 속성 초기화
        type = 0;
        isCheck = false;
    }
    public void Hide()
    {
        isCheck = true;
        //숨기기 루틴 실행
        StartCoroutine("HideRoutine");
    }
    IEnumerator HideRoutine()
    {
        //일정시간 딜레이를 주기 위한 루틴
        yield return new WaitForSeconds(0.2f);
        isCheck = false;
        gameObject.SetActive(false);
    }

}


