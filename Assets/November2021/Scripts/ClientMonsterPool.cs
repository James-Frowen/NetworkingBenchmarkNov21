using Mirage;
using Mirage.SocketLayer;
using UnityEngine;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    [RequireComponent(typeof(NetworkClient))]
    public class ClientMonsterPool : MonoBehaviour
    {
        public Monster prefab;

        NetworkClient client;
        ClientObjectManager clientObjectManager;

        Pool<Monster> pool;

        private void Awake()
        {
            client = GetComponent<NetworkClient>();
            clientObjectManager = GetComponent<ClientObjectManager>();

            client.Started.AddListener(ClientStarted);
        }

        private void ClientStarted()
        {
            var parent = new GameObject("Pool");
            parent.transform.parent = transform;
            pool = MonsterPool.CreatePool(prefab, parent.transform);
            clientObjectManager.RegisterSpawnHandler(prefab.Identity.PrefabHash, SpawnMonster, UnspawnMonster);
        }

        private NetworkIdentity SpawnMonster(SpawnMessage msg)
        {
            Monster clone = pool.Take();
            clone.gameObject.SetActive(true);
            return clone.Identity;
        }

        private void UnspawnMonster(NetworkIdentity spawned)
        {
            Monster monster = spawned.GetComponent<Monster>();
            monster.gameObject.SetActive(false);
            pool.Put(monster);
        }
    }
}
