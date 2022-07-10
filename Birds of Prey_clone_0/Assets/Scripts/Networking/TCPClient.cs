using System.Collections.Generic;
using WebSocketSharp;
using System.Threading;
using UnityEngine;

//This class is responsible for establishing the connection and receiving Messages
public class TCPClient {
    public static WebSocket ws;
    public static List<string> callStack = new List<string>();
    private List<string> receivedMessages = new List<string>();
    private static Thread senderThread;
    private static Thread receiverThread;

    private string ip;
    private int port;

    public TCPClient(string ip, int port) {
        this.ip = ip;
        this.port = port;
    }

    public void connect() {
        if(string.IsNullOrEmpty(NetworkedVariables.playerId)) {
            ws = new WebSocket($"ws://{ip}:{port}/ws");
            ws.OnMessage += (sender, e) => {
                if(!e.Data.IsNullOrEmpty()) {
                    //Debug.Log(e.Data);
                    receivedMessages.Insert(0, e.Data);
                }
            };
            //ws.OnClose += WsOnOnClose;
            ws.Connect();
        }

        if(senderThread == null) {
            senderThread = new Thread(new ThreadStart(sendCallStack));
            senderThread.Start();
        }

        if(receiverThread == null) {
            receiverThread = new Thread(new ThreadStart(receiveMessages));
            receiverThread.Start();
        }
    }

    void sendCallStack() {
        while(true) {
            if(callStack.Count > 0 && senderThread != null && callStack[callStack.Count - 1] != null) {
                ws.Send(callStack[callStack.Count - 1]);
                callStack.RemoveAt(callStack.Count - 1);
            } else {
                Thread.Sleep(10);
            }
        }
    }

    void receiveMessages() {
        while(true) {
            if(receivedMessages.Count > 0 && receivedMessages != null && receivedMessages[receivedMessages.Count - 1] != null) {
                JsonParser.decodeJsonMessage(receivedMessages[receivedMessages.Count - 1]);
                receivedMessages.RemoveAt(receivedMessages.Count - 1);
            } else {
                Thread.Sleep(10);
            }
        }
    }

    public static void killThreads() {
        if(senderThread != null) {
            senderThread.Abort();
        }

        if(receiverThread != null) {
            receiverThread.Abort();
        }
    }
}

