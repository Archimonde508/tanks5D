using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Sockets;
using System;

public class Connection
{
    static string host = "127.0.0.1";
    static int port = 9000;

    static TcpClient client;

    public static void func()
    {
        if(client != null)
        {
            Debug.Log("Connection is already open");
        }
        else
        {
            try
            {
                client = new TcpClient();
                client.Connect(host, port);
                Debug.Log("Polaczono pomyslnie");
            }
            catch(Exception ex)
            {
                client = null;
                Debug.Log("ERROR: " + ex.Message);
            }
        }
    }
}
