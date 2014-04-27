using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using FakeItEasy;
using FakeItEasy.Core;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	
	public class AssociativeReaderTests
	{
		[NotNull] private readonly XmlReader _xmlReader;
		[NotNull] private readonly Action<XmlReader> _a;
		[NotNull] private readonly Action<XmlReader> _b;
		[NotNull] private readonly AssociativeReader _reader;

		public AssociativeReaderTests()
		{
			_xmlReader = new XElement("R",
				new XElement("A"),
				new XElement("B"),
				new XElement("C")
				).CreateReader();

			_a = A.Fake<Action<XmlReader>>();
			_b = A.Fake<Action<XmlReader>>();

			A.CallTo(() => _a(_xmlReader)).Invokes(() => _xmlReader.Name.Should().Be("A"));
			A.CallTo(() => _b(_xmlReader)).Invokes(() => _xmlReader.Name.Should().Be("B"));
			
			_reader = new AssociativeReader(new TestLogger(), _xmlReader);
		}

		[Fact]
		public void Scan_Should_Fire_Associated_Callbacks()
		{
			_reader.Set("A", _a);
			_reader.Set("B", _b);

			_reader.Scan();

			A.CallTo(() => _a(_xmlReader)).MustHaveHappened();
			A.CallTo(() => _b(_xmlReader)).MustHaveHappened();
		}
	}

	public class AssociativeReader
	{
		private readonly ILogger _testLogger;
		private readonly XmlReader _reader;
		private readonly Dictionary<XName, Action<XmlReader>> 
			_actions = new Dictionary<XName, Action<XmlReader>>();
		public AssociativeReader([NotNull] ILogger testLogger, [NotNull] XmlReader reader)
		{
			_testLogger = testLogger;
			_reader = reader;
		}

		public void Set([NotNull] XName tagName, [NotNull] Action<XmlReader> action)
		{
			_actions[tagName] = action;
		}

		public void Scan()
		{
			while (_reader.Read())
			{
				XNamespace nsp = _reader.NamespaceURI;
				var name = nsp + _reader.Name;
				Action<XmlReader> action;
				if (_actions.TryGetValue(name, out action))
					action(_reader);
			}
		}
	}
}