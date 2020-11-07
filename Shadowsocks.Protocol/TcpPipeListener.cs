using Pipelines.Sockets.Unofficial;
using Shadowsocks.Protocol.Shadowsocks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Shadowsocks.Protocol
{
    public class TcpPipeListener
    {
        private readonly TcpListener _listener;
        private readonly IEnumerable<IStreamService> _services;

        public TcpPipeListener(IPEndPoint endPoint, IEnumerable<IStreamService> services)
        {
            _listener = new TcpListener(endPoint);
            _services = services;
        }

        public async Task Start()
        {
            _listener.Start();

            while (true)
            {
                var socket = await _listener.AcceptSocketAsync();
                var conn = SocketConnection.Create(socket);

                foreach (var svc in _services)
                {
                    if (await svc.IsMyClient(conn))
                    {
                        // todo: save to list, so we can optionally close them
                        _ = RunService(svc, conn);
                    }
                }
            }
        }

        private async Task RunService(IStreamService svc, SocketConnection conn)
        {
            var s5tcp = new PipePair();

            var raw = await svc.Handle(conn);
            var s5c = new ShadowsocksClient("none", "1");
            var tpc = new TcpPipeClient();
            var t2 = tpc.Connect(IPEndPoint.Parse("127.0.0.1:8388"), s5tcp.DownSide, null);
            var t1 = s5c.Connect(IPEndPoint.Parse("127.0.0.1:10000"), raw, s5tcp.UpSide);
            await Task.WhenAll(t1, t2);
        }

        public void Stop() => _listener.Stop();
    }
}
