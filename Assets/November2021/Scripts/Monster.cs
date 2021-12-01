using System;
using Mirror;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class Monster : NetworkBehaviour
    {
        [SyncVar]
        public float Speed;
        [SyncVar]
        public int Health;


        public bool TakeDamage(int damage)
        {
            // already dead
            if (Health < 0)
                return false;

            if (!isServer)
                throw new InvalidOperationException("TakeDamage called when server not active");

            Health -= damage;
            // alive
            if (Health > 0)
                return false;

            // dead
            UnSpawn();
            return true;
        }

        public Pool<Monster> pool;

        public void UnSpawn()
        {
            NetworkServer.UnSpawn(gameObject);
            gameObject.SetActive(false);
            pool.Return(this);
        }
    }
}
