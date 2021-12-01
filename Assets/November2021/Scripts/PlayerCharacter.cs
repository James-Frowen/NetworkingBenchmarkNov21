using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class PlayerCharacter : NetworkBehaviour
    {
        public float SpawnRadius;

        [SyncVar]
        public float Speed = 3;
        [SyncVar]
        public int Damage = 1;
        [SyncVar]
        public int Level = 1;
        [SyncVar]
        public int XP;


        public override void OnStartClient()
        {
            Material mat = GetComponent<Renderer>().material;
            if (isLocalPlayer)
            {
                mat.color = new Color(0, 0, 0.6f);
            }
            else
            {
                mat.color = new Color(0.6f, 0, 0);
            }
        }

        public override void OnStartServer()
        {
            transform.SetPositionAndRotation(Helper.GetRandomPosition(SpawnRadius), Quaternion.Euler(0, Random.value * 360, 0));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isServer)
                return;

            if (other.TryGetComponent(out Monster monster))
            {
                bool killed = monster.TakeDamage(Damage);
                if (killed)
                {
                    XP++;
                    if (XP > Level * Level * Level)
                    {
                        LevelUp();
                    }
                }
                // if not dead, shove monster to right/left
                else
                {
                    monster.transform.position += transform.right * (Random.value < 0.5f ? 2 : -2);
                }
            }
        }

        private void LevelUp()
        {
            XP = 0;
            Level++;
            if (Random.value > 0.2)
            {
                Damage += (int)Mathf.Sqrt(Level);
            }
            else
            {
                Speed += (int)Mathf.Sqrt(Mathf.Sqrt(Level));
            }
        }
    }
}
