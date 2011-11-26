using System;
using System.Net;

namespace NearlyFreeSpeechNet
{
	public class Site : Service
	{
		internal Site(Connection connection, string shortName)
			:base(connection, "site", shortName)
		{
		}
		
		/// <summary>
		/// Adds an alias to this site
		/// </summary>
		/// <param name='alias'>
		/// The alias to add (e.g. "www.example.com")
		/// </param>
		public void AddAlias(string alias)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("alias", alias);
			Connection.Post(BaseUrl + "/addAlias", parameters);
		}
		
		/// <summary>
		/// Removes an alias from this site
		/// </summary>
		/// <param name='alias'>
		/// The alias to remove.
		/// </param>
		public void RemoveAlias(string alias)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("alias", alias);
			Connection.Post(BaseUrl + "/removeAlias", parameters);
		}
		
		/// <summary>
		/// Gets the short name of the site represented by this object.
		/// </summary>
		public string ShortName
		{
			get
			{
				return InstanceId;
			}
		}
		
		public override string ToString()
		{
			return string.Format("[Site: ShortName={0}]", ShortName);
		}
	}
}
