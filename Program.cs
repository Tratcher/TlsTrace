// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;

Console.WriteLine("Hello, World!");

var capture = new NetCapture("out.pcap");
SocketsHttpHandler handler = new();
handler.PlaintextStreamFilter = (context, cancellationToken) =>
{
    return new(capture.AddStream(context.PlaintextStream));
};

HttpClient client = new(handler);
client.DefaultRequestVersion = new Version(2, 0);
for (int i = 0; i < 400; i++)
{
    var response = await client.GetAsync("https://httpbin.org/headers");
    Console.WriteLine($"Request {i} completed: " + response.Version);
    await Task.Delay(250);
}