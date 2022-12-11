using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float bgSpeed;
    public SystemManager manager;
    public int startIndex; //맨 위 배경의 숫자(2)
    public int endIndex; //맨 아래 배경의 숫자(0)
    public Transform[] background;  //배경 3개 담을 배열 선언

    // Update is called once per frame
    void Update()
    {
        bgSpeed = manager.speed;
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * bgSpeed * Time.deltaTime;
        transform.position = curPos + nextPos;

        //맨 아래 배경이 -10보다 내려가면 위로 붙이기
        //맨 처음 단계에서는 배경 순서가 위부터 2 1 0
        if (background[endIndex].position.y < -10)
        {
            Vector3 bgTopPos = background[startIndex].localPosition; //bg2
            Vector3 bgBottomPos = background[endIndex].localPosition; //bg0
            //bg0을 bg2 위로 붙이기
            background[endIndex].transform.localPosition = bgTopPos + Vector3.up * 10;

            //배경 순서 0 2 1
            //start=0, end=1로 바뀌어야
            int tmpStartIndex = startIndex;
            startIndex = endIndex;
            endIndex = tmpStartIndex - 1 == -1 ? background.Length - 1 : tmpStartIndex - 1;
        }
    }
}
