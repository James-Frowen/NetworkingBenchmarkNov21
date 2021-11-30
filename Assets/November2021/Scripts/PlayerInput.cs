using Mirage;
using UnityEngine;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class PlayerInput : NetworkBehaviour
    {
        PlayerCharacter character;
        private bool headless;

        Vector3 target;


        private void Awake()
        {
            headless = SystemInfo.graphicsDeviceName == null;
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
            transform.Rotate(0, horizontal * character.Speed * Time.deltaTime, 0);

            // move
            float vertical = Input.GetAxis("Vertical");
            Vector3 forward = transform.forward;
            Vector3 move = character.Speed * Time.deltaTime * vertical * forward;
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
            if (Vector3.Distance(target, position) < 0.1f)
                target = Helper.GetRandomPosition(character.SpawnRadius);

            Vector3 forward = transform.forward;
            // rotate first, so that position and target are never equal
            Vector3 direction = (target - position).normalized;
            Debug.Assert(direction != Vector3.zero, "Direction zero");
            transform.forward = Vector3.RotateTowards(forward, direction, character.Speed, character.Speed);
            transform.position = Vector3.MoveTowards(position, target, character.Speed * Time.deltaTime);
        }
    }
}
