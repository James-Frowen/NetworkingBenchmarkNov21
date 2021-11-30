using System;
using Mirage;

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
            if (!IsServer)
                throw new InvalidOperationException("TakeDamage called when server not active");

            Health -= damage;
            if (Health > 0)
                return false;

            ServerObjectManager.Destroy(Identity);

            return true;
        }
    }
}
