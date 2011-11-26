using System;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;

namespace NearlyFreeSpeechNet
{
	internal class Connection
	{
		private const string SaltChars = "0123456789ABCDEFGHIJKLMNOPQRTSUVWXYZabcdefghijklmnopqrstuvwxyz";
		private const string BaseUrl = "https://api.nearlyfreespeech.net";
		private const string AuthHeaderName = "X-NFSN-Authentication";
		
		private string m_login;
		private string m_apikey;
		private SHA1 m_hasher;
		
		internal Connection(string login, string apikey)
		{
			m_login = login;
			m_apikey = apikey;
			m_hasher = new SHA1CryptoServiceProvider();
		}
		
		/// <summary>
		/// Generates a salt to the required specification (16 characters long, [0-9a-zA-Z])
		/// </summary>
		private static string Salt()
		{
			Random rand = new Random();
			string result = "";
			for (int i = 0; i < 16; i++)
				result = result + SaltChars[rand.Next(SaltChars.Length)];
			return result;
		}
		
		/// <summary>
		/// Generates a timestamp of the required type (number of seconds since 1970, time_t)
		/// </summary>
		private static string Timestamp()
		{
			DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			TimeSpan currTime = DateTime.UtcNow - startTime;
			UInt32 time_tVal = (UInt32)currTime.TotalSeconds;
			return time_tVal.ToString();
		}
		
		/// <summary>
		/// Computes the SHA1 hash of a given string
		/// </summary>
		/// <returns>
		/// The hex string representing the hash
		/// </returns>
		/// <param name='data'>
		/// The source string to hash
		/// </param>
		private string ComputeHash(string data)
		{
			byte[] result;
			string output;
			
			result = m_hasher.ComputeHash(Encoding.UTF8.GetBytes(data));
			// Easiest thing here is to manually convert to hex
			output = "";
			foreach(byte b in result)
				output += b.ToString("X2");
			return output.ToLower();
		}
		
		/// <summary>
		/// Generates the required authentication header
		/// </summary>
		/// <returns>
		/// The string to use in the header
		/// </returns>
		/// <param name='url'>
		/// The path component of the URL to use in the request.
		/// </param>
		/// <param name='body'>
		/// The body of the request.
		/// </param>
		private string AuthHeader(string url, string body)
		{
			string timestamp;
			string salt;
			string hashString;
			string bodyHash, hash;
			
			if (body == null)
				body = "";
			timestamp = Timestamp();
			salt = Salt();
			bodyHash = ComputeHash(body);
			hashString = m_login + ";" + timestamp + ";" + salt + ";" + m_apikey + ";" + url + ";" + bodyHash;
			hash = ComputeHash(hashString);
			return m_login + ";" + timestamp + ";" + salt + ";" + hash;
		}
		
		/// <summary>
		/// Get the specified url.
		/// </summary>
		/// <param name='url'>
		/// The path component of the URL to get.
		/// </param>
		internal string Get(string url)
		{
			return DoSend("GET", url, null, null);
		}
		
		/// <summary>
		/// Put the specified body to the given url.
		/// </summary>
		/// <param name='url'>
		/// The path component of the URL to put.
		/// </param>
		/// <param name='body'>
		/// The content body to put.
		/// </param>
		internal string Put(string url, string body)
		{
			return DoSend("PUT", url, body, "text/plain");
		}
		
		/// <summary>
		/// Post the specified parameters as form data to the given url.
		/// </summary>
		/// <param name='url'>
		/// The path component of the URL to post.
		/// </param>
		/// <param name='parameters'>
		/// The parameters to be encoded as form data.
		/// </param>
		internal string Post(string url, WebHeaderCollection parameters)
		{
			string body = null;
			if (parameters != null)
			{
				StringBuilder parameterData = new StringBuilder();
				foreach(string name in parameters.AllKeys)
				{
					foreach(string aValue in parameters.GetValues(name))
					{
						parameterData.Append(name + "=");
						parameterData.Append(HttpUtility.UrlEncode(aValue) + "&");
					}
				}
				body = parameterData.ToString();
				if (body == "")
					body = null;
			}
			return DoSend("POST", url, body, "application/x-www-form-urlencoded");
		}
		
		/// <summary>
		/// Perform an HTTP request.
		/// </summary>
		/// <returns>
		/// Document content contained in the response.
		/// </returns>
		/// <param name='method'>
		/// POST, GET or PUT
		/// </param>
		/// <param name='url'>
		/// The path component of the URL to use.
		/// </param>
		/// <param name='body'>
		/// The content body for the request
		/// </param>
		/// <param name='mimetype'>
		/// The MIME type of the content body, if any
		/// </param>
		private string DoSend(string method, string url, string body, string mimetype)
		{
			HttpWebRequest request;
			
			request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
			request.Method = method;
			request.Headers.Add(AuthHeaderName, AuthHeader(url, body));
			if (body != null)
			{
				byte[] bodyData = Encoding.UTF8.GetBytes(body);
				request.ContentType = mimetype;
				request.ContentLength = bodyData.Length;
				using (Stream post = request.GetRequestStream())
				{
					post.Write(bodyData, 0, bodyData.Length);
				}
			}
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				StreamReader reader = new StreamReader(response.GetResponseStream());
				return reader.ReadToEnd();
			}
		}
		
		/// <summary>
		/// Convert the given JSON data into native types
		/// </summary>
		/// <param name='data'>
		/// The string JSON data
		/// </param>
		/// <typeparam name='T'>
		/// The type expected to be returned
		/// </typeparam>
		/// <returns>
		/// The object of type T
		/// </returns>
		public static T Deserialize<T>(string data)
		{
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
			return (T)ser.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(data)));
		}
	}
}
