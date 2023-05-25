using System;
using System.Net.Sockets;
using UnityEngine;

public class ServerTest : MonoBehaviour
{
    private string serverAddress;
    private int serverPort;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer;

    private void Start()
    {
        var envConfig = ServerConfigurationScriptable.Instance.LoadSelectedConfig();
        if(envConfig == null)
        {
            Debug.LogError("ServerConfig.json file not found in Resources folder.");
            return;
        }
        serverAddress = envConfig.IPAddress;
        serverPort = envConfig.Port;
        ConnectToServer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToServer("Hello, Server!");
        }
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverAddress, serverPort);
            stream = client.GetStream();

            receiveBuffer = new byte[1024];
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);

            Debug.Log("Connected to the server.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to the server: {e.Message}");
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int bytesRead = stream.EndRead(result);
            if (bytesRead <= 0)
            {
                DisconnectFromServer();
                return;
            }

            string message = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
            Debug.Log($"Received from server: {message}");

            // Continue reading from the stream
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while receiving data: {e.Message}");
            DisconnectFromServer();
        }
    }

    private void SendMessageToServer(string message)
    {
        try
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            Debug.Log($"Sent to server: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while sending data to the server: {e.Message}");
        }
    }

    private void DisconnectFromServer()
    {
        stream.Close();
        client.Close();
        Debug.Log("Disconnected from the server.");
    }
}
