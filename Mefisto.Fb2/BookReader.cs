using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public class BookReader
	{
		[NotNull] private readonly IFb2Reader _reader;

		public BookReader([NotNull] IFb2Reader reader)
		{
			_reader = reader;
		}

		[CanBeNull]
		public Book Read()
		{
			var result = new Book();

			var schema = new SchemaBuilder()
				.AddRead(C.Mandatory, "FictionBook")
				.AddRead(C.Mandatory, "description")
				.AddRead(C.Mandatory, "title-info")
				.AddRead(C.MandatoryString, "genre", v => result.Genre = v);

			foreach (var func in schema.Build())
			{
				Debug.Assert(func != null, "func != null");
				if (!func()) break;
			}

			return result;
		}
	}

	public class Author
	{
	}

	public class SchemaBuilder
	{
		[NotNull]
		public SchemaBuilder AddRead<T>([NotNull] ISettings<T> settings, [NotNull] string name, [CanBeNull] Action<T> setter = null)
		{
			throw new NotImplementedException();
			//return this;
		}

		[NotNull]
		public IEnumerable<Func<bool>> Build()
		{
			throw new NotImplementedException();
		}
	}


	public static class C
	{
		[NotNull] public static readonly Mandatory<Unit> Mandatory = new Mandatory<Unit>();
		[NotNull] public static readonly Mandatory<string> MandatoryString = new Mandatory<string>();
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