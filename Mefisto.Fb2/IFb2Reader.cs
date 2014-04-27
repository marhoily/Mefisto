using JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public interface IFb2Reader
	{
		bool ReadElement([NotNull] string name);
	}
}