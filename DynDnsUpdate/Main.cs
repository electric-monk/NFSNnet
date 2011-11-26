using System;
using NearlyFreeSpeechNet;

namespace DynDnsUpdate
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			if ((args.Length != 4) && (args.Length != 5))
			{
				System.Console.WriteLine("Usage: DynDnsUpdate <username> <api key> <hostname> <subdomain> [<IP address>]");
				return -2;
			}
			
			string username = args[0];
			string apiKey = args[1];
			string hostname = args[2];
			string subdomain = args[3];
			string externalIP = null;
			if (args.Length > 4)
				externalIP = args[4];
			
			try
			{
				Api api;
				Dns dns;
				Dns.Record[] records;
				bool found, required;
				
				// Configure the HTTPS stuff
				Api.ConfigureSelfSigned();
				
				// Get our IP
				if (externalIP == null)
				{
					try
					{
						externalIP = ExternIP.GetIP();
					}
					catch (Exception ex)
					{
						throw new Exception("Error getting our IP address!", ex);
					}
				}
				
				// Set up connection
				api = new Api(username, apiKey);
				dns = api.Dns(hostname);
				
				// Get a list of subdomains on the host
				records = dns.ListRRs(subdomain, "A", null);
				
				// Search list for matching subdomains
				found = false;
				required = false;
				foreach(Dns.Record record in records)
				{
					found = true;
					if (record.Data != externalIP)
					{
						required = true;
						dns.RemoveRR(record);
					}
				}
				
				// If necessary, create the record
				if (!found || required)
				{
					Dns.Record newRecord = new Dns.Record();
					newRecord.Name = subdomain;
					newRecord.Type = "A";
					newRecord.Data = externalIP;
					newRecord.TTL = 3600;
					dns.AddRR(newRecord);
				}
				
				return 0;
			}
			catch(Exception ex)
			{
				string error, type;
						
				error = "";
				type = "Exception";
				while (ex != null)
				{
					error += type + ": " + ex.ToString() + "\n";
					type = "Inner";
					ex = ex.InnerException;
				}
				System.Console.WriteLine(error);
				return -1;
			}
		}
	}
}
