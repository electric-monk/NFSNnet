using System;

namespace NearlyFreeSpeechNet
{
	public class Member : Service
	{
		internal Member (Connection connection, string username)
			:base(connection, "member", username)
		{
		}
		
		/// <summary>
		/// Retrieves the list of account numbers belonging to this member.
		/// </summary>
		public string[] Accounts
		{
			get
			{
				return Connection.Deserialize<string[]>(Parameter_Get("accounts"));
			}
		}
		
		/// <summary>
		/// Retrieves the list of sites belonging to this member.
		/// </summary>
		public string[] Sites
		{
			get
			{
				return Connection.Deserialize<string[]>(Parameter_Get("sites"));
			}
		}
		
		/// <summary>
		/// The member's username represented by this object.
		/// </summary>
		public string Username
		{
			get
			{
				return InstanceId;
			}
		}
		
		public override string ToString ()
		{
			return string.Format("[Member: Username={0}, Accounts={1}, Sites={2}]", Username, Accounts, Sites);
		}
	}
}
