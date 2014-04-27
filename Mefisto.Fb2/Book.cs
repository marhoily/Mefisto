using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public class Book
	{
		[CanBeNull] public string Genre { get; set; }
		[NotNull] public List<Author> Authors { get; set; }
	}
}