/*
*	2015-08-11
*	Cancer Robot Communicate Protocol
*/

using System;
using System.Net;
using System.Net.Sockets;

namespace communicate
{
    class Communicate
    {       
        //用于接收        
        public static Socket recvSocket;
        
        //用于发送
        public static EndPoint sendSocket;
        
        public void Communicate(string ip,int sendPort,int recvPort){
            IPEndPoint no = new IPEndPoint(IPAddress.Parse(ip), sendPort);
            sendSocket = (EndPoint)(no);
            sendSocket.SendTimeout = 3000;
            
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, recvPort);
            recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            recvSocket.ReceiveTimeout = 3000;         
        }
        
        public static void sendData(string data)
        {               
            byte[] sendDataString = new byte[1024];            
            sendDataString = Encoding.ASCII.GetBytes(data);
            try
            {
                sendSocket.SendTo(sendDataString, sendDataString.Length, SocketFlags.None, sendSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送失败！\n" + ex.Data); 
            }
        }
            
        public static string recvData()
        {
            int recv;
            string recvTemp = null;
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            byte[] data = new byte[1024];
            
            try
            {
                recv = recvSocket.ReceiveFrom(data, ref Remote);
            }
            catch (Exception ex)
            {
                MessageBox.Show("接收失败！\n" + ex.Data)
            }
            
            if (recv > 0)
            {
                recvTemp = Encoding.ASCII.GetString(data, 0, recv);
                return recvTemp;
            }
            else
            {
                return "-1";
            }
            
        }
        
        private static String convertToGcodeJSON(string data)
        {
            return "'Gcode':'" + data + "'";
        }
        
        private static String convertToWriteRegJSON(string data)
        {
            return "'WriteReg':'" + data + "'";
        }
    
        private static String convertToReadRegJSON(string data)
        {
            return "'ReadReg':'" + data + "'";
        }
        
        public static void SendGcode(string data)
        {
            sendData(convertToGcodeJSON(data));
        }
        
        public static void WriteReg(int regNumber,int status)
        {
            sendData(convertToWriteRegJSON(Convert.toString(regNumber) + Convert.toString(status)));
        }
        
        public static string ReadReg(int regNumber)
        {
            sendData(convertToReadRegJSON(Convert.toString(regNumber)));
            return recvData();
        }
        
        public void autoSendFile(string fileName,int sleepNum)
        {
            string strLine;
            try
            {
                FileStream aFile = new FileStream(fileName, FileMode.Open);
                StreamReader sr = new StreamReader(aFile);
                strLine = sr.ReadLine();
    
                while (strLine != null)
                {
                    if (strLine != "\n")
                    {
                        //MessageBox.Show(strLine);
                        sendData(convertToGcodeJSON(strLine));                        
                    }
                    strLine = sr.ReadLine();
                    Thread.Sleep(sleepNum);
                }
                sr.Close();
            }
            catch (IOException ex)
            {
                //MessageBox.Show("Read File Error! " + ex.Data);
                return;
            }
        }
    }
}

