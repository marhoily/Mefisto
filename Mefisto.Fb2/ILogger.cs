﻿using Mefisto.Fb2.Annotations.JetBrains.Annotations;

namespace Mefisto.Fb2
{
	public interface ILogger
	{
		void Error([NotNull] string format, [NotNull] params object[] arguments);
	}
}