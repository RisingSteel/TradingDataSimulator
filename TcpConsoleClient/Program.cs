using System.Net.Sockets;
using System.Text;

var client = new TcpClient();

await client.ConnectAsync("127.0.0.1", 5000);

using var reader = new StreamReader(client.GetStream(), Encoding.UTF8);

string? line;

while ((line = await reader.ReadLineAsync()) is not null)
{
    Console.WriteLine(line);
}
