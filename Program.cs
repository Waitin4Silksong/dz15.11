using System.Net;
using System.Net.Sockets;
using System.Text;

await using Socket tcpListener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try
{
    tcpListener.Bind(new IPEndPoint(IPAddress.Any, 8888));
    tcpListener.Listen();
    Console.WriteLine("Server run. Waiting connection...");

    while (true)
    {
        using var tcpClient = await tcpListener.AcceptAsync();
        var buffer = new List<byte>();
        var bytesRead = new byte[1];

        while (true)
        {
            var count = await tcpClient.ReceiveAsync(bytesRead.AsMemory(0, 1), SocketFlags.None);
            if (count == 0 || bytesRead[0] == '\n') break;
            buffer.Add(bytesRead[0]);
        }

        var message = Encoding.UTF8.GetString(buffer.ToArray());
        if (message.Trim() == "END") break;

        Console.WriteLine($"Catch message: {message}");
        buffer.Clear();
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
