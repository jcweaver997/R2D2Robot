using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace R2D2Robot
{
	class R2D2Networking
	{
		NetType netType;
		TcpClient remote;
		TcpListener robot;

		NetworkStream com;

		private const int port = 4445;
		private string ip;

		public enum NetType
		{
			Robot, Remote
		}

		public enum ValueType
		{
			throttle = 1
		}

		public struct ReturnValueType
		{
			public bool isData;
			public ValueType valueType;
			public float value;
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
			switch (netType)
			{
				case NetType.Remote:
					Console.WriteLine("Attempting connection...");
					remote = new TcpClient(ip, port);

					Console.WriteLine("Connected");
					com = remote.GetStream();
					break;
				case NetType.Robot:

					robot = new TcpListener(System.Net.IPAddress.Any, port);
					robot.Start();
					Console.WriteLine("Waiting for remote...");
					remote = robot.AcceptTcpClient();
					com = remote.GetStream();
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
			com.Write(packet, 0, 5);
			Console.WriteLine("sent 5");
		}

		public ReturnValueType RecvValue()
		{
			ReturnValueType ret = new ReturnValueType();
			if (!com.DataAvailable)
			{
				ret.isData = false;
				return ret;
			}
			ret.isData = true;
			byte[] buffer = new byte[5];
			int numberRead = 0;
			while (numberRead < 5)
			{
				numberRead += com.Read(buffer, numberRead, 5 - numberRead);
			}

			ret.valueType = (ValueType)buffer[0];
			ret.value = BitConverter.ToSingle(buffer, 1);
			return ret;
		}

		public bool HasData()
		{
			return com.DataAvailable;
		}

	}
}