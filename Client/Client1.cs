﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Client;

internal class Client1 : IDisposable
{
    private Guid _id;
    private string _pipeName;
    private string _host;
    private NamedPipeClientStream _pipeClient; 
    private Task _connectionTask;
    private readonly CancellationTokenSource _clientClosencst = new CancellationTokenSource();
    private bool disposedValue;

    public event EventHandler<string> ServerResponseReceived;

    public Client1(string host)
    {
        _host = host;
    }

    private void Connect()
    {
        GetIdFromServer();
        _pipeName = "pipe" + _id.ToString();
        _pipeClient = new NamedPipeClientStream(_host, _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        //TODO: розібратися з token
        _connectionTask = _pipeClient.ConnectAsync(_clientClosencst.Token);
    }


    private void GetIdFromServer()
    {
        using NamedPipeClientStream pipeClient = new NamedPipeClientStream(_host, "idPipe", PipeDirection.InOut);

        try
        {
            pipeClient.Connect();

            byte[] buffer = new byte[16];
            pipeClient.Read(buffer, 0, 16);

            pipeClient.Write(new byte[1], 0, 1);

            _id = new Guid(buffer);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error writing to named pipe: {ex.Message}");
        }
        finally
        {
            pipeClient.Close();
        }
    }

    public void SendCommand(string command)
    {
        if (_pipeClient == null || _connectionTask == null)
        {
            throw new InvalidOperationException("NamedPipeClientStream not initialized.");
        }    
        
        if (!_connectionTask.Wait(TimeSpan.FromSeconds(1)))
        {
            throw new TimeoutException("NamedPipeClientStream not connected.");
        }

        byte[] data = Encoding.UTF8.GetBytes(command);
        _pipeClient.Write(data, 0, data.Length);


        //byte[] buffer = new byte[256];
        //_pipeClient.Read(buffer, 0, buffer.Length);

        //string response = Encoding.UTF8.GetString(buffer);

        //return response;
    }

    public void ListenServer()
    {
        byte[] buffer = new byte[256];

        while (true)
        {
            int bytesRead = _pipeClient.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                // Handle received data from the server as needed
                OnServerResponseReceived(receivedData);
            }
        }
    }

    protected virtual void OnServerResponseReceived(string response)
    {
        ServerResponseReceived?.Invoke(this, response);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _clientClosencst.Cancel();

                try
                {
                    _connectionTask?.Wait();
                }
                catch (AggregateException ex)
                {
                    // Handle exceptions if needed
                    Console.WriteLine($"Exception while waiting for the infinite loop task: {ex}");
                }


                _clientClosencst.Dispose();
                _pipeClient?.Close();
                _pipeClient?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~Client1()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
