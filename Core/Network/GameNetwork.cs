using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;

using Navislamia.Configuration;
using Navislamia.Network.Security;

using RappelzPackets;

using Serilog;

namespace Navislamia.Network
{
    public class GameNetwork
    {
        ConfigurationManager configMgr = ConfigurationManager.Instance;

        TcpListener listener = null;

        int bufferLen = 1024;

        List<GameClient> connections = new List<GameClient>();

        AuthClient auth = null;

        XRC4Cipher recvCipher = new XRC4Cipher();
        XRC4Cipher sendCipher = new XRC4Cipher();

        public GameNetwork()
        {
            string cipherKey = configMgr["cipher.key", "Network"];

            bufferLen = configMgr.Get<int>("io.buffer_size", "Network");
            recvCipher.SetKey(cipherKey);
            sendCipher.SetKey(cipherKey);
        }

        public bool Start()
        {
            if (!connectToAuth())
            {
                Log.Fatal("Failed to connect to the Auth server!");
                return false;
            }

            sendGSInfoToAuth();

            if (!startClientListener())
            {
                Log.Fatal("Failed to start client listener!");
                return false;
            }

            return true;
        }

        bool connectToAuth()
        {
            string addrStr = configMgr["io.auth.ip", "Network"];
            short port = configMgr.Get<short>("io.auth.port", "Network");

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                Log.Fatal("Invalid network auth.io configuration! Review your Configuration.json!");
                return false;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                Log.Fatal("Failed to parse auth.io.ip: {addrStr}");
                return false;
            }

            IPEndPoint authEP = new IPEndPoint(addr, port);

            var authSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            int buffLen = configMgr.Get<int>("io.buffer_size", "Network");

            Log.Verbose("IOCP Buffer Length {buffLen} loaded from config!", buffLen);

            auth = new AuthClient(authSock, buffLen);
            auth.Connect(authEP);

            Log.Debug("Connected to Auth server successfully!");

            return true;
        }

        bool sendGSInfoToAuth()
        {
            try
            {
                TS_GA_LOGIN pMsg = new TS_GA_LOGIN();
                pMsg.server_idx = configMgr.Get<ushort>("index", "Server");
                pMsg.server_ip = configMgr.Get<string>("io.ip", "Network");
                pMsg.server_port = configMgr.Get<short>("io.port", "Network");
                pMsg.server_name = configMgr.Get<string>("name", "Server");
                pMsg.server_screenshot_url = configMgr.Get<string>("screenshort.url", "Server");
                pMsg.is_adult_server = configMgr.Get<bool>("adult", "Server");

                auth.Send(pMsg);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        bool startClientListener()
        {
            string addrStr = configMgr["io.ip", "Network"];
            short port = configMgr.Get<short>("io.port", "Network");
            int backlog = configMgr.Get<int>("io.backlog", "Network");

            if (string.IsNullOrEmpty(addrStr) || port == 0 || backlog == 0)
            {
                Log.Fatal("Invalid network io configuration! Review your Configuration.json!");
                return false;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                Log.Fatal("Failed to parse io.ip: {addrStr}");
                return false;
            }

            listener = new TcpListener(addr, port);

            listener.Start(backlog);

            Log.Information("Game network started!");

            listener.BeginAcceptSocket(AttemptAcceptScoket, listener);

            Log.Debug("- Listening at: {0} : {1} with backlog of: {2}", addrStr, port, backlog);

            return true;
        }

        private void AttemptAcceptScoket(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            Socket socket = listener.EndAcceptSocket(ar);

            GameClient client = new GameClient(socket, bufferLen);

            if (!connections.Contains(client))
                connections.Add(client);

            socket.BeginReceive(client.Data, 0, client.DataLength, 0, new AsyncCallback(AttemptReceive), client);
        }

        private void AttemptReceive(IAsyncResult ar)
        {
            GameClient client = (GameClient)ar.AsyncState;
            Socket socket = client.Socket;

            int readCount = socket.EndReceive(ar);

            if (readCount > 0) {
                byte[] decodedBytes = new byte[readCount];

                //recvCipher.Decode(client.Data, decodedBytes, readCount, true);
            }

            // TODO: set client socket for next read
        }
    }
}
