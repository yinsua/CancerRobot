/*
*	2015-08-11
*	Cancer Robot Communicate Protocol
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;

namespace communicate
{
    class Communicate
    {       
        
        //通信方式
        //  0 - Udp
        //  1 - Serial
        public static int type = 0;
        
        //定义串口
        SerialPort sp = new SerialPort();
        public static string strPortName = "COM1";
        public static string strBaudRate = "9600";
        public static string strDataBits = "8";
        public static string strStopBits = "1";
        
        //用于接收        
        public static Socket recvSocket;
        
        //用于发送
        public static EndPoint sendSocket;
        
        public void Communicate(string arg_1,int arg_2,int arg_3)
        {
            if(type==1)
            {                              
                strPortName = arg_1;
                strBaudRate = Convert.toString(arg_2);
                strDataBits = Convert.toString(arg_3);
                  
                 //定义串口                
                sp.PortName = strPortName;
                sp.BaudRate = int.Parse(strBaudRate);
                sp.DataBits = int.Parse(strDataBits);
                sp.StopBits = (StopBits)int.Parse(strStopBits);
                sp.ReadTimeout = 1000;
                sp.SendTimeout = 1000;
                      
                //打开串口
                sp.Open();                 
            }
            else if(type==0)
            {
                ip = arg_1;
                sendPort = arg_2;
                recvPort = arg_3;
                
                IPEndPoint no = new IPEndPoint(IPAddress.Parse(ip), sendPort);
                sendSocket = (EndPoint)(no);
                sendSocket.SendTimeout = 3000;
                
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, recvPort);
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                recvSocket.ReceiveTimeout = 3000;    
            }
  
            
    
        }
        
        public static void sendData(string data)
        {
            //串口方式
            if (type==1)
            {
                try
                {
                    //第一种发送接收方式
                    sp.Write(data);
                    //string strRecieve1 = sp.ReadExisting();
                   
                    ////第二种发送接收方式
                    //sp.WriteLine(data);
                    //string strRecieve = sp.ReadLine();
         
                    ////第三种发送接收方式
                    //byte[] SendBuf = new byte[256];
                    //SendBuf = System.Text.Encoding.UTF8.GetBytes(data);
                    //int len = data.Length;
                    //sp.Write(SendBuf, 0, len);
         
                    //byte[] RecieveBuf = new byte[256];
                    //sp.Read(RecieveBuf, 0, 256);
                    //string strRecieve = System.Text.Encoding.UTF8.GetString(RecieveBuf);
                    //txtRecieve.Text = strRecieve3;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("发送失败！\n" + ex.Data); 
                }

            }   
            //UDP方式
            else if(type==0)
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
            else
            {
                MessageBox.Show("type error!");
            }            

        }
            
        public static string recvData()
        {
            //串口方式
            if (type==1)
            {
                try
                {
                    string strRecieve = sp.ReadExisting();
                    return strRecieve;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("接收失败！\n" + ex.Data);
                    return "-1";
                }
            }
            //UDP方式
            else if (type==0)
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
                    return "-1";
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
            else
            {
                MessageBox.Show("type error!");
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
        
        public static void closeSerial()
        {
            sp.Close()            
        }
        
        public static void closeSocket()
        {
            //关闭套接字
            recvSocket.Close();
            sendSocket.Close();
        }
    }
}

