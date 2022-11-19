using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using DataExtractor.Core.Models.Json;
using Newtonsoft.Json;

namespace DataExtractor.Core.Exts;

internal static class HttpClientExts
{
	public static string GetString(this HttpClient httpClient, string url, Dictionary<string, string> headers = null)
	{
		Thread.Sleep(TimeSpan.FromSeconds(3.0));
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
		if (headers != null)
		{
			foreach (string key in headers.Keys)
			{
				httpRequestMessage.Headers.Add(key, headers[key]);
			}
		}
		try
		{
			HttpResponseMessage result = httpClient.SendAsync(httpRequestMessage).Result;
			return result.Content.ReadAsStringAsync().Result;
		}
		catch
		{
			return "";
		}
	}

	public static HttpResponseMessage Get(this HttpClient httpClient, string url, Dictionary<string, string> headers = null)
	{
		Thread.Sleep(TimeSpan.FromSeconds(3.0));
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
		if (headers != null)
		{
			foreach (string key in headers.Keys)
			{
				httpRequestMessage.Headers.Add(key, headers[key]);
			}
		}
		try
		{
			return httpClient.SendAsync(httpRequestMessage).Result;
		}
		catch
		{
			return null;
		}
	}

	public static string PostString(this HttpClient httpClient, string url, string formData, string contentType, Dictionary<string, string> headers = null)
	{
		Thread.Sleep(TimeSpan.FromSeconds(3.0));
		StringContent content = new StringContent(formData, Encoding.UTF8, contentType);
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
		httpRequestMessage.Content = content;
		if (headers != null)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				httpRequestMessage.Headers.Add(header.Key, header.Value);
			}
		}
		try
		{
			HttpResponseMessage result = httpClient.SendAsync(httpRequestMessage).Result;
			return result.Content.ReadAsStringAsync().Result;
		}
		catch
		{
			return "";
		}
	}

	public static HttpResponseMessage Post(this HttpClient httpClient, string url, string formData, string contentType, Dictionary<string, string> headers = null)
	{
		Thread.Sleep(TimeSpan.FromSeconds(3.0));
		StringContent content = new StringContent(formData, Encoding.UTF8, contentType);
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
		httpRequestMessage.Content = content;
		if (headers != null)
		{
			foreach (KeyValuePair<string, string> header in headers)
			{
				httpRequestMessage.Headers.Add(header.Key, header.Value);
			}
		}
		try
		{
			return httpClient.SendAsync(httpRequestMessage).Result;
		}
		catch
		{
			return null;
		}
	}

	private static IEnumerable<BatchNguongJson> BatchFile(this HttpClient httpClient, string url, string accessToken, object objectData)
	{
		string text = JsonConvert.SerializeObject(objectData);
		if (text == "[]")
		{
			return Enumerable.Empty<BatchNguongJson>();
		}
		text = "access_token=" + accessToken + "&batch=" + WebUtility.UrlEncode(text);
		string value = httpClient.PostString(url, text, "application/x-www-form-urlencoded");
		try
		{
			return JsonConvert.DeserializeObject<IEnumerable<BatchNguongJson>>(value);
		}
		catch
		{
			return Enumerable.Empty<BatchNguongJson>();
		}
	}
}
