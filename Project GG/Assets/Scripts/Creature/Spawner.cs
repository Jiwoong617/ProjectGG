using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Coroutine spawnCo;

    [SerializeField] GameObject monster;
    [SerializeField] ObjectHolder objectHolder;
    [SerializeField] float distance = 20f;

    private List<MonsterController> pool = new();
    private GameObject player;

    private void Start()
    {
        player = Global.Player;
        Global.Spawner = this;
        spawnCo = StartCoroutine(SpawnMobs());
    }

    public void Spawn()
    {
        Vector3 pos = GetSpawnPos();
        MonsterController mc = GetClone(monster);
        Instantiate(objectHolder.HoldingObjects[Random.Range(0, objectHolder.HoldingObjects.Count)], mc.transform);

        int hp = Random.Range(90, 120);
        int atk = Random.Range(3, 10);
        mc.Init(pos, new StatInfo(hp, hp, atk, 0, Random.Range(3, 8)));
    }

    private MonsterController GetClone(GameObject go)
    {
        foreach (MonsterController monster in pool)
            if (monster.isFree)
                return monster;

        return MakeNew(go);
    }

    private MonsterController MakeNew(GameObject go)
    {
        MonsterController monster = Instantiate(go, transform).GetComponent<MonsterController>();
        pool.Add(monster);
        return monster;
    }

    private Vector3 GetSpawnPos()
    {
        Vector3 playerPos = player.transform.position;
        float x = Random.Range(distance, distance * 2);
        float z = Random.Range(distance, distance * 2);

        Vector3 pos = new Vector3(playerPos.x + (Random.Range(0, 2) == 0 ? -x : x ),
            0, playerPos.z + (Random.Range(0, 2) == 0 ? -z : z));

        return pos;
    }

    IEnumerator SpawnMobs()
    {
        float spawntime = 3;
        int count = 0;
        while (player.GetComponent<PlayerController>().isAlive)
        {
            Spawn();
            yield return new WaitForSeconds(spawntime);
            count++;
            if(count == 10) { count = 0; spawntime = Mathf.Clamp(spawntime - 0.1f, 0.5f, 3); }
        }
    }
}
