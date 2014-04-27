using System;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public interface IFb2Reader
	{
		bool ReadElement([NotNull] string name);
		bool Read<T>([NotNull] string name, [CanBeNull] Action<T> setter = null);
	}

	public interface ISettings<T>
	{
	}
}