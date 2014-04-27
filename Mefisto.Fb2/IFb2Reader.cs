using System;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public interface IFb2Reader
	{
		bool ReadElement([NotNull] XName name);
		bool Read<T>([NotNull] XName name, [CanBeNull] Action<T> setter = null);
	}

	public interface ISettings<T>
	{
	}
}