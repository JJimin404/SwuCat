using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObsDirector : MonoBehaviour
{
    public GameObject[] obsPrefab;
    public Transform[] spawnPoints;

    public float maxSpawnDelay; //�����̵� �ð��� �ִ�(���� ��Ÿ��)
    public float curSpawnDelay; //���� ��ֹ��� �������� �ʰ� �����̵� �ð�

    // Update is called once per frame
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        //��ֹ��� �������� ���� �ð��� �ִ��� ������(���� ��Ÿ���� ������) ��ֹ� ����
        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnObs();
            //���� ��Ÿ���� �������� �缳���ϰ� ���� ������ �ð� �ʱ�ȭ
            maxSpawnDelay = Random.Range(0.8f, 2f);
            curSpawnDelay = 0;
        }

    }

    void SpawnObs()
    {
        //������ ��ֹ��� ������ ��ġ�� �����ϱ�
        int randomObs = Random.Range(0, 2); //��ֹ� ���� 2����(�� ��ֹ�, ª�� ��ֹ�)
        int ranPoint = Random.Range(0, 3);  //��ֹ� ��ġ
        Instantiate(obsPrefab[randomObs], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
    }

}
