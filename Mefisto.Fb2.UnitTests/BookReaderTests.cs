using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	public class BookReaderTests
	{
		[Fact]
		public void Read_When_Correct_Tag_Should_Return_Book()
		{
			var bookReader = new BookReader();
			var book = bookReader.Read(
				new XElement(Xmlns.Fb2 + "FictionBook").CreateReader());
			book.Should().NotBeNull();
		}
		[Fact]
		public void Read_When_Incorrect_Namespace_Should_Return_Null()
		{
			var bookReader = new BookReader();
			var book = bookReader.Read(
				new XElement("FictionBook").CreateReader());
			book.Should().BeNull();
		}
		[Fact]
		public void Read_When_Incorrect_Tag_Should_Return_Null()
		{
			var bookReader = new BookReader();
			var book = bookReader.Read(
				new XElement(Xmlns.Fb2 + "wrong").CreateReader());
			book.Should().BeNull();
		}
	}
}