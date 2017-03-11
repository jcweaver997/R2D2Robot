using System;
using System.IO.Ports;

namespace R2D2Robot
{
	public class R2D2Robot
	{
		SerialPort sp;
		R2D2Networking netCom;
		private float throttle = 0, turn = 0;
		public R2D2Robot()
		{


		}
		public void Init()
		{

			try
			{
				sp = new SerialPort(SerialPort.GetPortNames()[1], 9600, Parity.None, 8, StopBits.One);

				sp.Open();
			}
			catch (Exception){
				Init();
				return;
			}

			netCom = new R2D2Networking();
			netCom.Start();
			for (;;)
			{
				Update();
				System.Threading.Thread.Sleep(20);
				if (!netCom.connected)
				{
					MassZero();
					netCom.Start();
				}
			}
		}
		private byte counter = 0;
		public void Update()
		{
			counter++;
			this.throttle = netCom.RecvValue((int)R2D2Networking.ValueType.throttle);
			this.turn = netCom.RecvValue((int)R2D2Networking.ValueType.turn);

			sp.WriteLine(1 + " " + (throttle + turn)); // left
			sp.WriteLine(2 + " " + (throttle - turn)); // right
			if (counter>=30)
			{
				counter = 0;
				Console.WriteLine(1 + " " + (throttle)); // left
				Console.WriteLine(2 + " " + turn); // right
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
