using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using LightPhotos.Core.Logging;
using LightPhotos.Core.Protos;

namespace LightPhotos.Core.Helpers;
internal class GrpcHelper<TClient>
    where TClient : class
{
    private GrpcChannel? _channel;
    private TClient? _client;
    private readonly string _address;
    private const string ADDRESS_NULL_ERROR = "address should not be empty.";

    public GrpcHelper(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentNullException(ADDRESS_NULL_ERROR);
        }
        _address = address;
    }

    private bool EnsureChannel(out GrpcChannel? grpcChannel)
    {
        grpcChannel = null;
        if (_channel is not null)
        {
            grpcChannel = _channel;
            return true;
        }
        try
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            grpcChannel = _channel = GrpcChannel.ForAddress(_address, new GrpcChannelOptions { HttpHandler = handler });
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
        return false;
    }

    private bool EnsureClient(out TClient? client)
    {
        client = null;
        if (_client is not null)
        {
            client = _client;
            return true;
        }
        try
        {
            if (EnsureChannel(out var channel))
            {
                if (Activator.CreateInstance(typeof(TClient), channel) is TClient instance)
                {
                    client = _client = instance;
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
        return false;
    }

    public async Task<TResponse> InvokeRequest<TRequest, TResponse>(Func<TClient, Task<TResponse>> requestFunc)
        where TResponse : class
        where TRequest : class
    {
        if (EnsureClient(out var client) && client is not null)
        {
            try
            {
                return await requestFunc(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }
        return Activator.CreateInstance<TResponse>();
    }
}
