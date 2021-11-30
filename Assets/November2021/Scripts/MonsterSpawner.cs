using Mirage;
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
            while (targetObjects < objectCount)
            {
                targetObjects++;
                spawnMonster();
            }
        }

        private void spawnMonster()
        {
            Monster clone = Instantiate(prefab, Helper.GetRandomPosition(radius), Quaternion.Euler(0, Random.value * 360, 0));
            clone.Speed = Random.Range(1f, 5f);
            clone.Health = Random.Range(5, 25);
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
