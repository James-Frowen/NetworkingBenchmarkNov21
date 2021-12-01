using Mirage;
using Mirage.Logging;
using Mirage.SocketLayer;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public static class MonsterPool
    {
        public static Pool<Monster> CreatePool(Monster prefab, Transform parent)
        {
            // 0 initial size so unspawned objects dont apear on client
            return new Pool<Monster>(createNewMonsterWrapper(prefab, parent), default, 0, 5000, LogFactory.GetLogger<MonsterSpawner>());
        }

        static Pool<Monster>.CreateNewItem createNewMonsterWrapper(Monster prefab, Transform parent)
              => (int _bufferSize, Pool<Monster> pool) =>
          {
              Monster monster = GameObject.Instantiate(prefab, parent);
#if DEBUG
              parent.name = $"MonsterSpawner {parent.childCount}";
#endif
              monster.pool = pool;
              return monster;
          };
    }


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
            pool = MonsterPool.CreatePool(prefab, transform);
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
