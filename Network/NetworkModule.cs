using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Net;
using System.Net.Sockets;

using Configuration;
using Notification;
using Serilog.Events;

using Network.Security;

using Navislamia.Network.Packets;
using Navislamia.Network.Objects;

namespace Network
{
    public class NetworkModule : INetworkService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        TcpListener listener = null;

        int BufferLength = 1024;

        List<GameClient> connections = new List<GameClient>();

        IClient auth = null;

        XRC4Cipher recvCipher = new XRC4Cipher();
        XRC4Cipher sendCipher = new XRC4Cipher();

        public NetworkModule() { }

        public NetworkModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
        }

        public int ConnectToAuth()
        {
            if (connectToAuth() > 0)
                return 1;

            sendGSInfoToAuth();

            return 0;
        }

        int connectToAuth()
        {
            string addrStr = configSVC.Get<string>("io.auth.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.auth.port", "Network", 4502);

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                notificationSVC.WriteMarkup("[bold red]Invalid network auth.io configuration! Review your Configuration.json![/]");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteMarkup($"[bold red]Failed to parse auth.io.ip: {addrStr}[/]");
                return 1;
            }

            IPEndPoint authEP = new IPEndPoint(addr, port);

            var authSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            BufferLength = configSVC.Get<int>("io.buffer_size", "Network", 32768);

            notificationSVC.WriteMarkup($"[orange3]IOCP Buffer Length {BufferLength} loaded from config![/]", LogEventLevel.Verbose);

            int status = 1;

            try
            {
                auth = new AuthClient(authSock, BufferLength, notificationSVC, this);
                status = auth.Connect(authEP);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            if (status == 1)
            {
                notificationSVC.WriteMarkup("[bold red]Failed to connect to the auth server![/]");

                return 1;
            }

            notificationSVC.WriteString("Connected to Auth server successfully!");

            return 0;
        }

        bool sendGSInfoToAuth()
        {
            try
            {
                var idx = configSVC.Get<ushort>("index", "Server", 0);
                var ip = configSVC.Get<string>("io.ip", "Network", "127.0.0.1");
                var port = configSVC.Get<short>("io.port", "Network", 4515);
                var name = configSVC.Get<string>("name", "Server", "Navislamia");
                var screenshot_url = configSVC.Get<string>("screenshort.url", "Server", "about:blank");
                var adult_server = configSVC.Get<bool>("adult", "Server", false);

                var msg = new TS_GA_LOGIN(idx, ip, port, name, screenshot_url, adult_server);

                ((AuthClient)auth).Send(msg, true);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return false;
            }

            return true;
        }

        bool startClientListener()
        {
            string addrStr = configSVC.Get<string>("io.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.port", "Network", 4515);
            int backlog = configSVC.Get<int>("io.backlog", "Network", 100);

            if (string.IsNullOrEmpty(addrStr) || port == 0 || backlog == 0)
            {
                notificationSVC.WriteMarkup("Invalid network io configuration! Review your Configuration.json!");
                return false;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteMarkup($"[bold red]Failed to parse io.ip: {addrStr}[/]");
                return false;
            }

            listener = new TcpListener(addr, port);

            listener.Start(backlog);

            notificationSVC.WriteString("Game network started!", LogEventLevel.Information);

            listener.BeginAcceptSocket(AttemptAcceptScoket, listener);

            notificationSVC.WriteMarkup($"[yellow]- Listening at: {addrStr} : {port} with backlog of: {backlog}[/]\n", LogEventLevel.Information);

            return true;
        }

        private void AttemptAcceptScoket(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            Socket socket = listener.EndAcceptSocket(ar);

            //GameClient client = new GameClient(socket, bufferLen);

            //if (!connections.Contains(client))
            //    connections.Add(client);

            //socket.BeginReceive(client.Data, 0, client.DataLength, 0, new AsyncCallback(AttemptReceive), client);
        }

        private void AttemptReceive(IAsyncResult ar)
        {
            GameClient client = (GameClient)ar.AsyncState;
            //Socket socket = client.Socket; // TODO:

            //int readCount = socket.EndReceive(ar);

            //if (readCount > 0)
            //{
            //    byte[] decodedBytes = new byte[readCount];

            //    //recvCipher.Decode(client.Data, decodedBytes, readCount, true);
            //}

            // TODO: set client socket for next read
        }

        public int StartListener()
        {
            if (!startClientListener())
            {
                notificationSVC.WriteMarkup("[bold red]Failed to start client listener![/]", LogEventLevel.Error);
                return 1;
            }

            return 0;
        }
    }
}
