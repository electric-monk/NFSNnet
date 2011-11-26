using System;
using System.Runtime.Serialization;
using System.Net;
using System.IO;
using System.Text;

namespace NearlyFreeSpeechNet
{
	public class Dns : Service
	{
		/// <summary>
		/// A DNS record stored in the zone file
		/// </summary>
		[DataContract]
		public class Record
		{
			[DataMember(Name="name")]
			public string Name { get; set; }
			[DataMember(Name="type")]
			public string Type { get; set; }
			[DataMember(Name="data")]
			public string Data { get; set; }
			public int TTL { get; set; }
			public string Scope
			{
				get
				{
					return ScopeInternal;
				}
			}
			[DataMember(Name="ttl")]
			private string TTLinternal
			{
				get
				{
					return TTL.ToString();
				}
				set
				{
					TTL = int.Parse(value);
				}
			}
			[DataMember(Name="scope")]
			private string ScopeInternal { get; set; }
			public override string ToString()
			{
				return string.Format("[Dns.Record: Name={0}, Type={1}, Data={2}, TTL={3}, Scope={4}]", Name, Type, Data, TTL, Scope);
			}
		}
		
		internal Dns(Connection connection, string hostname)
			:base(connection, "dns", hostname)
		{
		}
		
		#region Properties
		
		public int Expire
		{
			get
			{
				return int.Parse(Parameter_Get("expire"));
			}
			set
			{
				Parameter_Set("expire", value.ToString());
			}
		}
		
		public int MinTTL
		{
			get
			{
				return int.Parse(Parameter_Get("minTTL"));
			}
			set
			{
				Parameter_Set("minTTL", value.ToString());
			}
		}
		
		public int Refresh
		{
			get
			{
				return int.Parse(Parameter_Get("refresh"));
			}
			set
			{
				Parameter_Set("refresh", value.ToString());
			}
		}
		
		public int Retry
		{
			get
			{
				return int.Parse(Parameter_Get("retry"));
			}
			set
			{
				Parameter_Set("retry", value.ToString());
			}
		}
		
		public int Serial
		{
			get
			{
				return int.Parse(Parameter_Get("serial"));
			}
		}
		
		#endregion
		
		#region Functions
		
		/// <summary>
		/// Lists records on the domain represented by this object.
		/// </summary>
		/// <returns>
		/// Array of the records
		/// </returns>
		/// <param name='name'>
		/// Name to filter by, or null for no filter.
		/// </param>
		/// <param name='type'>
		/// Type to filter by, or null for no filter.
		/// </param>
		/// <param name='data'>
		/// Data to filter by, or null for no filter.
		/// </param>
		public Record[] ListRRs(string name, string type, string data)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			if (name != null)
				parameters.Add("name", name);
			if (type != null)
				parameters.Add("type", type);
			if (data != null)
				parameters.Add("data", data);
			string result = Connection.Post(BaseUrl + "/listRRs", parameters);
			return Connection.Deserialize<Record[]>(result);
		}
		
		/// <summary>
		/// Adds a record to the domain represented by this object.
		/// </summary>
		/// <param name='entry'>
		/// A Record object configured to represent the desired settings.
		/// </param>
		public void AddRR(Record entry)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("name", entry.Name);
			parameters.Add("type", entry.Type);
			parameters.Add("data", entry.Data);
			parameters.Add("ttl", entry.TTL.ToString());
			Connection.Post(BaseUrl + "/addRR", parameters);
		}
		
		/// <summary>
		/// Removes the given record from the domain represented by this object.
		/// </summary>
		/// <param name='entry'>
		/// The record to remove.
		/// </param>
		public void RemoveRR(Record entry)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("name", entry.Name);
			parameters.Add("type", entry.Type);
			parameters.Add("data", entry.Data);
			Connection.Post(BaseUrl + "/removeRR", parameters);
		}
		
		/// <summary>
		/// Updates the DNS server's serial number for the given domain
		/// </summary>
		public void UpdateSerial()
		{
			Connection.Post(BaseUrl + "/updateSerial", null);
		}
		
		#endregion
		
		/// <summary>
		/// Gets the hostname for this object.
		/// </summary>
		public string Hostname
		{
			get
			{
				return InstanceId;
			}
		}
		
		public override string ToString()
		{
			return string.Format("[Dns: Hostname={0}, Expire={1}, MinTTL={2}, Refresh={3}, Retry={4}, Serial={5}]", Hostname, Expire, MinTTL, Refresh, Retry, Serial);
		}
	}
}
