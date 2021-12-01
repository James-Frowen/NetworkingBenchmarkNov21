using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public static class MonsterPool
    {
        private class PoolHolder
        {
            public Pool<Monster> Pool;
        }
        public static Pool<Monster> CreatePool(Monster prefab, Transform parent)
        {
            // need holder like this so that we can pass the refernce of Pool to the object it creates
            var holder = new PoolHolder();
            var pool = new Pool<Monster>(createNewMonsterWrapper(prefab, parent, holder), 0);
            holder.Pool = pool;
            return pool;
        }

        static Func<Monster> createNewMonsterWrapper(Monster prefab, Transform parent, PoolHolder holder)
              => () =>
          {
              Monster monster = GameObject.Instantiate(prefab, parent);
#if DEBUG
              parent.name = $"MonsterSpawner {parent.childCount}";
#endif
              monster.pool = holder.Pool;
              return monster;
          };
    }


    public class MonsterSpawner : MonoBehaviour
    {
        public float monstersToPlayer = 10;
        public Monster prefab;
        public float radius;

        public Pool<Monster> pool;

        private void Update()
        {
            if (NetworkServer.active)
            {
                if (pool == null)
                {
                    pool = MonsterPool.CreatePool(prefab, transform);
                }
                SpawnUpdate();
            }
        }

        private void SpawnUpdate()
        {
            int playerCount = NetworkServer.connections.Count;
            int objectCount = NetworkServer.spawned.Count;

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

            NetworkServer.Spawn(clone.gameObject);
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
