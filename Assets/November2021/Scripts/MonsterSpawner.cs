using Mirage;
using Mirage.Logging;
using Mirage.SocketLayer;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class MonsterSpawner : MonoBehaviour
    {
        public NetworkServer Server;
        public ServerObjectManager ServerObjectManager;
        public float monstersToPlayer = 10;
        public Monster prefab;
        public float radius;

        public Pool<Monster> pool;

        private void Awake()
        {
            Server.Started.AddListener(ServerStarted);
        }

        private void ServerStarted()
        {
            pool = new Pool<Monster>(createNewMonster, default, 10, 5000, LogFactory.GetLogger<MonsterSpawner>());
        }

        private void Update()
        {
            if (Server.Active)
            {
                SpawnUpdate();
            }
        }

        private void SpawnUpdate()
        {
            int playerCount = Server.Players.Count;
            int objectCount = Server.World.SpawnedIdentities.Count;

            int targetObjects = Mathf.CeilToInt(playerCount * monstersToPlayer);
            // while less than target
            while (objectCount < targetObjects)
            {
                objectCount++;
                spawnMonster();
            }
        }

        Monster createNewMonster(int _bufferSize, Pool<Monster> pool)
        {
            Monster monster = Instantiate(prefab, Helper.GetRandomPosition(radius), Quaternion.Euler(0, Random.value * 360, 0), transform);
#if DEBUG
            Debug.Log($"Monster Count: {transform.childCount}");
            name = $"MonsterSpawner {transform.childCount}";
#endif
            monster.pool = pool;
            return monster;
        }
        private void spawnMonster()
        {
            Monster clone = pool.Take();
            clone.transform.SetPositionAndRotation(Helper.GetRandomPosition(radius), Quaternion.Euler(0, Random.value * 360, 0));
            clone.Speed = Random.Range(1f, 5f);
            clone.Health = Random.Range(5, 25);

            clone.gameObject.SetActive(true);
            ServerObjectManager.Spawn(clone.Identity);
        }
    }

    public static class Helper
    {
        public static Vector3 GetRandomPosition(float radius)
        {
            Vector3 xz = Random.insideUnitCircle * radius;
            return new Vector3(xz.x, 0, xz.y);
        }
    }
}
