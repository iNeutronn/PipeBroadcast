using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Client;

internal class Client : IDisposable
{
    private Guid _id;
    private string _pipeName;
    private NamedPipeClientStream _pipeClient;
    private Task _connectionTask;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private bool disposedValue;

    Client()
    {
        _id = GetId();
        _pipeName = "pipe" + _id.ToString();
        _pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        _connectionTask = _pipeClient.ConnectAsync();
    }

    private Guid GetId()
    {
        using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "idPipe", PipeDirection.InOut);

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

        return _id;
    }

    public string Request(string command)
    {
        if (_pipeClient == null)
        {
            throw new InvalidOperationException("NamedPipeClientStream not initialized.");
        }    
        
        if (!_connectionTask.Wait(TimeSpan.FromSeconds(1)))
        {
            throw new TimeoutException("NamedPipeClientStream not connected.");
        }

        byte[] data = Encoding.UTF8.GetBytes(command);
        _pipeClient.Write(data, 0, data.Length);


        byte[] buffer = new byte[256];
        _pipeClient.Read(buffer, 0, buffer.Length);

        string response = Encoding.UTF8.GetString(buffer);

        return response;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _pipeClient?.Close();
                _pipeClient?.Dispose();
                //_connectionTask?.Dispose(); 
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~Client()
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
