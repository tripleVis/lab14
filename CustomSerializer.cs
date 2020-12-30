using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text.Json;
using System.Xml.Serialization;

namespace lab14
{
	static class CustomSerializer
	{
		// Сериализация объекта различными форматами в файл
		public static void Serialize(object item, Format format, string file)
		{
			var fileStream = new FileStream(file, FileMode.Create);
			using var sw = new StreamWriter(fileStream);
			switch (format)
			{
				case Format.Binary:
					new BinaryFormatter().Serialize(fileStream, item);
					break;
				case Format.Soap:
					new SoapFormatter().Serialize(fileStream, item);
					break;
				case Format.Json:
					sw.Write(JsonSerializer.Serialize(item, item.GetType()));
					break;
				case Format.Xml:
					new XmlSerializer(item.GetType()).Serialize(sw, item);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		// Десериализация данных из файла
		public static T Deserialize<T>(Format format, string file)
		{
			var fileStream = new FileStream(file, FileMode.Open);
			using var sr = new StreamReader(fileStream);
			return format switch
			{
				Format.Binary =>
					(T)new BinaryFormatter().Deserialize(fileStream),
				Format.Soap =>
					(T)new SoapFormatter().Deserialize(fileStream),
				Format.Json =>
				   JsonSerializer.Deserialize<T>(sr.ReadToEnd()),
				Format.Xml =>
					(T)new XmlSerializer(typeof(T)).Deserialize(fileStream),
				_ => throw new NotImplementedException()
			};
		}
	}

	// Форматы сериализации
	enum Format
	{
		Binary,
		Soap,
		Json,
		Xml
	}
}
