//-----------------------------------------------------------------------
// <copyright file="AddCommentsToPost.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Database;
using Raven.Database.Data;
using Raven.Database.Json;
using Raven.Json.Linq;
using Raven.StackOverflow.Etl.Generic;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Raven.StackOverflow.Etl.Posts
{
	public class AddCommentsToPost : BatchFileWritingProcess
	{
		public AddCommentsToPost(string outputDirectory) : base(outputDirectory)
		{
		}

		public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
			int count = 0;
			foreach (var commentsForPosts in rows.Partition(Constants.BatchSize))
			{
				var cmds = new List<ICommandData>();

				foreach (var commentsForPost in commentsForPosts.GroupBy(row => row["PostId"]))
				{
					var comments = new RavenJArray();
					foreach (var row in commentsForPost)
					{
						comments.Add(new RavenJObject
						{
							{"Score", new RavenJValue(row["Score"])},
							{"CreationDate", new RavenJValue(row["CreationDate"])},
							{"Text", new RavenJValue(row["Text"])},
							{"UserId", new RavenJValue(row["UserId"])}
						});
							
					}
					cmds.Add(new PatchCommandData
					{
						Key = "posts/" + commentsForPost.Key,
						Patches = new[]
						{
							new PatchRequest
							{
								Name = "Comments",
								Type = PatchCommandType.Set,
								Value = comments
							},
						}
					});
				}

				count++;

				WriteCommandsTo("Comments #" + count.ToString("00000") + ".json", cmds);
			}

			yield break;
		}
	}
}
