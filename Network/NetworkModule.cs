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
using Navislamia.Network.Packets.Upload;
using Navislamia.Network.Objects;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Interfaces;

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
        IClient upload = null;

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

            notificationSVC.WriteSuccess(new[] { "Connected to Auth server successfully!" }, false); 

            return sendGSInfoToAuth();
        }

        public int ConnectToUpload()
        {
            if (connectToUpload() > 0)
                return 1;

            notificationSVC.WriteSuccess(new[] { "Connected to Upload server successfully!" }, false); // TODO: last param should be default

            return sendInfoToUpload();
        }

        int connectToAuth()
        {
            string addrStr = configSVC.Get<string>("io.auth.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.auth.port", "Network", 4502);

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                notificationSVC.WriteError("Invalid network auth.io.ip configuration! Review your Configuration.json!");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteError($"Failed to parse auth.io.ip: {addrStr}");
                return 1;
            }

            IPEndPoint authEP = new IPEndPoint(addr, port);

            var authSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            BufferLength = configSVC.Get<int>("io.buffer_size", "Network", 32768);

            if (configSVC.Get<bool>("debug", "Runtime", false))
                notificationSVC.WriteDebug($"io.buffer_length is: {BufferLength}");

            int status = 0;

            try
            {
                auth = new AuthClient(authSock, BufferLength, configSVC, notificationSVC, this);
                status = auth.Connect(authEP);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                status = 1;
            }

            if (status == 1)
            {
                notificationSVC.WriteError("Failed to connect to the auth server!");

                return 1;
            }

            return 0;
        }

        int sendGSInfoToAuth()
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

                auth.Send(msg);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }
        
        int connectToUpload()
        {
            string addrStr = configSVC.Get<string>("io.upload.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.upload.port", "Network", 4616);

            if (string.IsNullOrEmpty(addrStr) || port == 0)
            {
                notificationSVC.WriteError("Invalid network io.upload.ip configuration! Review your Configuration.json!");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteError($"Failed to parse io.upload.ip: {addrStr}");
                return 1;
            }

            IPEndPoint uploadEP = new IPEndPoint(addr, port);

            var uploadSock = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            BufferLength = configSVC.Get<int>("io.buffer_size", "Network", 32768);

            int status = 0;

            try
            {
                upload = new UploadClient(uploadSock, BufferLength, configSVC, notificationSVC, this);
                status = upload.Connect(uploadEP);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                status = 1;
            }

            if (status == 1)
            {
                notificationSVC.WriteError("Failed to connect to the upload server!");

                return 1;
            }

            return 0;
        }

        int sendInfoToUpload()
        {
            try
            {
                var serverName = configSVC.Get<string>("name", "Server", "Navislamia");

                var msg = new TS_SU_LOGIN(serverName);

                upload.Send(msg);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }

        int startClientListener()
        {
            string addrStr = configSVC.Get<string>("io.ip", "Network", "127.0.0.1");
            short port = configSVC.Get<short>("io.port", "Network", 4515);
            int backlog = configSVC.Get<int>("io.backlog", "Network", 100);

            if (string.IsNullOrEmpty(addrStr) || port == 0 || backlog == 0)
            {
                notificationSVC.WriteError("Invalid network io configuration! Review your Configuration.json!");
                return 1;
            }

            IPAddress addr;

            if (!IPAddress.TryParse(addrStr, out addr))
            {
                notificationSVC.WriteError($"Failed to parse io.ip: {addrStr}");
                return 1;
            }


            listener = new TcpListener(addr, port);
            listener.Start(backlog);
            listener.BeginAcceptSocket(AttemptAcceptScoket, listener);

            notificationSVC.WriteSuccess(new string[] { "Game network started!", $"- [yellow]Listening at: {addrStr} : {port} with backlog of: {backlog}[/]\n" }, true);

            return 0;
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

        private void AttemptReceive(IAsyncResult ar) // TODO: reimplement
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
            if (startClientListener() > 0)
            {
                notificationSVC.WriteError("Failed to start client listener!");
                return 1;
            }

            return 0;
        }
    }
}
