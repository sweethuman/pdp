using System.Net;
using System.Net.Sockets;
using System.Text;
using pdp_lab4.domain;
using pdp_lab4.utils;

namespace pdp_lab4.impl;

public class TaskRunnerAsync : AbstractTaskRunner
{
    private List<string> hosts;

    public override void Run(List<string> urls)
    {
        this.hosts = urls;
        var tasks = new List<Task>();

        for (var i = 0; i < this.hosts.Count; i++)
        {
            tasks.Add(Task.Factory.StartNew(DoStart, i));
        }
        Task.WaitAll(tasks.ToArray());
    }

    private void DoStart(object idObject)
    {
        var id = (int)idObject;
        Console.WriteLine("Running Connection {0}, on thread {1}", id, Thread.CurrentThread.ManagedThreadId);
        StartClient(hosts[id], id);
    }

    private async void StartClient(string host, int id)
    {
        var ipHostInfo = Dns.GetHostEntry(host.Split('/')[0]);
        var ipAddress = ipHostInfo.AddressList[0];
        var remoteEndpoint = new IPEndPoint(ipAddress, Parser.PORT);

        // create the TCP/IP socket
        var client =
            new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // create client socket

        var requestSocket = new CustomSocket
        {
            sock = client,
            hostname = host.Split('/')[0],
            endpoint = host.Contains("/") ? host.Substring(host.IndexOf("/", StringComparison.Ordinal)) : "/",
            remoteEndPoint = remoteEndpoint,
            id = id
        }; // state object

        await Connect(requestSocket); // connect to remote server

        await Send(requestSocket,
            Parser.GetRequestString(requestSocket.hostname,
                requestSocket.endpoint)); // request data from the server

        await Receive(requestSocket); // receive server response

        Console.WriteLine("Connection {0} > Content length is:{1}", requestSocket.id,
            Parser.GetContentLen(requestSocket.responseContent.ToString()));

        // release the socket
        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    private async Task Connect(CustomSocket state)
    {
        state.sock.BeginConnect(state.remoteEndPoint, ConnectCallback, state);

        await Task.FromResult(state.connectDone.WaitOne()); // block until signaled
    }


    private async Task Send(CustomSocket state, string data)
    {
        var byteData = Encoding.ASCII.GetBytes(data);

        // send data
        state.sock.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, state);

        await Task.FromResult<object>(state.sendDone.WaitOne());
    }


    private async Task Receive(CustomSocket state)
    {
        // receive data
        state.sock.BeginReceive(state.buffer, 0, CustomSocket.BUFF_SIZE, 0, ReceiveCallback, state);

        await Task.FromResult<object>(state.receiveDone.WaitOne());
    }
}
