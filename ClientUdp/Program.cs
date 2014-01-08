namespace ClientUdp
{
	using System;

	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				FileSender fileSender = new FileSender(args[0], int.Parse(args[1]), args[2]);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
