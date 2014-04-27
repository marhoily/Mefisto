using System;
using System.Xml.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	public class Fb2ReaderTests : IDisposable
	{
		[NotNull] private readonly TestLogger _testLogger;

		public Fb2ReaderTests()
		{
			_testLogger = new TestLogger();
		}

		public void Dispose()
		{
			_testLogger.Messages.Should().BeEmpty();
		}

		[Fact]
		public void Read_Should_Return_True()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Xmlns.Fb2 + "FictionBook").CreateReader());
			bookReader.ReadElement("FictionBook").Should().BeTrue();
		}

		[Fact]
		public void Read_Should_Allow_Tag_To_Be_Case_Insensitive()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Xmlns.Fb2 + "Fictionbook").CreateReader());
			bookReader.ReadElement("FictionBook").Should().BeTrue();
		}

		[Fact]
		public void Read_When_Incorrect_Namespace_Should_Log_So_And_Return_False()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement("Fictionbook").CreateReader());
			bookReader.ReadElement("FictionBook").Should().BeFalse();
			_testLogger.DequeueMessages().Should().Equal(
				"[Error] Expected {http://www.gribuser.ru/xml/fictionbook/2.0}:FictionBook, " +
				"and the tag was allright but found different namespace: ''");
		}

		[Fact]
		public void Read_When_Incorrect_Tag_Should_Log_So_And_Return_False()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Xmlns.Fb2 + "wrong").CreateReader());
			bookReader.ReadElement("FictionBook").Should().BeFalse();
			_testLogger.DequeueMessages().Should().Equal(
				"[Error] Expected FictionBook, but found: 'wrong'");
		}
	}
}