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
using Objects.Network;
using Serilog.Events;

using Network.Security;

using RappelzPackets;
using Autofac;

namespace Network
{
    public class NetworkModule : Autofac.Module, INetworkService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        TcpListener listener = null;

        int bufferLen = 1024;

        List<GameClient> connections = new List<GameClient>();

        AuthClient auth = null;

        XRC4Cipher recvCipher = new XRC4Cipher();
        XRC4Cipher sendCipher = new XRC4Cipher();

        public NetworkModule() { }

        public NetworkModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;

            string cipherKey = configSVC.Get<string>("cipher.key", "Network", "}h79q~B%al;k'y $E");

            bufferLen = configSVC.Get<int>("io.buffer_size", "Network", 32768);
            recvCipher.SetKey(cipherKey);
            sendCipher.SetKey(cipherKey);
        }

        public int Start()
        {
            // TODO:
            if (!connectToAuth())
            {
                notificationSVC.WriteMarkup("[bold red]Failed to connect to the Auth server![/]", LogEventLevel.Error);
                return 1;
            }

            sendGSInfoToAuth();

            if (!startClientListener())
            {
                notificationSVC.WriteMarkup("[bold red]Failed to start client listener![/]", LogEventLevel.Error);
                return 1;
            }

            return 0;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var configServiceTypes = Directory.EnumerateFiles(Environment.CurrentDirectory)
                .Where(filename => filename.Contains("Modules") && filename.EndsWith("Network.dll"))
                .Select(filepath => Assembly.LoadFrom(filepath))
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(INetworkService).IsAssignableFrom(type) && type.IsClass));

            foreach (var configServiceType in configServiceTypes)
                builder.RegisterType(configServiceType).As<INetworkService>();
        }

        bool connectToAuth()
        {
            string addrStr = configSVC.Get<string>("io.auth.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.auth.port", "Network", 4502);

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                notificationSVC.WriteMarkup("[bold red]Invalid network auth.io configuration! Review your Configuration.json![/]");
                return false;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteMarkup($"[bold red]Failed to parse auth.io.ip: {addrStr}[/]");
                return false;
            }

            IPEndPoint authEP = new IPEndPoint(addr, port);

            var authSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            int buffLen = configSVC.Get<int>("io.buffer_size", "Network", 32768);

            notificationSVC.WriteMarkup($"[orange3]IOCP Buffer Length {buffLen} loaded from config![/]", LogEventLevel.Verbose);

            // TODO:
            //auth = new AuthClient(authSock, buffLen);
            //auth.Connect(authEP);

            notificationSVC.WriteString("Connected to Auth server successfully!");

            return true;
        }

        bool sendGSInfoToAuth()
        {
            try
            {
                TS_GA_LOGIN pMsg = new TS_GA_LOGIN();
                pMsg.server_idx = configSVC.Get<ushort>("index", "Server", 0);
                pMsg.server_ip = configSVC.Get<string>("io.ip", "Network", "127.0.0.1");
                pMsg.server_port = configSVC.Get<short>("io.port", "Network", 4515);
                pMsg.server_name = configSVC.Get<string>("name", "Server", "Navislamia");
                pMsg.server_screenshot_url = configSVC.Get<string>("screenshort.url", "Server", "about:blank");
                pMsg.is_adult_server = configSVC.Get<bool>("adult", "Server", false);

                // TODO:
                //auth.Send(pMsg);
            }
            catch (Exception ex)
            {
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

            notificationSVC.WriteMarkup($"[yellow]- Listening at: {addrStr} : {port} with backlog of: {backlog}[/]", LogEventLevel.Information);

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
    }
}
