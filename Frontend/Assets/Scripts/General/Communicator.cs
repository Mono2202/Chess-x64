using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Communicator
{
    // Fields:
    private NetworkStream m_socket;

    // Constants:
    private const string IP = "127.0.0.1";
    private const int PORT = 54321;
    private const int SIZE = 4096;


    // C'tor:
    public Communicator()
    {
        // Creating the socket with the server:
        try
        {
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
            client.Connect(serverEndPoint);
            m_socket = client.GetStream();
        }

        catch
        {
            Debug.Log("Error: no connection with server");
        }
    }


    // Methods:

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
            byte[] buffer = new ASCIIEncoding().GetBytes(message);
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
            return System.Text.Encoding.Default.GetString(buffer);
        }

        catch
        {
            Debug.Log("Error: no connection with server");
            return "";
        }
    }
}
