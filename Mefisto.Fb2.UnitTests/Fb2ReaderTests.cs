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
			_testLogger.Messages.Should().Equal(new object[0]);
		}

		[Fact]
		public void Read_Should_Return_True()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Xmlns.Fb2 + "book").CreateReader());
			bookReader.ReadElement("book").Should().BeTrue();
		}

		[Fact]
		public void Read_Should_Allow_Tag_To_Be_Case_Insensitive()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Xmlns.Fb2 + "Book").CreateReader());
			bookReader.ReadElement("book").Should().BeTrue();
		}
		[Fact]
		public void Read_When_Incorrect_Namespace_Should_Log_So_And_Return_False()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement("book").CreateReader());
			bookReader.ReadElement("book").Should().BeFalse();
			_testLogger.DequeueMessages().Should().Equal(
				"[Error] Expected <book xmlns=\"http://www.gribuser.ru/xml/fictionbook/2.0\">, " +
				"but found: <book xmlns=\"\">");
		}

		[Fact]
		public void Read_When_Incorrect_Tag_Should_Log_So_And_Return_False()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Xmlns.Fb2 + "wrong").CreateReader());
			bookReader.ReadElement("book").Should().BeFalse();
			_testLogger.DequeueMessages().Should().Equal(
				"[Error] Expected <book xmlns=\"http://www.gribuser.ru/xml/fictionbook/2.0\">, " +
				"but found: <wrong xmlns=\"http://www.gribuser.ru/xml/fictionbook/2.0\">");
		}

		[Fact]
		public void Read_Should_Read_Inner_Tag()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Xmlns.Fb2 + "book",
					new XElement(Xmlns.Fb2 + "genre", "sf_fantasy"))
						.CreateReader());

			bookReader.ReadElement("book").Should().BeTrue();
			bookReader.ReadElement("genre").Should().BeTrue();
		}
	}
}