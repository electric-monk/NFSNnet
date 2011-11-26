using System;
using System.Net;
using System.IO;

namespace DynDnsUpdate
{
	public class ExternIP
	{
		public static string GetIP()
		{
			HttpWebRequest request = WebRequest.Create("http://checkip.dyndns.org/") as HttpWebRequest;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			StreamReader reader = new StreamReader(response.GetResponseStream());
			string result = reader.ReadToEnd();
			result = result.ToLower();
			int start = result.IndexOf("<body>") + "<body>".Length;
			int end = result.IndexOf("</body>");
			string content = result.Substring(start, end - start);
			int position = 0;
			while (content[position] != ':')
				position++;
			while ((content[position] == ':') || (content[position] == ' '))
				position++;
			return content.Substring(position);
		}
	}
}

