using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Mefisto.Fb2.UnitTests
{
	public class TestLogger : ILogger
	{
		[NotNull]
		public Queue<string> Messages { get; private set; }

		public TestLogger()
		{
			Messages = new Queue<string>();
		}

		[NotNull]
		public IEnumerable<string> DequeueMessages()
		{
			return EnumerateMessages().ToList();
		}
		[NotNull]
		private IEnumerable<string> EnumerateMessages()
		{
			while (Messages.Count > 0)
				yield return Messages.Dequeue();
		}

		public void Error(string format, params object[] arguments)
		{
			Messages.Enqueue(string.Format("[Error] {0}",
				string.Format(format, arguments)));
		}
	}
}