﻿// -----------------------------------------------------------------------
//  <copyright file="DeleteDocumentsCommand.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Commands;
using Raven.Studio.Features.Documents;
using Raven.Studio.Features.Input;
using Raven.Studio.Infrastructure;
using Raven.Studio.Messages;
using Raven.Studio.Models;

namespace Raven.Studio.Commands
{
	public class DeleteDocumentsCommand : ListBoxCommand<ViewableDocument>
	{
		public override void Execute(object parameter)
		{
			var documentsIds = Items
				.Select(x => x.Id)
				.ToList();

			AskUser.ConfirmationAsync("Confirm Delete", documentsIds.Count > 1
										? string.Format("Are you sure you want to delete these {0} documents?", documentsIds.Count)
										: string.Format("Are you sure that you want to delete this document? ({0})", documentsIds.First()))
				.ContinueWhenTrue(() => DeleteDocuments(documentsIds))
				.ContinueWhenTrueInTheUIThread(() =>
									{
										var model = (DocumentsModel)Context;
										foreach (var document in Items)
										{
											model.Documents.Remove(document);
										}
									});
		}

		private void DeleteDocuments(IList<string> documentIds)
		{
			var deleteCommandDatas = documentIds.Select(id => new DeleteCommandData
			                                                  {
			                                                  	Key = id
			                                                  }).ToArray();
			ApplicationModel.Current.Server.Value.SelectedDatabase.Value.AsyncDatabaseCommands.BatchAsync(deleteCommandDatas)
				.ContinueOnSuccessInTheUIThread(() =>
				                   {
				                   	View.UpdateAllFromServer();
				                   	ApplicationModel.Current.AddNotification(new Notification(documentIds.Count > 1
				                   	                                                          	? string.Format(
				                   	                                                          		"{0} documents were deleted",
				                   	                                                          		documentIds.Count)
				                   	                                                          	: string.Format(
				                   	                                                          		"Document {0} was deleted",
				                   	                                                          		documentIds.First())));
				                   });
		}
	}
}