using System;
using System.Xml;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public class BookReader
	{
		[NotNull]
		private readonly IFb2Reader _reader;

		public BookReader([NotNull] IFb2Reader reader)
		{
			_reader = reader;
		}

		[CanBeNull]
		public Book Read()
		{
			return _reader.ReadElement("FictionBook") ? new Book() : null;
		}
	}
}