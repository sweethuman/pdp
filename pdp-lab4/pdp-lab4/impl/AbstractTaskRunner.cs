using System.Text;
using pdp_lab4.domain;
using pdp_lab4.utils;

namespace pdp_lab4.impl;

public abstract class AbstractTaskRunner
{
    protected void ConnectCallback(IAsyncResult ar)
    {
        // retrieve the details from the connection information wrapper
        var resultSocket = (CustomSocket)ar.AsyncState;
        var clientSocket = resultSocket.sock;
        var clientId = resultSocket.id;
        var hostname = resultSocket.hostname;

        clientSocket.EndConnect(ar); // complete connection

        Console.WriteLine("Connection {0} > Socket connected to {1} ({2})", clientId, hostname,
            clientSocket.RemoteEndPoint);

        resultSocket.connectDone.Set(); // signal connection is up
    }

    protected static void SendCallback(IAsyncResult ar)
    {
        var resultSocket = (CustomSocket)ar.AsyncState;
        var clientSocket = resultSocket.sock;
        var clientId = resultSocket.id;

        var bytesSent = clientSocket.EndSend(ar); // complete sending the data to the server

        Console.WriteLine("Connection {0} > Sent {1} bytes to server.", clientId, bytesSent);

        resultSocket.sendDone.Set(); // signal that all bytes have been sent
    }

    protected static void ReceiveCallback(IAsyncResult ar)
    {
        // retrieve the details from the connection information wrapper
        var resultSocket = (CustomSocket)ar.AsyncState;
        var clientSocket = resultSocket.sock;

        try
        {
            // read data from the remote device.
            var bytesRead = clientSocket.EndReceive(ar);

            // get from the buffer, a number of characters <= to the buffer size, and store it in the responseContent
            resultSocket.responseContent.Append(Encoding.ASCII.GetString(resultSocket.buffer, 0, bytesRead));

            // if the response header has not been fully obtained, get the next chunk of data
            if (!Parser.ResponseHeaderObtained(resultSocket.responseContent.ToString()))
                clientSocket.BeginReceive(resultSocket.buffer, 0, CustomSocket.BUFF_SIZE, 0, ReceiveCallback,
                    resultSocket);
            else
                resultSocket.receiveDone.Set(); // signal that all bytes have been received
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public abstract void Run(List<string> urls);
}
