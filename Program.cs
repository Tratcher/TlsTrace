// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using System.Net.Security;
using System.Net.Sockets;

Console.WriteLine("Hello, World!");

SocketsHttpHandler handler = new SocketsHttpHandler();
handler.ConnectCallback = async (context, cancellationToken) =>
{
    // Console.WriteLine($"Connecting to {context.DnsEndPoint}");
    var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
    await socket.ConnectAsync(context.DnsEndPoint);
    var network = new NetworkStream(socket, ownsSocket: true);
    var capture = new NetCapture("out.pcap");
    var pcap = capture.AddStream(network);
    var options = new SslClientAuthenticationOptions
    {
        TargetHost = context.DnsEndPoint.Host,
    };
    options.ApplicationProtocols = new List<SslApplicationProtocol>
    {
        SslApplicationProtocol.Http2,
    };
    await pcap.AuthenticateAsClientAsync(options);
    return pcap;
};

HttpClient client = new HttpClient(handler);
client.DefaultRequestVersion = new Version(2, 0);
for (int i = 0; i < 400; i++)
{
    var response = await client.GetAsync("https://httpbin.org/headers");
    Console.WriteLine($"Request {i} completed: " + response.Version);
    await Task.Delay(1000);
}