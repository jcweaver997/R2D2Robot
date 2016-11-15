using System;
using System.IO.Ports;

namespace R2D2Robot
{
	public class R2D2Robot
	{
		SerialPort sp;
		R2D2Networking netCom;

		public R2D2Robot()
		{


		}
		public void Init()
		{
			netCom = new R2D2Networking();
			netCom.Start();
			sp = new SerialPort("COM9", 9600, Parity.None, 8, StopBits.One);
			sp.Open();

			for (;;)
			{
				Update();
				if (!netCom.connected)
				{
					MassZero();
					netCom.Start();
				}
			}
		}

		public void Update()
		{
			R2D2Networking.ReturnValueType message = netCom.RecvValue();
			if (!message.isData)
			{
				return;
			}
			switch (message.valueType)
			{
				case R2D2Networking.ValueType.throttle:
					Console.WriteLine(((int)message.valueType) + " " + message.value);
					sp.WriteLine(((int)message.valueType)+" "+message.value);
					break;
				default:
					
					break;
			}
		}

		// For safety purposes
		public void MassZero()
		{
			foreach (R2D2Networking.ValueType v in Enum.GetValues(typeof(R2D2Networking.ValueType)))
			{
				sp.WriteLine(((int)v) + " " + 0);
			}
		}

	}
}
