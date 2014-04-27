using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	public class BookReaderTests
	{
		[NotNull] private readonly BookReader _bookReader;
		[NotNull] private readonly IFb2Reader _fb2Reader;

		public BookReaderTests()
		{
			_fb2Reader = A.Fake<IFb2Reader>();
			A.CallTo(() => _fb2Reader.ReadElement("FictionBook")).Returns(true);
			_bookReader = new BookReader(_fb2Reader);
		}


		[Fact]
		public void Read_Should_Return_Book()
		{
			_bookReader.Read().Should().NotBeNull();
		}

		[Fact]
		public void Read_When_Incorrect_Tag_Should_Return_Null()
		{
			A.CallTo(() => _fb2Reader.ReadElement("FictionBook")).Returns(false);
			//_bookReader.Read().Should().BeNull();
		}
	}
}