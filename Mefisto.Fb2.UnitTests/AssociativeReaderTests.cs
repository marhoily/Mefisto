using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
		[NotNull] private readonly XmlReader _xmlReader;
		[NotNull] private readonly Action<XmlReader> _a;
		[NotNull] private readonly Action<XmlReader> _b;

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
		}

		[Fact]
		public void Scan_Should_Fire_Associated_Callbacks()
		{
			var reader = new AssociativeReader(new TestLogger(), _xmlReader,
				new ScopeHandler("R",
					new ScopeHandler("A", _a),
					new ScopeHandler("B", _b)));

			reader.Scan();

			A.CallTo(() => _a(_xmlReader)).MustHaveHappened();
			A.CallTo(() => _b(_xmlReader)).MustHaveHappened();
		}
	}

	[DebuggerDisplay("{_name}: {SubDescriptors.Count}")]
	public class ScopeHandler
	{
		[NotNull] private readonly XName _name;

		[CanBeNull] public Action<XmlReader> Handler { get; private set; }

		[NotNull] public Dictionary<XName, ScopeHandler> SubDescriptors { get; private set; }

		public ScopeHandler([NotNull] XName name, [NotNull] Action<XmlReader> handler)
			: this(name)
		{
			Handler = handler;
		}

		public ScopeHandler([NotNull] XName name, [NotNull] params ScopeHandler[] subScopeHandlers)
		{
			_name = name;
			SubDescriptors = subScopeHandlers.ToDictionary(x => x._name, x => x);
		}
	}

	public class AssociativeReader
	{
		private readonly ILogger _testLogger;
		private readonly XmlReader _reader;
		private ScopeHandler _scopeHandler;

		public AssociativeReader(
			[NotNull] ILogger testLogger,
			[NotNull] XmlReader reader,
			[NotNull] ScopeHandler scopeHandler)
		{
			_testLogger = testLogger;
			_reader = reader;
			_scopeHandler = scopeHandler;
		}


		public void Scan()
		{
			while (_reader.Read())
			{
				XNamespace nsp = _reader.NamespaceURI;
				var name = nsp + _reader.Name;
				ScopeHandler scopeHandler;
				if (!_scopeHandler.SubDescriptors.TryGetValue(name, out scopeHandler)) continue;
				if (!_reader.IsEmptyElement)
				{
					_scopeHandler = scopeHandler;
				}
				if (scopeHandler.Handler != null)
				{
					scopeHandler.Handler(_reader);
				}
			}
		}
	}
}