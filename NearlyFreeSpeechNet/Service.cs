using System;

namespace NearlyFreeSpeechNet
{
	/// <summary>
	/// A base class for all the services offered by the API,
	/// providing basic functionality in the form of parameter
	/// setting and getting and management of the base URL.
	/// </summary>
	public abstract class Service
	{
		private Connection m_connection;
		private string m_type;
		private string m_instanceId;
		
		internal Service(Connection connection, string type, string instanceId)
		{
			m_connection = connection;
			m_type = type;
			m_instanceId = instanceId;
		}
		
		protected string InstanceId
		{
			get
			{
				return m_instanceId;
			}
		}
		
		internal Connection Connection
		{
			get
			{
				return m_connection;
			}
		}
		
		protected string BaseUrl
		{
			get
			{
				return string.Format("/{0}/{1}", m_type, m_instanceId);
			}
		}
		
		protected string Parameter_Get(string name)
		{
			return Connection.Get(BaseUrl + "/" + name);
		}
		
		protected void Parameter_Set(string name, string data)
		{
			Connection.Put(BaseUrl + "/" + name, data);
		}
	}
}
