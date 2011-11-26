using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace NearlyFreeSpeechNet
{
	public class Email : Service
	{
		/*
		 * According to the documentation and various posts online,
		 * we should be able to use a combination of a custom
		 * serializable class and the code:
		 * 			return Connection.Deserialize<EmailForwardArray>(result).Forwards;
		 * to generate an array. However, when I try it on mono
		 * it doesn't work - its guts appear to lack any support
		 * for ISerializable. Thus, in this case just randomly
		 * hook into System.Web.Extensions, which can do it for us.
		 
		[Serializable]
		private class EmailForwardArray : ISerializable
		{
			private Dictionary<string, string> m_forwards;
			
			public EmailForwardArray()
			{
				m_forwards = new Dictionary<string, string>();
			}
			
			protected EmailForwardArray(SerializationInfo info, StreamingContext context)
			{
				m_forwards = new Dictionary<string, string>();
				foreach (SerializationEntry entry in info)
					m_forwards.Add(entry.Name, entry.Value as string);
			}
			
			public Dictionary<string, string> Forwards
			{
				get
				{
					return m_forwards;
				}
			}
			
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext contect)
			{
				foreach (KeyValuePair<string, string> kvp in m_forwards)
					info.AddValue(kvp.Key, kvp.Value);
			}
		}
		*/

		internal Email (Connection connection, string domain)
			:base(connection, "email", domain)
		{
		}
		
		/// <summary>
		/// Lists the mail forwards configured for the domain represented by this object.
		/// </summary>
		/// <returns>
		/// A dictionary of mappings of forwarders and their target e-mail addresses.
		/// </returns>
		public Dictionary<string, string> ListForwards()
		{
			string result = Connection.Post(BaseUrl + "/listForwards", null);
			System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
			return ser.Deserialize<Dictionary<string, string>>(result);
		}
		
		/// <summary>
		/// Removes a mail forward on the domain represented by this object.
		/// </summary>
		/// <param name='forward'>
		/// The name of the forward to remove (e.g. 'cat' for 'cat@example.com')
		/// </param>
		public void RemoveForward(string forward)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("forward", forward);
			Connection.Post(BaseUrl + "/removeForward", parameters);
		}
		
		/// <summary>
		/// Adds a mail forward to the domain represented by this object.
		/// </summary>
		/// <param name='forward'>
		/// The name of the forward (e.g. 'cat' for 'cat@example.com')
		/// </param>
		/// <param name='destinationEmail'>
		/// The target e-mail address for the forward (e.g. 'dog@example.com')
		/// </param>
		public void SetForward(string forward, string destinationEmail)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("forward", forward);
			parameters.Add("dest_email", destinationEmail);
			Connection.Post(BaseUrl + "/setForward", parameters);
		}
		
		/// <summary>
		/// The domain name represented by this object.
		/// </summary>
		public string Domain
		{
			get
			{
				return InstanceId;
			}
		}
		
		public override string ToString()
		{
			return string.Format("[Email: Domain={0}]", Domain);
		}
	}
}
