using System;
using System.Net;
using Shadowsocks.Protocol;
using Shadowsocks.Protocol.Socks5;

namespace SsLocalCli
{
    // Temporary cli for test purpose
    class Program
    {
        static async void Main(string[] args)
        {
            var l = new TcpPipeListener(new IPEndPoint(IPAddress.Loopback, 1082), new[]
              {
                new Socks5Service(),
            });
            await l.Start();
        }
    }
}
