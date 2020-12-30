using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using lab5;
// ReSharper disable All

namespace lab14
{
	class Program
	{
		static readonly List<Action> Tasks = new List<Action>() {
			Task1,
			Task2,
			Task3,
			Task4,
		};

		static List<Transport> transport = new List<Transport> {
			new Ship("Корабль1",34.2,(30,40,50),20),
			new Ship("Корабль2",37,(30,55,40),26),
			new Steamer("Пароход1",35,(12,20,150),2015,70),
			new Steamer("Пароход2",25,(15,25,100),2009,40),
			new Sailboat("Парусник 1",20,(15,14,22),2011,787),
			new Sailboat("Парусник2",18,(10,15,25),2019,365),
			new Corvette("Корвет1",40,(30,40,115),30),
			new Corvette("Корвет1",35,(20,30,125),70)
		};

		public static void Menu()
		{
			while (true)
			{
				Console.Write(
					"1 - сериализация/десериализация объекта" +
					"\n2 - сериализацию/десериализацию коллекции" +
					"\n3 - селекторы для XML" +
					"\n4 - Linq to JSON" +
					"\n0 - выход" +
					"\nВыберите действие: "
					);
				if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > Tasks.Count)
				{
					Console.WriteLine("Нет такого действия");
					Console.ReadKey();
					Console.Clear();
					continue;
				}
				if (choice == 0)
				{
					Console.WriteLine("Выход...");
					Environment.Exit(0);
				}
				Tasks[choice - 1]();
				Console.ReadKey();
				Console.Clear();
			}
		}

		static void Task1()
		{
			if (Directory.Exists("task1"))
				Directory.Delete("task1", true);
			Directory.CreateDirectory("task1");

			// Сериализация
			CustomSerializer.Serialize(transport[6], Format.Binary, "task1/binary.bin");
			CustomSerializer.Serialize(transport[6], Format.Soap, "task1/soap.soap");
			CustomSerializer.Serialize(transport[6], Format.Json, "task1/json.json");
			CustomSerializer.Serialize(transport[6], Format.Xml, "task1/xml.xml");
			Console.WriteLine("Данные сериализованы и записаны");

			// Открытие созданных файлов
			Process.Start("transport", "task1\\binary.bin");
			Process.Start("transport", "task1\\soap.soap");
			Process.Start("transport", "task1\\json.json");
			Process.Start("transport", "task1\\xml.xml");

			Console.WriteLine("Десериализация:");
			var res = CustomSerializer.Deserialize<Steamer>(Format.Binary, "task1/binary.bin");
			Console.WriteLine("\nBinary");
			Print(res);
			res = CustomSerializer.Deserialize<Steamer>(Format.Soap, "task1/soap.soap");
			Console.WriteLine("\nSoap");
			Print(res);
			res = CustomSerializer.Deserialize<Steamer>(Format.Json, "task1/json.json");
			Console.WriteLine("\nJson");
			Print(res);
			res = CustomSerializer.Deserialize<Steamer>(Format.Xml, "task1/xml.xml");
			Console.WriteLine("\nXml");
			Print(res);

			static void Print(Steamer steamer)
			{
				Console.WriteLine(
					$"{steamer.Name}" +
					$"\nFullSpeed: {steamer.FullSpeed}" +
					$"\nDimensions: {steamer.Dimensions} " +
					$"\nSailingAuthonomy: {steamer.SailingAuthonomy}days" +
					$"\nCreationYear: {steamer.CreationYear}"
					);
			}
		}

		static void Task2()
		{
			// Создание нового сервера localhost:3030
			var server = new Server(3030);
			server.Start();
			// Создание нового клиента с ip сервера
			var client = new Client(server.IpAddress, server.IpEndPoint);

			// Отправка данных / получение ответа
			var data = client.Send(Encoding.UTF8.GetBytes("Дай данных"));
			// Десериализация ответа
			var collection = JsonConvert.DeserializeObject<List<string>>(Encoding.UTF8.GetString(data));

			Console.WriteLine("Полученные данные с сервера:");
			foreach (var item in collection)
				Console.WriteLine(item);

			server.Stop();
		}

		static void Task3()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load("task1/xml.xml");
			// Получения корня xml
			var root = doc.DocumentElement;
			// Получение первого узла с именем "Name"
			var name = root.SelectSingleNode("Name");
			// Получение первого узла с именем "Dimensions"
			var dimensions = root.SelectSingleNode("Dimensions");
			// Выбор всех узлов
			var dimensionsItems = dimensions.SelectNodes("*");
			Console.Write($"Название: {name.InnerText}\nГабариты: ");
			foreach (var item in dimensionsItems)
				Console.Write(((XmlNode)item).InnerText + " ");
		}

		static void Task4()
		{
			string json = JsonConvert.SerializeObject(transport);
			json = "{\"Data\": " + json + "}";
			using (var sw = new StreamWriter("task4.json"))
			{
				sw.Write(json);
			}
			// Для Linq + json
			var jParsed = JObject.Parse(json);

			// Транспорт с полной скоростью ниже заданной
			int fullspeed = 40;
			var fromSpeed = jParsed["Data"].Where(item => item["FullSpeed"].Value<int>() < fullspeed);
			Console.WriteLine("\nТранспорт с полной скоростью  ниже " + fullspeed);
			foreach (var item in fromSpeed)
				Console.WriteLine($"{item["Name"],-20}: {item["FullSpeed"]}$");

			// Транспорт заданной категории
			string category = "Sailboat";
			var fromCategory = jParsed["Data"].Where(item => item["Category"].Value<string>() == category);
			Console.WriteLine($"\nТранспорт {category}");
			foreach (var item in fromCategory)
				Console.WriteLine($"{item["Name"],-20}: {item["FullSpeed"]}$");

		}
	}
}
