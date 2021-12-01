using Mirror;
using UnityEngine;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class ClientMonsterPool : MonoBehaviour
    {
        public Monster prefab;

        Pool<Monster> pool;

        public void ClientStarted()
        {
            var parent = new GameObject("Pool");
            pool = MonsterPool.CreatePool(prefab, parent.transform);
            NetworkClient.RegisterSpawnHandler(prefab.GetComponent<NetworkIdentity>().assetId, SpawnMonster, UnspawnMonster);
        }

        private GameObject SpawnMonster(SpawnMessage msg)
        {
            Monster clone = pool.Take();
            clone.gameObject.SetActive(true);
            return clone.gameObject;
        }

        private void UnspawnMonster(GameObject spawned)
        {
            Monster monster = spawned.GetComponent<Monster>();
            monster.gameObject.SetActive(false);
            pool.Return(monster);
        }
    }
}
