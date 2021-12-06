using System.Collections;
using System.Collections.Generic;
using Mirage;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class DebugStart : MonoBehaviour
    {
        [Scene] public string scene;
        public NetworkManager prefab;

        List<PhysicsScene> physics = new List<PhysicsScene>();
        int clientCount = 0;
        private IEnumerator Start()
        {
            yield return createServer();
            yield return null;
            yield return createClient();
        }

        IEnumerator createServer()
        {
            yield return SceneManager.LoadSceneAsync(this.scene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });
            Scene scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            NetworkManager manager = Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(manager.gameObject, scene);
            NetworkServer server = manager.Server;
            server.StartServer();
            server.World.onSpawn += (identity) => SceneManager.MoveGameObjectToScene(identity.gameObject, scene);
            foreach (NetworkIdentity identity in server.World.SpawnedIdentities) SceneManager.MoveGameObjectToScene(identity.gameObject, scene);
            yield return null;
        }
        IEnumerator createClient()
        {
            clientCount++;
            yield return SceneManager.LoadSceneAsync(this.scene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });
            Scene scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            NetworkManager manager = Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(manager.gameObject, scene);
            NetworkClient client = manager.Client;
            client.Connect();
            client.World.onSpawn += (identity) => SceneManager.MoveGameObjectToScene(identity.gameObject, scene);
            foreach (NetworkIdentity identity in client.World.SpawnedIdentities) SceneManager.MoveGameObjectToScene(identity.gameObject, scene);
            yield return null;
        }


        private void FixedUpdate()
        {
            foreach (PhysicsScene physics in physics)
            {
                physics.Simulate(Time.fixedDeltaTime);
            }
        }
    }
}
