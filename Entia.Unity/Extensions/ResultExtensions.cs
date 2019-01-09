using Entia.Core;
using System;
using System.Linq;
using UnityEngine;

namespace Entia.Unity
{
	public static class ResultExtensions
	{
		public static Result<T> Log<T>(this Result<T> result)
		{
			if (result.TryMessages(out var messages) && messages.Length > 0)
				Debug.LogError(string.Join(Environment.NewLine, messages.Select(message => "-> " + message)));

			return result;
		}
	}
}
