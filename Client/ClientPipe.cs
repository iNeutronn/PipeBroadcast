using System;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.DataTranslators;

namespace Client;



public class ClientPipe : IDisposable
{
    private Guid _id;
    private string _host;
    private NamedPipeClientStream _pipeClient;
    private Task _serverResponses;
    private bool disposedValue;
    private Thread listenServerThrerad;

    private bool _isSubscribedToWeather = false;
    private bool _isSubscribedToShares = false;
    private bool _isSubscribedToCurrency = false;

    public bool IsSubscribedToWeather => _isSubscribedToWeather;
    public bool IsSubscribedToShares => _isSubscribedToShares;
    public bool IsSubscribedToCurrency => _isSubscribedToCurrency;

   


    public event EventHandler<string> OnSharesRecived;
    public event EventHandler<string> OnWeatherRecived;
    public event EventHandler<string> OnCurrencyRecived;

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
        listenServerThrerad = new Thread(ListenServer);
        listenServerThrerad.IsBackground = true;
        listenServerThrerad.Start();
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
        while (_pipeClient == null || !_pipeClient.IsConnected)
        {
            Thread.Sleep(100);
        }
        if (_pipeClient == null)
        {
            throw new InvalidOperationException("NamedPipeClientStream not initialized.");
        }


        byte[] data = Encoding.UTF8.GetBytes(command);
        _pipeClient.Write(data, 0, data.Length);


    }

    private void ListenServer()
    {
        _serverResponses = Task.Run(() =>
        {
            if (_pipeClient == null)
            {
                throw new InvalidOperationException("NamedPipeClientStream not initialized.");
            }

            byte[] buffer = new byte[100000];

            while (true)
            {
                int bytesRead = _pipeClient.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    TransitionObject trasitionObject = JsonConvert.DeserializeObject<TransitionObject>(receivedData!)!;
                    switch (trasitionObject.Header)
                    {
                        case "SharesData":
                            OnCurrencyRecived?.Invoke(this, trasitionObject.Data);
                            break;
                        case "WeatherData":
                            OnWeatherRecived?.Invoke(this, trasitionObject.Data);
                            break;
                        case "CurrencyData":
                            OnSharesRecived?.Invoke(this, trasitionObject.Data);
                            break;
                        case
                            "ServisData":
                            ProcessServiseData(trasitionObject.Data);
                            break;
                        default:
                            throw new InvalidOperationException("Unknown header of revived record");
                    }
                   
                    
                }
            }
        });
    }

    private void ProcessServiseData(string data)
    {
        throw new NotImplementedException();
    }



    #region subscriprion functions
    public void SubscribeToWeather()
    {
        if (_isSubscribedToWeather) return;
        SendCommand("SubscribToWeather");
        _isSubscribedToWeather = true;
    }

    public void SubscribeToShares()
    {
        if (_isSubscribedToShares) return;
        SendCommand("SubscribToShares");
        _isSubscribedToShares = true;
    }

    public void SubscribeToCurrency()
    {
        if (_isSubscribedToCurrency) return;
        SendCommand("SubscribToCurrency");
        _isSubscribedToCurrency = true;
    }

    public void UnSubscribeToWeather()
    {
        if (!_isSubscribedToWeather) return;
        SendCommand("UnSubscribToWeather");
        _isSubscribedToWeather = false;
    }

    public void UnSubscribeToShares()
    {
        if (!_isSubscribedToShares) return;
        SendCommand("UnSubscribToShares");
        _isSubscribedToShares = false;
    }

    public void UnSubscribeToCurrency()
    {
        if (!_isSubscribedToCurrency) return;
        SendCommand("UnSubscribToCurrency");
        _isSubscribedToCurrency = false;
    }

    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                SendCommand("quit");
                _pipeClient?.Close();
                _pipeClient?.Dispose();
                _serverResponses?.Dispose();
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
