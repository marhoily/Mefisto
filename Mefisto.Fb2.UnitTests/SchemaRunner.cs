using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Mefisto.Fb2.UnitTests
{
	public class SchemaRunner
	{
		public void Run([NotNull] IEnumerable<NotNull<Func<bool>>> schema)
		{
			foreach (var step in schema)
				if (!step.Value())
					break;
		}
	}
}