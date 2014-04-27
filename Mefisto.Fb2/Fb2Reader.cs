using System;
using System.Xml;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public class Fb2Reader : IFb2Reader
	{
		private const string Fb2 = "http://www.gribuser.ru/xml/fictionbook/2.0";
		[NotNull] private readonly ILogger _logger;
		[NotNull] private readonly XmlReader _reader;

		public Fb2Reader([NotNull] ILogger logger, [NotNull] XmlReader reader)
		{
			_logger = logger;
			_reader = reader;
		}

		public bool ReadElement(string name)
		{
			_reader.Read();
			if (CorrectName(name) && CorrectNamespace()) return true;
			_logger.Error(string.Format(
				"Expected <{1} xmlns=\"{2}\">, but found: <{0} xmlns=\"{3}\">",
				_reader.Name, name, Fb2, _reader.NamespaceURI));
			return false;
		}

		private bool CorrectNamespace()
		{
			return _reader.NamespaceURI == Fb2;
		}

		private bool CorrectName([NotNull] string name)
		{
			return string.Compare(_reader.Name, name, StringComparison.OrdinalIgnoreCase) == 0;
		}
	}
}