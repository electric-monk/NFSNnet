using System;
using System.Net;
using System.Runtime.Serialization;

namespace NearlyFreeSpeechNet
{
	public class Account : Service
	{
		/// <summary>
		/// Represents the current status of a given account
		/// </summary>
		[DataContract]
		public class AccStatus
		{
			/// <summary>
			/// The long form of the status
			/// </summary>
			public string Description
			{
				get
				{
					return state;
				}
			}
			
			/// <summary>
			/// The short form of the status
			/// </summary>
			public string Status
			{
				get
				{
					return sstate;
				}
			}
			
			/// <summary>
			/// The colour to use for representing the given status to the user (as an HTML colour)
			/// </summary>
			public string Colour
			{
				get
				{
					return colour;
				}
			}
			
			[DataMember(Name="status")]
			private string state { get; set; }
			[DataMember(Name="short")]
			private string sstate { get; set; }
			[DataMember(Name="color")]
			private string colour { get; set; }
			
			public override string ToString ()
			{
				return string.Format("[AccStatus: Description={0}, Status={1}, Colour={2}]", Description, Status, Colour);
			}
		}
		
		internal Account(Connection connection, string accountNumber)
			:base(connection, "account", accountNumber)
		{
		}
		
		#region Properties
	
		/// <summary>
		/// The current balance on the account.
		/// </summary>
		public float Balance
		{
			get
			{
				return float.Parse(Parameter_Get("balance"));
			}
		}
		
		/// <summary>
		/// The current cash balance on the account.
		/// </summary>
		public float BalanceCash
		{
			get
			{
				return float.Parse(Parameter_Get("balanceCash"));
			}
		}
		
		/// <summary>
		/// The current credit balance on the account (such as transferred from another user, or other special situation)
		/// </summary>
		public float BalanceHigh
		{
			get
			{
				return float.Parse(Parameter_Get("balanceHigh"));
			}
		}
		
		/// <summary>
		/// Gets or sets the friendly name of the account.
		/// </summary>
		public string FriendlyName
		{
			get
			{
				return Parameter_Get("friendlyName");
			}
			set
			{
				Parameter_Set("friendlyName", value);
			}
		}
		
		/// <summary>
		/// Gets the current status of the account.
		/// </summary>
		public AccStatus Status
		{
			get
			{
				return Connection.Deserialize<AccStatus>(Parameter_Get("status"));
			}
		}
		
		/// <summary>
		/// Returns the sites on the account
		/// </summary>
		public string[] Sites
		{
			get
			{
				return Connection.Deserialize<string[]>(Parameter_Get("sites"));
			}
		}
		
		#endregion
		
		#region Functions
		
		/// <summary>
		/// Adds a site to the account
		/// </summary>
		/// <param name='shortName'>
		/// The short name for the new account
		/// </param>
		public void AddSite(string shortName)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("site", shortName);
			Connection.Post(BaseUrl + "/addSite", parameters);
		}
		
		/// <summary>
		/// Adds a balance warning to the account
		/// </summary>
		/// <param name='balance'>
		/// The balance at which the warning should be triggered
		/// </param>
		public void AddWarning(float balance)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("balance", balance.ToString("F2"));
			Connection.Post(BaseUrl + "/addWarning", parameters);
		}
		
		/// <summary>
		/// Removes a balance warning from the account
		/// </summary>
		/// <param name='balance'>
		/// The balance of the existing warning
		/// </param>
		public void RemoveWarning(float balance)
		{
			WebHeaderCollection parameters = new WebHeaderCollection();
			parameters.Add("balance", balance.ToString("F2"));
			Connection.Post(BaseUrl + "/removeWarning", parameters);
		}
		
		#endregion
		
		/// <summary>
		/// Gets the account number of this object
		/// </summary>
		public string AccountNumber
		{
			get
			{
				return InstanceId;
			}
		}
		
		public override string ToString()
		{
			return string.Format("[Account: AccountNumber={0}, Balance={1}, BalanceCash={2}, BalanceHigh={3}, FriendlyName={4}, Status={5}, Sites={6}]", AccountNumber, Balance, BalanceCash, BalanceHigh, FriendlyName, Status, Sites);
		} 
	}
}
