//.net 라이브러리
using System;
//.net에서 네트워크 및 소켓통신을 하기 위한 라이브러리
using System.Net;
using System.Net.Sockets;
//데이터를 읽기 / 쓰기 하기 위한 라이브러리
using System.IO;
using System.Threading;//멀티스래딩을 하기 위한 라이브러리
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TCP_Manager : MonoBehaviour
{
    public TMP_InputField IPAdress;
    public TMP_InputField Port;

    [SerializeField] TextMeshProUGUI t_log;

    StreamReader reader;
    StreamWriter writer;

    public TMP_InputField message_input;
    private Message_Pooling message;

    private Queue<string> log = new Queue<string>();

    private void Log_message()
    {
        if (log.Count > 0)
        {
            t_log.text = log.Dequeue();
        }
    }

    #region Server

    public void Server_open()
    {
        message = FindAnyObjectByType<Message_Pooling>();
        Thread th = new Thread(ServerConnect);
        th.IsBackground = true;
        th.Start();
    }

    private void ServerConnect()
    {
        try
        {
            TcpListener tcp = new TcpListener(IPAddress.Parse(IPAdress.text), int.Parse(Port.text));
            tcp.Start();
            log.Enqueue("Server Open");

            TcpClient client = tcp.AcceptTcpClient();
            log.Enqueue("Client 접속확인");
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;

            while (client.Connected == true)
            {
                string readdata = reader.ReadLine();
                message.Message( readdata);
            }
        }
        catch (Exception e)
        {
            log.Enqueue(e.Message);
        }
    }

    #endregion

    #region Client

    public void client_Connect()
    {
        message = FindAnyObjectByType<Message_Pooling>();
        log.Enqueue("Client Connected");
        Thread th = new Thread(client_connect);
        th.IsBackground = true;
        th.Start();
    }

    private void client_connect()
    {
        try
        {
            TcpClient client = new TcpClient();
            IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(IPAdress.text), int.Parse(Port.text));
            client.Connect(ipend);
            log.Enqueue("Server connect Complete");

            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;

            while (client.Connected)
            {
                string readdata = reader.ReadLine();
                message.Message(readdata);
            }
        }
        catch (Exception e)
        {
            log.Enqueue(e.Message);
        }

    }

    #endregion

        public void Sending_btn()
    {
        //내가 보낸 메세지도
        //message box에 넣어야 함.
        if(Sendingmessage(message_input.text))
        {
            message.Message(message_input.text);
            message_input.text = string.Empty;
        }
    }
    private bool Sendingmessage(string me)
    {
        if(writer!=null)
        {
            writer.WriteLine(me);
            return true;
        }
        else
        {
            log.Enqueue("Writer Null!!");
            return false;
        }
    }

    private void Update()
    {
        Log_message();
    }
}
