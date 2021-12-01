using Mirror;
using UnityEngine;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class GodNetworkManager : NetworkManager
    {
        public override void OnStartClient()
        {
            Debug.Log("God of the network mangaer, please may you run the Pool script before the server spawns any objects on this client");
            ClientMonsterPool pool = GetComponent<ClientMonsterPool>();
            pool.ClientStarted();
            base.OnStartClient();
        }
    }
}
