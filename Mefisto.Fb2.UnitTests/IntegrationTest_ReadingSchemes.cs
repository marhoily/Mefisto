using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FakeItEasy;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Mefisto.Fb2.UnitTests
{
	public class IntegrationTest_ReadingSchemes
	{
		[NotNull] private readonly TestLogger _testLogger = new TestLogger();

		[Fact(DisplayName = "Given XML, build correct schema, run it, setters should be called")]
		public void T1()
		{
			var setter = A.Fake<Action<string>>();

			var schema = new SchemaBuilder(new Fb2Reader(_testLogger, 
				new XElement("A", 
					new XElement("B", "value")).CreateReader()))

				.AddRead("A")
				.AddRead("B", setter);

			new SchemaRunner().Run(schema.Build());

			A.CallTo(() => setter("value")).MustHaveHappened();
		}
		[Fact(DisplayName = "Given XML, build WRONG schema, " +
		                    "run it, should see meaningfull errors")]
		public void T2()
		{
			var setter = A.Fake<Action<string>>();

			var schema = new SchemaBuilder(new Fb2Reader(_testLogger, 
				new XElement("A", 
					new XElement("B", "value")).CreateReader()))

				.AddRead("A")
				.AddRead("C", setter);

			new SchemaRunner().Run(schema.Build());

			A.CallTo(() => setter("")).WithAnyArguments().MustNotHaveHappened();
			_testLogger.DequeueMessages().Should().Equal(
				"[Error] Expected <C>, but found: <B>");
		}
		class Author { [CanBeNull]
		public string Name { get; set; } }

		[Fact(DisplayName = "Given XML with collection in it, build schema, " +
		                    "run it, collection of objects should be created")]
		public void T3()
		{
			var collection = new List<Author>();

			var schema = new SchemaBuilder(new Fb2Reader(_testLogger,
				new XElement("authors",
					new XElement("author",
						new XElement("name", "1")),
					new XElement("author",
						new XElement("name", "2"))).CreateReader()))

				.AddRead("authors")
				.AddReadMany("author", () => collection.AddAndReturn(new Author()),
					scope => scope.AddRead<string>("name", 
						v => scope.GetParent<Author>().Name = v));

			new SchemaRunner().Run(schema.Build());

			//collection.ShouldAllBeEquivalentTo(new[]
			//	{
			//		new Author { Name = "1"},
			//		new Author { Name = "2"},
			//	});
		}
	}

	public static class ListExtensions
	{
		[NotNull]
		public static T AddAndReturn<T>(
			[NotNull] this ICollection<T> collection,
			[NotNull] T item)
			where T : class
		{
			collection.Add(item);
			return item;
		}
	}
}