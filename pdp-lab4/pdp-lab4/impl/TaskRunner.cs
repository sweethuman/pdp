using System.Net;
using System.Net.Sockets;
using System.Text;
using pdp_lab4.domain;
using pdp_lab4.utils;

namespace pdp_lab4.impl;

public class TaskRunner : AbstractTaskRunner
{
    private List<string> urls;

    public override void Run(List<string> urls)
    {
        this.urls = urls;
        var tasks = new List<Task>();

        for (var i = 0; i < this.urls.Count; i++)
        {
            tasks.Add(Task.Factory.StartNew(DoStart, i));
        }

        Task.WaitAll(tasks.ToArray());
    }

    public void DoStart(object idObject)
    {
        var id = (int)idObject;
        Console.WriteLine("Running Connection {0}, on thread {1}", id, Thread.CurrentThread.ManagedThreadId);
        StartClient(urls[id], id);
    }

    private void StartClient(string host, int id)
    {
        var ipHostInfo = Dns.GetHostEntry(host.Split('/')[0]);
        var ipAddr = ipHostInfo.AddressList[0];
        var remEndPoint = new IPEndPoint(ipAddr, Parser.PORT);

        var client = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        var requestSocket = new CustomSocket
        {
            sock = client,
            hostname = host.Split('/')[0],
            endpoint = host.Contains("/") ? host.Substring(host.IndexOf("/", StringComparison.Ordinal)) : "/",
            remoteEndPoint = remEndPoint,
            id = id
        };

        Connect(requestSocket).Wait(); // connect to remote server
        Send(requestSocket, Parser.GetRequestString(requestSocket.hostname, requestSocket.endpoint))
            .Wait(); // request data from server
        Receive(requestSocket).Wait(); // receive server response

        Console.WriteLine("Connection {0} > Content length is:{1}", requestSocket.id,
            Parser.GetContentLen(requestSocket.responseContent.ToString()));

        // release the socket
        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    private Task Connect(CustomSocket state)
    {
        state.sock.BeginConnect(state.remoteEndPoint, ConnectCallback, state);

        return Task.FromResult(state.connectDone.WaitOne()); // block until signaled
    }

    private Task Send(CustomSocket state, string data)
    {
        // convert the string data to byte data using ASCII encoding.
        var byteData = Encoding.ASCII.GetBytes(data);

        // send data
        state.sock.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, state);

        return Task.FromResult(state.sendDone.WaitOne());
    }

    private Task Receive(CustomSocket state)
    {
        // receive data
        state.sock.BeginReceive(state.buffer, 0, CustomSocket.BUFF_SIZE, 0, ReceiveCallback, state);

        return Task.FromResult(state.receiveDone.WaitOne());
    }
}
