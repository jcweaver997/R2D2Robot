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
			sp = new SerialPort("COM9", 115200, Parity.None, 8, StopBits.One);
			sp.Open();

			for (;;)
			{
				Update();
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
					Console.WriteLine(message.value);
					sp.WriteLine(((int)message.valueType)+" "+message.value);
					break;
				default:
					
					break;
			}
		}

	}
}
