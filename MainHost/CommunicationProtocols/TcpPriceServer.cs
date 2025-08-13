using Core.Interfaces;
using Core.Models;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MainHost.CommunicationProtocols
{
    public class TcpPriceServer : IPriceChangeNotifier
    {
        private TcpListener _listener;
        private readonly ILogger<TcpPriceServer> _logger;
        private readonly ConcurrentDictionary<TcpClient, StreamWriter> _conns = new();
        private readonly CancellationTokenSource _cts = new();
        public TcpPriceServer(ILogger<TcpPriceServer> logger)
        {
            _logger = logger;
            _ = Start();
        }

        private async Task Start()
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            _listener.Start();
            _logger.LogDebug("TCP Server started on port 5000");
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    NetworkStream stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
                    _conns[client] = writer;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TCP Server");
            }



            _logger.LogDebug("Client connected");
        }

        public async Task PriceNotifierAsync(PriceTick tick)
        {
            var line = $"{tick.TimestampUtc} : Price for {tick.Symbol} is {tick.Price}";

            foreach (var kvp in _conns)
            {
                TcpClient client = kvp.Key;
                StreamWriter writer = kvp.Value;
                try
                {
                    await writer.WriteLineAsync(line);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Client write failed. Dropping connection.");
                    try { writer.Dispose(); } catch { }
                    try { client.Dispose(); } catch { }
                    _conns.TryRemove(client, out _);
                }
            }
            
        }

    }
}
