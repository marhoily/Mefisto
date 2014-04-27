using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	
	public class AssociativeReaderTests
	{
		[Fact]
		public void T1()
		{
			var xmlReader = new XElement("R",
				new XElement("A"),
				new XElement("B"),
				new XElement("C" +
				             "")
				).CreateReader();
			var reader = new AssociativeReader(new TestLogger(), xmlReader);
			var a = A.Fake<Action<XmlReader>>();
			var b = A.Fake<Action<XmlReader>>();
			reader.Set("A", a);
			reader.Set("B", b);

			reader.Scan();
			A.CallTo(() => a(xmlReader))
				.Invokes(_ => xmlReader.Name.Should().Be("A"))
				.MustHaveHappened();
			A.CallTo(() => b(xmlReader))
				.Invokes(_ => xmlReader.Name.Should().Be("B"))
				.MustHaveHappened();
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