﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
namespace R2D2Robot
{
	class R2D2Networking
	{
		NetType netType;
		UdpClient client;
		IPEndPoint remote;
		Thread timeout;
        Thread receiveThread;
		public bool connected { get; private set;}

		private const int port = 4445;
		private string ip;
		private bool btimeout = false;

        
		private ConcurrentDictionary<int, float> receivedValues;

		public enum NetType
		{
			Robot, Remote
		}

		public enum ValueType
		{
			throttle = 1, turn = 2
		}

		public R2D2Networking(string ip)
		{
			netType = NetType.Remote;
			this.ip = ip;

		}
		public R2D2Networking()
		{
			netType = NetType.Robot;
		}

		public void Start()
		{
			receivedValues = new ConcurrentDictionary<int, float>();

			receivedValues.TryAdd((int)ValueType.throttle, 0);
			receivedValues.TryAdd((int)ValueType.turn, 0);
			switch (netType)
			{
				case NetType.Remote:
					Console.WriteLine("Attempting connection...");
					foreach (IPAddress a in Dns.GetHostEntry(ip).AddressList)
					{
						if (a.ToString().Contains(".")) // Make sure its ipv4
						{
							remote = new IPEndPoint(a, port);
							break;
						}
					}
                    client = new UdpClient();

					Console.WriteLine("Connected");
					connected = true;
					receiveThread = new Thread(new ThreadStart(Receive));
					receiveThread.Start();
					break;
				case NetType.Robot:
					if (client != null)
					{
						client.Close();
					}
					remote = new IPEndPoint(IPAddress.Any, port);
                    client = new UdpClient(remote);
                    Console.WriteLine("Waiting for remote...");
                    client.Receive(ref remote);
					Console.WriteLine("Connected to "+remote.Address.ToString());
					connected = true;
                    receiveThread = new Thread(new ThreadStart(Receive));
					receiveThread.Start();
                    timeout = new Thread(new ThreadStart(Timeout));
					timeout.Start();

					break;
				default:
					break;
			}
		}

		public void SendValue(ValueType t, float v)
		{
			byte[] packet = new byte[5];
			packet[0] = (byte)t;
			byte[] values = BitConverter.GetBytes(v);
			values.CopyTo(packet, 1);
            client.Send(packet,5,remote);
		}

		public float RecvValue(int i)
		{
			return receivedValues[i];
            
		}

        private void Receive()
        {
			
            while (connected)
            {
				byte[] b = new byte[5];
				try
				{
					b = client.Receive(ref remote);
					btimeout = false;
				}
				catch (Exception) { Console.WriteLine("Couldnt rec");}
					if (b.Length != 5)
                {
                    Console.WriteLine("ERROR: Packet format wrong!");
                    continue;
                }
				receivedValues[b[0]] = BitConverter.ToSingle(b, 1);
            }

        }

		private void Timeout()
		{
			for (;;)
			{
				Thread.Sleep(1000);
				if (btimeout)
				{
					connected = false;
					client.Close();
					break;
				}

				btimeout = true;
			}
		}

	}
}