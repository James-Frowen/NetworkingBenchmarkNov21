using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace JamesFrowen.NetworkBenchmark.November2021
{
    public class CommandLineStartNetwork : MonoBehaviour
    {
        public NetworkManager networkManager;

        // only let 1 StartNetwork setup, incase it gets copied
        static bool hasSetup = false;
        private void Awake()
        {
            if (hasSetup) return;
            hasSetup = true;

            Application.targetFrameRate = 60;

            // to lower because we dont care about case
            string[] args = Environment.GetCommandLineArgs().Select(x => x.ToLower()).ToArray();
            List<ArgCommand> commands = getArgCommands();
            foreach (ArgCommand command in commands)
            {
                int index = Array.IndexOf(args, command.start);
                if (index != -1)
                {
                    command.handler.Invoke(index, args);
                }
            }
        }

        public delegate void HandleArgDelate(int index, IReadOnlyList<string> args);
        private struct ArgCommand
        {
            public string start;
            public HandleArgDelate handler;

            public ArgCommand(string start, HandleArgDelate handler)
            {
                this.start = (start ?? throw new ArgumentNullException(nameof(start))).ToLower();
                this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }
        }

        private List<ArgCommand> getArgCommands()
        {
            var args = new List<ArgCommand>();
            args.Add(new ArgCommand("-transport", parseTransport));
            args.Add(new ArgCommand("-address", parseAddress));
            args.Add(new ArgCommand("-port", parsePort));
            args.Add(new ArgCommand("-server", parseServer));
            args.Add(new ArgCommand("-client", parseClient));

            return args;
        }

        private void parseTransport(int index, IReadOnlyList<string> args)
        {
            throw new NotSupportedException();
            //Type newType = getNewTransportType(args[index + 1]);

            //SocketFactory old = networkManager.Server.SocketFactory;
            //GameObject.Destroy(old);

            //var factory = networkManager.gameObject.AddComponent(newType) as SocketFactory;

            //networkManager.Server.SocketFactory = factory;
            //networkManager.Client.SocketFactory = factory;
        }

        private Type getNewTransportType(string v)
        {
            throw new NotImplementedException();
        }

        static string address;
        private void parseAddress(int index, IReadOnlyList<string> args)
        {
            address = args[index + 1];
            //if (networkManager.Server.SocketFactory is IHasAddress hasAddress)
            //{
            //    hasAddress.Address = args[index + 1];
            //}
            //else
            //{
            //    Debug.LogError($"Socket Factory does not support Address, Type:{networkManager.Server.SocketFactory?.GetType()}");
            //}
        }

        private void parsePort(int index, IReadOnlyList<string> args)
        {
            throw new NotSupportedException();
            //if (networkManager.Server.SocketFactory is IHasPort hasPort)
            //{
            //    hasPort.Port = int.Parse(args[index + 1]);
            //}
            //else
            //{
            //    Debug.LogError($"Socket Factory does not support Port, Type:{networkManager.Server.SocketFactory?.GetType()}");
            //}
        }

        private void parseServer(int index, IReadOnlyList<string> args)
        {
            NetworkManager.singleton.StartServer();
        }

        private void parseClient(int index, IReadOnlyList<string> args)
        {
            int count = GetClientCount(index, args);
            if (count != 1)
            {
                throw new NotSupportedException();
            }

            NetworkManager.singleton.networkAddress = string.IsNullOrEmpty(address) ? "localhost" : address;
            NetworkManager.singleton.StartClient();
        }

        /// <summary>
        /// Returns the number after the -client arg, or 1 if there is none
        /// </summary>
        private static int GetClientCount(int index, IReadOnlyList<string> args)
        {
            // not enough args
            if (index + 1 >= args.Count)
                return 1;

            string next = args[index + 1];
            // is another Command
            if (next.StartsWith("-"))
            {
                return 1;
            }

            int count = int.Parse(next);
            if (count < 1) throw new ArgumentOutOfRangeException("Client Count", count, "Client count should be atleast 1");
            return count;
        }
    }
}
