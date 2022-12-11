using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObsDirector : MonoBehaviour
{
    public GameObject[] obsPrefab;
    public Transform[] spawnPoints;

    public float maxSpawnDelay; //딜레이된 시간의 최댓값(스폰 쿨타임)
    public float curSpawnDelay; //현재 장애물이 생성되지 않고 딜레이된 시간

    // Update is called once per frame
    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        //장애물이 생성되지 않은 시간이 최댓값을 넘으면(스폰 쿨타임이 지나면) 장애물 생성
        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnObs();
            //스폰 쿨타임을 랜덤으로 재설정하고 현재 딜레이 시간 초기화
            maxSpawnDelay = Random.Range(0.8f, 2f);
            curSpawnDelay = 0;
        }

    }

    void SpawnObs()
    {
        //랜덤한 장애물을 랜덤한 위치에 생성하기
        int randomObs = Random.Range(0, 2); //장애물 종류 2가지(긴 장애물, 짧은 장애물)
        int ranPoint = Random.Range(0, 3);  //장애물 위치
        Instantiate(obsPrefab[randomObs], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
    }

}
