using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Net;
using System.Linq;

namespace NearlyFreeSpeechNet
{
	public class Api
	{
		private static string GetHashFile()
		{
			// Work out a place to store the hash
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NSFNhash");
		}
		
		private static byte[] LoadHash()
		{
			try
			{
				// Attempt to load the hash
				using (FileStream file = File.OpenRead(GetHashFile()))
				{
					using (BinaryReader reader = new BinaryReader(file))
					{
						return reader.ReadBytes((int)file.Length);
					}
				}
			}
			catch
			{
				return null;
			}
		}
		
		private static void SaveHash(byte[] hash)
		{
			// Simply save the hash
			using (FileStream file = File.Create(GetHashFile()))
			{
				file.Write(hash, 0, hash.Length);
			}
		}
		
		private static bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
		{
			// First check if it's a good certificate in the first place
			if (errors == SslPolicyErrors.None)
				return true;
			
			// Check if it's the expected NearlyFreeSpeech.Net one
			byte[] goodHash = LoadHash();
			if (goodHash == null)
			{
				SaveHash(cert.GetCertHash());
				return true;
			}
			else
			{
				return goodHash.SequenceEqual(cert.GetCertHash());
			}
		}
		
		/// <summary>
		/// This function can be called to set up a mechanism to deal with the NearlyFreeSpeech.Net API's
		/// self-signed certificate. Once called, the code will cache the hash of the first certificate
		/// it sees, and thereafter will compare the given certificate to the cached one.
		/// 
		/// Note that this is a security issue, and won't play nicely with other self-signed servers.
		/// </summary>
		public static void ConfigureSelfSigned()
		{
			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
		}

		private Connection m_connection;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="NearlyFreeSpeechNet.Api"/> class.
		/// </summary>
		/// <param name='login'>
		/// Login.
		/// </param>
		/// <param name='apikey'>
		/// Apikey.
		/// </param>
		public Api(string login, string apikey)
		{
			m_connection = new Connection(login, apikey);
		}
		
		/// <summary>
		/// Returns an object for manipulating the specified account.
		/// </summary>
		/// <param name='accountNumber'>
		/// The account number of the account to manipulate, which
		/// can be found via the Member object.
		/// </param>
		public Account Account(string accountNumber)
		{
			return new Account(m_connection, accountNumber);
		}
		
		/// <summary>
		/// Returns an object for manipulating the specified domain.
		/// </summary>
		/// <param name='hostname'>
		/// The hostname (e.g. example.com) to manipulate.
		/// </param>
		public Dns Dns(string hostname)
		{
			return new Dns(m_connection, hostname);
		}
		
		/// <summary>
		/// Returns an object for manipulating the e-mail settings on
		/// a domain.
		/// </summary>
		/// <param name='domain'>
		/// The domain (e.g. example.com) to manipulate.
		/// </param>
		public Email Email(string domain)
		{
			return new Email(m_connection, domain);
		}
		
		/// <summary>
		/// Returns an object for manipulating a member's account.
		/// </summary>
		/// <param name='username'>
		/// The member's username. This can be the same username
		/// passed as the login to the constructor of <see cref="NearlyFreeSpeechNet.Api"/>.
		/// </param>
		public Member Member(string username)
		{
			return new Member(m_connection, username);
		}
		
		/// <summary>
		/// Returns an object for manipulating a site.
		/// </summary>
		/// <param name='shortName'>
		/// The site's short name, as shown in the NFSN interface or
		/// returned by the Member object.
		/// </param>
		public Site Site(string shortName)
		{
			return new Site(m_connection, shortName);
		}
	}
}
