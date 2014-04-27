using System;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public class Fb2Reader : IFb2Reader
	{
		//private const string Fb2 = "http://www.gribuser.ru/xml/fictionbook/2.0";
		[NotNull] private readonly ILogger _logger;
		[NotNull] private readonly XmlReader _reader;

		public Fb2Reader([NotNull] ILogger logger, [NotNull] XmlReader reader)
		{
			_logger = logger;
			_reader = reader;
		}

		public bool ReadElement(XName name)
		{
			return ReadAndVerifyName(name);
		}

		private bool ReadAndVerifyName([NotNull] XName name)
		{
			_reader.Read();
			if (CorrectName(name) && CorrectNamespace(name)) return true;
			_logger.Error(string.Format(
				"Expected <{1} xmlns=\"{2}\">, but found: <{0} xmlns=\"{3}\">",
				_reader.Name, name.LocalName, name.NamespaceName, _reader.NamespaceURI));
			return false;
		}

		private bool CorrectNamespace([NotNull] XName name)
		{
			return _reader.NamespaceURI == name.NamespaceName;
		}

		private bool CorrectName([NotNull] XName name)
		{
			return string.Compare(_reader.Name, name.LocalName, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public bool Read<T>(XName name, Action<T> setter = null)
		{
			if (!ReadAndVerifyName(name)) 
				return false;

			_reader.Read();

			if (_reader.NodeType != XmlNodeType.Text)
				return false;

			if (setter != null) 
				setter((T)(object)_reader.Value);

			return true;
		}
	}
}