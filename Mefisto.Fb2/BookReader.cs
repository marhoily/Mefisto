using System;
using System.Xml;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public class BookReader
	{
		private const string Fb2 = "http://www.gribuser.ru/xml/fictionbook/2.0";
		[NotNull]
		private readonly ILogger _logger;

		public BookReader([NotNull] ILogger logger)
		{
			_logger = logger;
		}

		[CanBeNull]
		public Book Read([NotNull] XmlReader reader)
		{
			reader.Read();
			if (string.Compare(reader.Name, "FictionBook", StringComparison.OrdinalIgnoreCase) != 0)
			{
				_logger.Error(string.Format(
					"Expected FictionBook, but found: '{0}'", reader.Name));
				return null;
			}
			if (reader.NamespaceURI != Fb2)
			{
				_logger.Error(string.Format(
					"Expected {{{{{0}}}}}:FictionBook, and the tag was allright but " +
					"found different namespace: '{1}'", Fb2, reader.NamespaceURI));
				return null;
			}
			return new Book();
		}
	}
}