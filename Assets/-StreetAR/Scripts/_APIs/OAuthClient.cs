using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OAuthClient : MonoBehaviour
{
	private string _clientId = "JSZ9a4c81f31c67|u4Zqk";
	private string _clientSecret = "dBlKwesMS6m1qXVyD2g0LU7KHak";

	void Start()
	{
		RunTest();
	}
	
	void Update()
	{
		
	}

    private void RunTest()
	{
		var queryParams = new List<QueryParameter>();

		var if_clientid = new QueryParameter("if_clientid", "JSZ9a4c81f31c67|u4Zqk");
		var if_auth_type = new QueryParameter("if_auth_type", "oauth");
		var latlon = new QueryParameter("latlon", "35.6633583,139.7613555"); // <=== PUT ACTUAL GPS VALUES HERE
		var radius = new QueryParameter("radius", "250"); // <=== PUT ACTUAL RADIUS VALUE HERE
		//var word = new QueryParameter("word", "Lawson"); // <=== PUT ACTUAL SEARCH TERM VALUE HERE

		queryParams.Add(if_clientid);
		queryParams.Add(if_auth_type);
		queryParams.Add(latlon);
		queryParams.Add(radius);
		//queryParams.Add(word);


		// OAuth 1.0 parameters
		var oauth_consumer_key = new QueryParameter("oauth_consumer_key", "JSZ9a4c81f31c67|u4Zqk");
		var oauth_nonce = new QueryParameter("oauth_nonce", "1UQ4M9");
		var oauth_timestamp = new QueryParameter("oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
		var oauth_signature_method = new QueryParameter("oauth_signature_method", "HMAC-SHA1");
		var oauth_version = new QueryParameter("oauth_version", "1.0");		

		queryParams.Add(oauth_consumer_key);
		queryParams.Add(oauth_nonce);
		queryParams.Add(oauth_timestamp);
		queryParams.Add(oauth_signature_method);
		queryParams.Add(oauth_version);


		StartCoroutine(GetData("https://test.core.its-mo.com/zmaps/api/apicore/core/v1_0/poi/latlon", queryParams));
	}

	public IEnumerator GetData(string URL, List<QueryParameter> parameters)
	{
		var httpMethod = "GET";
		var encodedUri = Uri.EscapeDataString(URL);

		Debug.Log("Query Params Length = " + parameters.Count);

		// https://support.e-map.ne.jp/manuals/v3/oauth_sig
		// 1   = Generate text to be signed
		// 1.1 = Decompose the HTTP request into the following three elements: HTTP_METHOD, URI, QUERY_PARAMS (URL-encoded)
		// 1.2 = Sort the query parameter key value in byte order and connect with “&”
		// 1.3 = URL-encode each of the three disassembled elements and connect with &
		// 2   = Generate OAuth signature using Signature Base String
		// 2.1 = Generate digest value using the signature base string HMAC-SHA1 algorithm (use the secret key issued by ZDC with & appended to it)
		// 2.2 = The digest value created in 2-1 is completed with Base64 encoding


		// PART 1-1: Build the Base String
		// TODO: this process could be much cleaner/succinct, but works for now...
		// url encode the parameters
		var urlEncodedParams = parameters.Select(param => new QueryParameter(param.Key, param.Value)).ToList();
		var urlEncodedParamsString = string.Empty;

		int totalKeys = urlEncodedParams.Count;
        int keyCount = 0;

		foreach (var parameter in urlEncodedParams)
		{
			// must URL-encode before doing byte-order
			parameter.Key = Uri.EscapeDataString(parameter.Key);
			parameter.Value = Uri.EscapeDataString(parameter.Value);

			// also build the find querystrings to send via UnityWebRequest
			urlEncodedParamsString += parameter.Key + "=" + parameter.Value;
			if (++keyCount != totalKeys) // indicates the last item is found
                urlEncodedParamsString += "&";
		}

		Debug.Log("Querystrings:\n" + urlEncodedParamsString);

		// PART 1-2
		var orderedQueryParameters = urlEncodedParams
			.OrderBy(parm => parm, new QueryParameterComparer());

		// PART 1-3
		string parameterString = String.Empty;
		totalKeys = parameters.Count;
        keyCount = 0;
        foreach (QueryParameter parameter in orderedQueryParameters)
        {
            parameterString += Uri.EscapeDataString(parameter.Key + "=" + parameter.Value);

            if (++keyCount != totalKeys) // indicates the last item is found
                parameterString += Uri.EscapeDataString("&");
        }

		// this is the final base string:
		var baseString = httpMethod + "&" + encodedUri + "&" + parameterString;
		Debug.Log("OAuth Base String:\n" + baseString);

		// PART 2: Generate OAuth signature using Signature Base String
		HMACSHA1 hmacsha1 = new HMACSHA1(new ASCIIEncoding().GetBytes(_clientSecret + "&"));
        string signature = Convert.ToBase64String(hmacsha1.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));
		Debug.Log("OAuth Signature:\n" + signature);



		// PART 3: make the request
		// based on here: https://support.e-map.ne.jp/manuals/v3/?q=auth#oauth
		string requestURL = URL + "?" + urlEncodedParamsString + "&oauth_signature=" + Uri.EscapeDataString(signature);
		Debug.Log("requestURL:\n" + requestURL);

		UnityWebRequest request = UnityWebRequest.Get(requestURL);
		// TODO: decide which headers we actually want to keep
		request.SetRequestHeader("Accept", "application/json");
		request.SetRequestHeader("Accept-Encoding", "gzip, deflate, br");
		request.SetRequestHeader("Accept-Language", "en-US,en;q=0.9");
		request.SetRequestHeader("Cache-Control", "no-cache");
		request.SetRequestHeader("X-Zdc-Auth-Cache", "0");
		request.SetRequestHeader("X-Zdc-Auth-Expire", "3600");

        yield return request.SendWebRequest();
 
        if(request.isNetworkError) {
            Debug.LogError(request.error);
        }
        else {
            // the data from the API
            Debug.Log(request.downloadHandler.text);
        }
	}

	
}

public class QueryParameter
{
	public string Key { get; set; }
	public string Value { get; set; }

	public QueryParameter(string key, string value)
	{
		Key = key;
		Value = value;
	}
}

public class QueryParameterComparer : IComparer<QueryParameter>
{
    public int Compare(QueryParameter x, QueryParameter y)
	{
          if(x.Key == y.Key) {
             return string.Compare(x.Value, y.Value, StringComparison.Ordinal);
          }
           else {
             return string.Compare(x.Key, y.Key, StringComparison.Ordinal);
          }
    }
}
