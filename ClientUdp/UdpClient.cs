using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUdp
{
	using System.Net;
	using System.Net.Sockets;

	public class UdpClient
	{
		private readonly Socket _clientSocket;
		private EndPoint _endPoint;
		private byte _handle = 0x1;

		public UdpClient(Socket socket)
		{
			_endPoint = new IPEndPoint(0,0);
			_clientSocket = socket;
			_clientSocket.ReceiveTimeout = 10000;
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			for (int i = 0; i < 5; i++)
			{
				var bytesRead = 0;
				try
				{
					bytesRead = _clientSocket.ReceiveFrom(buffer, offset,
						count, SocketFlags.None, ref _endPoint);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}

				byte[] bytes = Encoding.UTF8.GetBytes(NetworkConstants.Ask);
				_clientSocket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, _endPoint);


				return bytesRead;
			}

			return 0;
		}

		public bool Write(byte[] buffer, int offset, int count)
		{
			byte[] newArray = new byte[buffer.Length + 1];
			buffer.CopyTo(newArray, 1);
			_handle = (byte)((_handle << 1) | (_handle >> 7));
			newArray[0] = _handle;

			byte[] bytes = new byte[3];
			for (int i = 0; i < 5; i++)
			{
				_clientSocket.SendTo(newArray, 0, count + 1, SocketFlags.None, _endPoint);

				try
				{
					_clientSocket.ReceiveFrom(bytes, 0, bytes.Length, SocketFlags.None, ref _endPoint);

					if (Encoding.ASCII.GetString(bytes, 0, bytes.Length) != NetworkConstants.Ask)
					{
						continue;
					}
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}

				return true;
			}
			return false;
		}

		public void EndConnection()
		{
			if (_clientSocket != null)
			{
				_clientSocket.Shutdown(SocketShutdown.Both);
				_clientSocket.Close();
			}
		}

		public bool Connect(string ipAddress, int port)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(NetworkConstants.Con);

			Console.WriteLine(bytes.Length);

			IPEndPoint endPoint = new IPEndPoint(Dns.Resolve(ipAddress)
				.AddressList[0], port);

			_clientSocket.SendTo(bytes, 0, bytes.Length, SocketFlags.None, endPoint);

			bytes = new byte[4];

			for (int i = 0; i < 5; i++)
			{
				try
				{
					var bytesRead = _clientSocket.ReceiveFrom(bytes, ref _endPoint);
				}
				catch(Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}
				Console.WriteLine(BitConverter.ToInt32(bytes,0));
				return true;
			}

			return false;
		}
	}
}
