using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float bgSpeed;
    public SystemManager manager;
    public int startIndex; //�� �� ����� ����(2)
    public int endIndex; //�� �Ʒ� ����� ����(0)
    public Transform[] background;  //��� 3�� ���� �迭 ����

    // Update is called once per frame
    void Update()
    {
        bgSpeed = manager.speed;
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * bgSpeed * Time.deltaTime;
        transform.position = curPos + nextPos;

        //�� �Ʒ� ����� -10���� �������� ���� ���̱�
        //�� ó�� �ܰ迡���� ��� ������ ������ 2 1 0
        if (background[endIndex].position.y < -10)
        {
            Vector3 bgTopPos = background[startIndex].localPosition; //bg2
            Vector3 bgBottomPos = background[endIndex].localPosition; //bg0
            //bg0�� bg2 ���� ���̱�
            background[endIndex].transform.localPosition = bgTopPos + Vector3.up * 10;

            //��� ���� 0 2 1
            //start=0, end=1�� �ٲ���
            int tmpStartIndex = startIndex;
            startIndex = endIndex;
            endIndex = tmpStartIndex - 1 == -1 ? background.Length - 1 : tmpStartIndex - 1;
        }
    }
}
