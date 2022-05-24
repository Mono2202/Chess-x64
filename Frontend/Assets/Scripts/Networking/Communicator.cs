using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Communicator
{
    // Fields:
    public NetworkStream m_socket;
    private AES m_aes;

    // Constants:
    const int SIZE = 100000;

    /*
     * C'tor
     */
    public Communicator(string ip, int port)
    {
        // Inits:
        m_aes = new AES();

        // Creating the socket with the server:
        try
        {
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            client.Connect(serverEndPoint);
            m_socket = client.GetStream();
        }

        catch
        {
            Debug.Log("Error: no connection with server");
        }
    }

    /*
     * Sends a message to the server
     * Input : message - the client's message
     * Output: < None >
     */
    public void Write(string message)
    {
        // Sending the message to the server:
        try
        {
            byte[] buffer = new ASCIIEncoding().GetBytes(m_aes.AESEncrypt(message));
            m_socket.Write(buffer, 0, buffer.Length);
            m_socket.Flush();
        }

        catch
        {
            Debug.Log("Error: no connection with server");
        }
    }

    /*
     * Reads a message from the server
     * Input : < None >
     * Output: the server's message
     */
    public string Read()
    {
        // Reading the message from the server:
        try
        {
            byte[] buffer = new byte[SIZE];
            m_socket.Read(buffer, 0, SIZE);
            string data = System.Text.Encoding.UTF8.GetString(buffer);
            return m_aes.AESDecrypt(data);
        }

        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return "";
        }
    }
}
