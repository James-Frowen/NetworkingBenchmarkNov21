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
        public int CreateClientCount;

        List<PhysicsScene> physics = new List<PhysicsScene>();
        int clientCount = 0;
        private IEnumerator Start()
        {
            yield return createServer();
            yield return null;
            for (int i = 0; i < CreateClientCount; i++)
            {
                yield return createClient();
            }
        }

        IEnumerator createServer()
        {
            Debug.Log("Create Server");
            yield return SceneManager.LoadSceneAsync(this.scene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });
            Scene scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            physics.Add(scene.GetPhysicsScene());
            Destroy(FindObjectOfType<Camera>().gameObject);
            Destroy(FindObjectOfType<Light>().gameObject);
            NetworkManager manager = Instantiate(prefab);
            NetworkManagerGUI gui = manager.GetComponent<NetworkManagerGUI>();
            if (gui != null) gui.enabled = false;
            SceneManager.MoveGameObjectToScene(manager.gameObject, scene);
            NetworkServer server = manager.Server;
            server.StartServer();
            server.World.onSpawn += (identity) => MoveIfNotChild(identity.gameObject, scene);
            server.World.onSpawn += (identity) => disableOnServer(identity);
            foreach (NetworkIdentity identity in server.World.SpawnedIdentities) { MoveIfNotChild(identity.gameObject, scene); disableOnServer(identity); }
            yield return null;
        }

        private void disableOnServer(NetworkIdentity identity)
        {
            foreach (Renderer renderer in identity.gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }

        IEnumerator createClient()
        {
            clientCount++;
            Debug.Log($"Create Client {clientCount}");
            yield return SceneManager.LoadSceneAsync(this.scene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });
            Scene scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            Camera[] cameras = FindObjectsOfType<Camera>();
            Light[] lights = FindObjectsOfType<Light>();
            for (int i = 1; i < cameras.Length; i++) { Destroy(cameras[i].gameObject); }
            for (int i = 1; i < lights.Length; i++) { Destroy(lights[i].gameObject); }

            physics.Add(scene.GetPhysicsScene());
            NetworkManager manager = Instantiate(prefab);
            NetworkManagerGUI gui = manager.GetComponent<NetworkManagerGUI>();
            if (gui != null) gui.enabled = false; SceneManager.MoveGameObjectToScene(manager.gameObject, scene);
            NetworkClient client = manager.Client;
            client.Connect();
            client.World.onSpawn += (identity) => MoveIfNotChild(identity.gameObject, scene);
            foreach (NetworkIdentity identity in client.World.SpawnedIdentities) MoveIfNotChild(identity.gameObject, scene);
            yield return null;
        }

        void MoveIfNotChild(GameObject target, Scene scene)
        {
            if (target.transform.parent == null)
            {
                SceneManager.MoveGameObjectToScene(target, scene);
            }
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
