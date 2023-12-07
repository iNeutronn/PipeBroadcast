using System;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Threading;

namespace Client;

internal class ClientPipe : IDisposable
{
    private Guid _id;
    private readonly string _host; 
    private NamedPipeClientStream _pipeClient; 
    private bool disposedValue;

    public event EventHandler<string> ServerResponseReceived;

    public ClientPipe(string host)
    {
        _host = host;
    }

    public void Connect()
    {
        GetIdFromServer();
        string pipeName = "pipe" + _id.ToString();
        _pipeClient = new NamedPipeClientStream(_host, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        _pipeClient.Connect();
        Thread t = new(new ThreadStart(ListenServer));
        t.IsBackground = true;
        t.Name = $"pipe {_id} Lisener";
        t.Start();
    }


    private void GetIdFromServer()
    {
        using NamedPipeClientStream pipeClient = new NamedPipeClientStream(_host, "idPipe", PipeDirection.InOut);

        try
        {
            pipeClient.Connect();

            byte[] buffer = new byte[36];
            pipeClient.Read(buffer, 0, 36);

            pipeClient.Write(new byte[1], 0, 1);

            _id = new Guid(Encoding.UTF8.GetString(buffer));
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
        if (_pipeClient == null)
        {
            throw new InvalidOperationException("NamedPipeClientStream not initialized.");
        }    

        byte[] data = Encoding.UTF8.GetBytes(command);
        _pipeClient.Write(data, 0, data.Length);
    }

    private void ListenServer()
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
                _pipeClient?.Close();
                _pipeClient?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~ClientPipe()
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
