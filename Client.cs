using System.Net;
using System.Net.Sockets;

namespace lab14
{
	class Client
	{
		public Socket Socket { get; private set; }

		public Client(IPAddress ipAddress, IPEndPoint ipEndPoint)
		{
			Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			Socket.Connect(ipEndPoint);
		}

		~Client()
		{
			Socket.Shutdown(SocketShutdown.Both);
			Socket.Close();
		}

		// Отправка сообщения
		public byte[] Send(byte[] msg)
		{
			Socket.Send(msg);
			byte[] response = new byte[1024];
			Socket.Receive(response);
			return response;
		}
	}
}
