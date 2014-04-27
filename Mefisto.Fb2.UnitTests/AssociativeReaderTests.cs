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
	public class AssociativeReaderTests : IDisposable
	{
		[NotNull] private readonly XmlReader _xmlReader;
		[NotNull] private readonly Action<XmlReader> _a;
		[NotNull] private readonly Action<XmlReader> _b;
		private TestLogger _testLogger = new TestLogger();

		public AssociativeReaderTests()
		{
			_xmlReader = new XElement("R",
				new XElement("B",
					new XElement("D")),
				new XElement("A"),
				new XElement("C")
				).CreateReader();

			_a = A.Fake<Action<XmlReader>>();
			_b = A.Fake<Action<XmlReader>>();

			A.CallTo(() => _a(_xmlReader)).Invokes(() => _xmlReader.Name.Should().Be("A"));
			A.CallTo(() => _b(_xmlReader)).Invokes(() => _xmlReader.Name.Should().Be("B"));
		}

		public void Dispose()
		{
			_testLogger.Messages.Should().Equal(new string[0]);
		}

		[Fact]
		public void Scan_Should_Fire_Associated_Callbacks()
		{
			var reader = new AssociativeReader(_testLogger, _xmlReader,
				new ScopeHandler("R",
					new ScopeHandler("A", _a),
					new ScopeHandler("B", _b)));

			reader.Scan();

			A.CallTo(() => _a(_xmlReader)).MustHaveHappened();
			A.CallTo(() => _b(_xmlReader)).MustHaveHappened();
		}

		[Fact]
		public void Lack_Of_Mandatory_Element_Should_Log_Errors()
		{
			var reader = new AssociativeReader(_testLogger, _xmlReader,
				new ScopeHandler("R",
					new ScopeHandler("D")
					{
						Mandatory = true,
					}));

			reader.Scan();

			_testLogger.DequeueMessages().Should().Equal("x");
		}
	}

	[DebuggerDisplay("{Name}: {SubDescriptors.Count}")]
	public class ScopeHandler
	{
		[NotNull] public XName Name { get; private set; }

		[CanBeNull]
		public Action<XmlReader> Handler { get; private set; }

		[NotNull]
		public Dictionary<XName, ScopeHandler> SubDescriptors { get; private set; }

		public bool Mandatory { get; set; }

		public ScopeHandler([NotNull] XName name, [NotNull] Action<XmlReader> handler)
			: this(name)
		{
			Handler = handler;
		}

		public ScopeHandler([NotNull] XName name, [NotNull] params ScopeHandler[] subScopeHandlers)
		{
			Name = name;
			SubDescriptors = subScopeHandlers.ToDictionary(x => x.Name, x => x);
		}
	}

	public class Scope
	{
		[NotNull]
		public ScopeHandler Handler { get; private set; }

		[NotNull]
		public Scope ParentScope { get; private set; }

		public Scope([NotNull] ScopeHandler handler)
		{
			Handler = handler;
		}

		public Scope([NotNull] Scope parentScope, [NotNull] ScopeHandler handler)
			: this(handler)
		{
			ParentScope = parentScope;
		}
	}

	public class AssociativeReader
	{
		private readonly ILogger _testLogger;
		private readonly XmlReader _reader;
		private readonly ScopeHandler _rootScopeHandler;
		private ScopeHandler _scopeHandler;
		private Scope _currentScope;

		public AssociativeReader(
			[NotNull] ILogger testLogger,
			[NotNull] XmlReader reader,
			[NotNull] ScopeHandler scopeHandler)
		{
			_testLogger = testLogger;
			_reader = reader;
			_rootScopeHandler = scopeHandler;
		}

		private int _virtualScopeCounter;
		public void Scan()
		{
			while (_reader.Read())
			{
				if (_reader.NodeType == XmlNodeType.EndElement)
				{
					if (_virtualScopeCounter > 0)
					{
						_virtualScopeCounter --;
						continue;
					}
					_scopeHandler = _currentScope.Handler;
					_currentScope = _currentScope.ParentScope;
					continue;
				}
				if (_virtualScopeCounter > 0)
				{
					if (!_reader.IsEmptyElement)
						_virtualScopeCounter++;
					continue;
				}
				XNamespace nsp = _reader.NamespaceURI;
				var name = nsp + _reader.Name;
				if (_scopeHandler == null)
				{
					Debug.Assert(name == _rootScopeHandler.Name);
					_scopeHandler = _rootScopeHandler;
					_currentScope = new Scope(_scopeHandler);
					continue;
				}
				ScopeHandler scopeHandler;
				if (!_scopeHandler.SubDescriptors.TryGetValue(name, out scopeHandler))
				{
					if (!_reader.IsEmptyElement)
						_virtualScopeCounter++;
					continue;
				}
				if (!_reader.IsEmptyElement)
				{
					_currentScope = new Scope(_currentScope, _scopeHandler);
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