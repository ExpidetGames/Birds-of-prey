using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class UDPClient {
    public static List<string> udpCallStack;
    public static UdpClient updClient;

    public int thisPort;
    private int port;
    private string ip;

    private IPEndPoint remoteEndPoint;
    private Thread sender;
    private Thread receiver;


    public UDPClient(string ip, int serverPort, int localPort) {
        this.ip = ip;
        this.port = serverPort;
        this.thisPort = localPort;
    }

    public void connect() {
        udpCallStack = new List<string>();
        updClient = new UdpClient(thisPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        try {
            updClient.Connect(ip, port);
        } catch(Exception e) {
            Debug.Log("Error occured: " + e.Message);
        }
        sender = new Thread(new ThreadStart(sendCallStack));
        receiver = new Thread(new ThreadStart(receiveMessageOnUDP));
        sender.Start();
        receiver.Start();
    }

    void sendMessageOnUDP(string message) {
        if(!string.IsNullOrEmpty(message)) {
            byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            updClient.Send(sendBytes, sendBytes.Length);
        }
    }

    //Assigned to sender Thread
    void sendCallStack() {
        while(true) {
            if(udpCallStack.Count > 0 && sender != null) {
                sendMessageOnUDP(udpCallStack[udpCallStack.Count - 1]);
                udpCallStack.RemoveAt(udpCallStack.Count - 1);
            } else {
                Thread.Sleep(10);
            }
        }
    }

    //Assigned to receiver thread
    void receiveMessageOnUDP() {
        while(true) {
            byte[] receiveBytes = updClient.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            if(!string.IsNullOrEmpty(receivedString)) {
                JsonParser.decodeJsonMessage(receivedString);
            } else {
                Thread.Sleep(10);
            }
        }
    }

    private void OnApplicationQuit() {
        if(sender != null) {
            sender.Abort();
        }

        if(receiver != null) {
            receiver.Abort();
        }
    }
}
