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
			if (string.Compare(_reader.Name, name, StringComparison.OrdinalIgnoreCase) != 0)
			{
				_logger.Error(string.Format(
					"Expected {1}, but found: '{0}'", _reader.Name, name));
				return false;
			}
			if (_reader.NamespaceURI != Fb2)
			{
				_logger.Error(string.Format(
					"Expected {{{{{0}}}}}:{2}, and the tag was allright but " +
					"found different namespace: '{1}'", Fb2, _reader.NamespaceURI, name));
				return false;
			}
			return true;
		}
	}
}