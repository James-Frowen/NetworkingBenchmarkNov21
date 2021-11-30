using Mirage;
using UnityEngine;
using UnityEngine.Rendering;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class PlayerInput : NetworkBehaviour
    {
        PlayerCharacter character;
        private bool headless;

        Vector3 target;

        // faster for human
        float Speed => character.Speed * (headless ? 10 : 20);
        float RotateSpeed => Speed * 5;


        private void Awake()
        {
            headless = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
            character = GetComponent<PlayerCharacter>();
        }

        private void Update()
        {
            if (!IsLocalPlayer)
                return;

            if (headless)
            {
                headlessMove();
            }
            else
            {
                humanMove();
            }
        }

        private void humanMove()
        {
            // rotate
            float horizontal = Input.GetAxis("Horizontal");
            transform.Rotate(0, horizontal * RotateSpeed * Time.deltaTime, 0);

            // move
            float vertical = Input.GetAxis("Vertical");
            Vector3 forward = transform.forward;
            Vector3 move = Speed * Time.deltaTime * vertical * forward;
            transform.Translate(move);

            // force in bounds
            if (Vector3.Distance(transform.position, Vector3.zero) > character.SpawnRadius)
            {
                transform.position = transform.position.normalized * character.SpawnRadius;
            }
        }

        private void headlessMove()
        {
            Vector3 position = transform.position;
            if (Vector3.Distance(target, position) < 0.1f || target == Vector3.zero)
                target = Helper.GetRandomPosition(character.SpawnRadius);

            Vector3 forward = transform.forward;
            // rotate first, so that position and target are never equal
            Vector3 direction = (target - position).normalized;
            Debug.Assert(direction != Vector3.zero, "Direction zero");
            transform.forward = Vector3.RotateTowards(forward, direction, RotateSpeed, RotateSpeed);
            transform.position = Vector3.MoveTowards(position, target, Speed * Time.deltaTime);
        }
    }
}
