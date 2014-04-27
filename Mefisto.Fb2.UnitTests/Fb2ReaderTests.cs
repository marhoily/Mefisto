using System;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	public class Fb2ReaderTests : IDisposable
	{
		[NotNull] private readonly TestLogger _testLogger;
		[NotNull] private readonly Action<string> _setter;
		public static readonly XNamespace Nsp = "nsp";

		public Fb2ReaderTests()
		{
			_testLogger = new TestLogger();
			_setter = A.Fake<Action<string>>();
		}

		public void Dispose()
		{
			_testLogger.Messages.Should().Equal(new object[0]);
		}

		[Fact]
		public void Read_Should_Return_True()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Nsp + "book").CreateReader());
			bookReader.ReadElement(Nsp + "book").Should().BeTrue();
		}

		[Fact]
		public void Read_Should_Allow_Tag_To_Be_Case_Insensitive()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Nsp + "Book").CreateReader());
			bookReader.ReadElement(Nsp + "book").Should().BeTrue();
		}
		[Fact]
		public void Read_When_Incorrect_Namespace_Should_Log_So_And_Return_False()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement("book").CreateReader());
			bookReader.ReadElement(Nsp + "book").Should().BeFalse();
			_testLogger.DequeueMessages().Should().Equal(
				"[Error] Expected <book xmlns=\"nsp\">, " +
				"but found: <book xmlns=\"\">");
		}

		[Fact]
		public void Read_When_Incorrect_Tag_Should_Log_So_And_Return_False()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Nsp + "wrong").CreateReader());
			bookReader.ReadElement(Nsp + "book").Should().BeFalse();
			_testLogger.DequeueMessages().Should().Equal(
				"[Error] Expected <book xmlns=\"nsp\">, " +
				"but found: <wrong xmlns=\"nsp\">");
		}

		[Fact]
		public void Read_Should_Read_Inner_Tag()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Nsp + "book",
					new XElement(Nsp + "genre", "sf_fantasy"))
						.CreateReader());

			bookReader.ReadElement(Nsp + "book").Should().BeTrue();
			bookReader.ReadElement(Nsp + "genre").Should().BeTrue();
		}
		[Fact]
		public void Read_When_TextElement_Should_Return_Text()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Nsp + "genre", "sf_fantasy")
					.CreateReader());

			bookReader.Read(Nsp + "genre", _setter).Should().BeTrue();
			A.CallTo(() => _setter("sf_fantasy")).MustHaveHappened();
		}
		[Fact]
		public void Read_String_When_Wrong_Tag_Name_Should_Return_False()
		{
			var bookReader = new Fb2Reader(_testLogger,
				new XElement(Nsp + "blah", "sf_fantasy")
					.CreateReader());

			bookReader.Read<string>("genre").Should().BeFalse();

			_testLogger.DequeueMessages().Should().NotBeEmpty();
			A.CallTo(() => _setter(""))
				.WithAnyArguments()
				.MustNotHaveHappened();
		}
	}
}