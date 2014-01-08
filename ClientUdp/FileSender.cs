using System;
using System.Text;

namespace ClientUdp
{
	using System.IO;
	using System.Net.Sockets;

	class FileSender
	{
		private UdpClient _udpClient;
		private string _fileName;

		public FileSender(string ipAddress, int port, string fileName)
		{
			_fileName = fileName;

			_udpClient = new UdpClient(new Socket(AddressFamily.InterNetwork,
				SocketType.Dgram, ProtocolType.Udp));

			if (!_udpClient.Connect(ipAddress, port))
			{
				_udpClient.EndConnection();
				return;
			}

			SendingFileToServer();

			_udpClient.EndConnection();
		}


		private void SendingFileToServer()
		{
			var file = new FileStream(_fileName, FileMode.Open, FileAccess.Read);

			int offset;
			
			var byteRead = 0;

			var message = Encoding.UTF8.GetBytes(_fileName);

			if (!_udpClient.Write(message, 0, message.Length))
			{
				return;
			}


			message = new Byte[4096];

			try
			{
				byteRead = _udpClient.Read(message, 0, message.Length);
			}
			catch
			{
				return;
			}

			if (byteRead == 0)
			{
				return;
			}

			byte[] tempArray = new byte[byteRead];
			Array.Copy(message, tempArray, byteRead);

			offset = BitConverter.ToInt32(tempArray, 0);

			Console.WriteLine(offset);

			file.Seek(offset, SeekOrigin.Begin);

			while (file.Position != file.Length)
			{
				byteRead = file.Read(message, 0, message.Length);

				try
				{
					if (!_udpClient.Write(message, 0, byteRead))
					{
						break;
					}
				}
				catch
				{
					break;
				}
			}
		}
	}
}
