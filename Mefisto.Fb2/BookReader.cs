using System;
using System.Xml;

namespace Mefisto.Fb2
{
	public class BookReader
	{
		private const string Fb2 = "http://www.gribuser.ru/xml/fictionbook/2.0";

		public Book Read(XmlReader reader)
		{
			reader.Read();
			if (reader.Name != "FictionBook")
				return null;
			if (reader.NamespaceURI != Fb2)
				return null;
			return new Book();
		}
	}

	public class Book
	{
	}
}