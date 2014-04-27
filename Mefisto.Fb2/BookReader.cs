using System;
using System.Collections.Generic;
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
			var result = new Book();

			var schema = new SchemaBuilder(_reader)
				.AddRead("FictionBook")
				.AddRead("description")
				.AddRead("title-info")
				.AddRead<string>("genre", v => result.Genre = v);

			foreach (var step in schema.Build())
				if (!step.Value()) 
					break;

			return result;
		}
	}

	public class Author
	{
	}

	public struct NotNull<T> where T : class
	{
		[NotNull]
		public T Value { get; private set; }

		public NotNull([NotNull] T value)
			: this()
		{
			Value = value;
		}

		public static implicit operator NotNull<T>([NotNull] T value)
		{
			return new NotNull<T>(value);
		}
	}

	public class SchemaBuilder
	{
		[NotNull]
		private readonly IFb2Reader _reader;
		[NotNull]
		private readonly List<NotNull<Func<bool>>> _readers;

		public SchemaBuilder([NotNull] IFb2Reader reader)
		{
			_reader = reader;
			_readers = new List<NotNull<Func<bool>>>();
		}

		[NotNull]
		public SchemaBuilder AddRead([NotNull] string name)
		{
			_readers.Add(new NotNull<Func<bool>>(() => _reader.ReadElement(name)));
			return this;
		}
		[NotNull]
		public SchemaBuilder AddRead<T>([NotNull] string name, [NotNull] Action<T> setter)
		{
			_readers.Add(new NotNull<Func<bool>>(() => _reader.Read(name, setter)));
			return this;
		}

		[NotNull]
		public IEnumerable<NotNull<Func<bool>>> Build()
		{
			return _readers;
		}
	}


	public static class C
	{
		[NotNull]
		public static readonly Mandatory<Unit> Mandatory = new Mandatory<Unit>();
		[NotNull]
		public static readonly Mandatory<string> MandatoryString = new Mandatory<string>();
		public static ISettings<Unit> Optional { get; set; }

		public static ISettings<T> Bind<T>(Action<T> action)
		{
			throw new NotImplementedException();
		}
	}

	public struct Unit
	{
	}

	public class Mandatory<T> : ISettings<T>
	{
	}
}