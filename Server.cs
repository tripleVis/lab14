using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Newtonsoft.Json;

namespace lab14
{
	class Server
	{
		public Socket Socket { get; private set; }
		public IPAddress IpAddress { get; private set; }
		public IPEndPoint IpEndPoint { get; private set; }
		public bool IsLive { get; private set; }

		private Thread ServerThread { get; set; }

		private readonly List<string> _data = new List<string>() {
			"Строка 1",
			"Строка 2",
			"Строка 3",
			"И другие данные"
		};

		public Server(int port)
		{
			// Получение ip адреса localhost
			IpAddress = Dns.GetHostEntry("localhost").AddressList[0];
			IpEndPoint = new IPEndPoint(IpAddress, port);
			Socket = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			Socket.Bind(IpEndPoint);
		}

		// Начало прослушивания
		public void Start()
		{
			IsLive = true;
			ServerThread = new Thread(StartThread);
			ServerThread.Start();
		}

		// Остановка
		public void Stop()
		{
			IsLive = false;
			try
			{
				ServerThread.Abort();
				Socket.Close();
			}
			catch { }
		}

		private void StartThread()
		{
			try
			{
				// Прослушивание сообщения
				Socket.Listen(5);

				// Ответ, если сервер онлайн
				while (IsLive)
				{
					Socket handler = Socket.Accept();
					var data = JsonConvert.SerializeObject(_data);
					var bytes = Encoding.UTF8.GetBytes(data);
					handler.Send(bytes);
				}
			}
			catch { }
		}
	}
}