using System;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Mefisto.Fb2.Annotations.JetBrains.Annotations;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	public class BookReaderTests : IDisposable
	{
		[NotNull] private readonly BookReader _bookReader;
		[NotNull] private readonly TestLogger _testLogger;

		public BookReaderTests()
		{
			_testLogger = new TestLogger();
			_bookReader = new BookReader(_testLogger);
		}

		public void Dispose()
		{
			_testLogger.Messages.Should().BeEmpty();
		}

		[Fact]
		public void Read_When_Correct_Tag_Should_Return_Book()
		{
			var book = _bookReader.Read(
				new XElement(Xmlns.Fb2 + "FictionBook").CreateReader());
			book.Should().NotBeNull();
		}

		[Fact]
		public void Read_When_Incorrect_Namespace_Should_Log_So_And_Return_Null()
		{
			var book = _bookReader.Read(
				new XElement("FictionBook").CreateReader());
			book.Should().BeNull();
			_testLogger.DequeueMessages().ToList().Should().Equal(
				"[Error] Expected {http://www.gribuser.ru/xml/fictionbook/2.0}:FictionBook, " +
				"and the tag was allright but found different namespace: ''");
		}

		[Fact]
		public void Read_When_Incorrect_Tag_Should_Log_So_And_Return_Null()
		{
			var book = _bookReader.Read(
				new XElement(Xmlns.Fb2 + "wrong").CreateReader());
			book.Should().BeNull();
			_testLogger.DequeueMessages().ToList().Should().Equal(
				"[Error] Expected FictionBook, but found: 'wrong'");
		}
	}
}