using System;
using System.IO.Ports;

namespace R2D2Robot
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			R2D2Robot robot = new R2D2Robot();
			robot.Init();
		}
	}
}
