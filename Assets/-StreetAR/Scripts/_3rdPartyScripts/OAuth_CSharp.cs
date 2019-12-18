using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

public class OAuth_CSharp
{
    private string oauthConsumerKey = null;
    private string oauthConsumerSecret = null;
    private string oauthToken = null;
    private string oauthTokenSecret = null;
    private string oauthNonce = null;
    private string oauthTimeStamp = null;

    private string oauthSignature = "";

    private const string oauthMethod = "HMAC-SHA1";
    private const string oauthVersion = "1.0";

    private SortedDictionary<string, string> urlParamsDictionary;
    private string outputString = "";

    protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
    /// <summary>
    /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
    /// </summary>
    private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };


    public OAuth_CSharp(string consumerKey, string consumerSecret)
    {
        oauthConsumerKey = consumerKey;
        oauthConsumerSecret = consumerSecret;
        oauthNonce = GetNonce();
        oauthTimeStamp = GetTimestamp();

        GenerateDictionaryNoToken();
    }

    public OAuth_CSharp(string consumerKey, string consumerSecret, string token, string tokenSecret)
    {
        oauthConsumerKey = consumerKey;
        oauthConsumerSecret = consumerSecret;
        oauthToken = token;
        oauthTokenSecret = tokenSecret;
        oauthNonce = GetNonce();
        oauthTimeStamp = GetTimestamp();

        GenerateDictionary();
    }

    public string GenerateRequestURL(string url, string HTTP_Method , List<string> parameters)
    {
        outputString = url;

        // outputString is changed in GenerateSignatureBase and GenerateSignature
        string signatureBase = GenerateSignatureBase(url, HTTP_Method, parameters);
        string signature = GenerateSignature(signatureBase);

        //string signature = UrlEncode(GenerateSignature(signatureBase));

        return outputString;
    }

    public string GenerateRequestURL(string url, string HTTP_Method)
    {
        outputString = url;

        // outputString is changed in GenerateSignatureBase and GenerateSignature
        string signatureBase = GenerateSignatureBase(url, HTTP_Method);
        string signature = GenerateSignature(signatureBase);

        //string signature = UrlEncode(GenerateSignature(signatureBase));



        return outputString;
    }


    private string GetNonce()
    {
        //string nonce = rand.Next(123400, 9999999).ToString();

        //// http://www.i-avington.com/Posts/Post/making-a-twitter-oauth-api-call-using-c
        //System.Random rand = new System.Random();
        //string nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));
        //return nonce;


        string nonce = Guid.NewGuid().ToString("N");
        //string nonce = "pcg4rU";

        return nonce;
        ////return "vwr2RsJB51L";
        ////return "98cf8340b2af4c98ac6404139d9ccdb2";
    }

    private string GetTimestamp()
    {
        /*
        // http://www.i-avington.com/Posts/Post/making-a-twitter-oauth-api-call-using-c
        TimeSpan _timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
        string timeStamp = Convert.ToInt64(_timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        return timeStamp;
        */

        
        //http://stackoverflow.com/questions/16919320/c-sharp-oauth-and-signatures
        TimeSpan ts = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        string timeStamp = ts.TotalSeconds.ToString();
        timeStamp = timeStamp.Substring(0, timeStamp.IndexOf("."));
        return timeStamp;
        //return "1575596995";
    }

    private void GenerateDictionary()
    {
        //the key value pairs have to be sorted by encoded key
        urlParamsDictionary = new SortedDictionary<string, string>()
        {           
            {Uri.EscapeDataString("oauth_consumer_key"), Uri.EscapeDataString(oauthConsumerKey)},
            {Uri.EscapeDataString("oauth_nonce"), Uri.EscapeDataString(oauthNonce)},
            {Uri.EscapeDataString("oauth_signature_method"), Uri.EscapeDataString(oauthMethod)},
            {Uri.EscapeDataString("oauth_timestamp"), Uri.EscapeDataString(oauthTimeStamp)},
            {Uri.EscapeDataString("oauth_token"), Uri.EscapeDataString(oauthToken)},
            {Uri.EscapeDataString("oauth_version"), Uri.EscapeDataString(oauthVersion)},

            //{Uri.EscapeDataString("oauth_signature"), Uri.EscapeDataString(oauthSignature)}
        };
    }

    private void GenerateDictionaryNoToken()
    {
        //the key value pairs have to be sorted by encoded key
        urlParamsDictionary = new SortedDictionary<string, string>()
        {           
            {Uri.EscapeDataString("oauth_consumer_key"), Uri.EscapeDataString(oauthConsumerKey)},
            {Uri.EscapeDataString("oauth_nonce"), Uri.EscapeDataString(oauthNonce)},
            {Uri.EscapeDataString("oauth_signature_method"), Uri.EscapeDataString(oauthMethod)},
            {Uri.EscapeDataString("oauth_timestamp"), Uri.EscapeDataString(oauthTimeStamp)},           
            {Uri.EscapeDataString("oauth_version"), Uri.EscapeDataString(oauthVersion)},

            //{Uri.EscapeDataString("oauth_signature"), Uri.EscapeDataString(oauthSignature)}
        };
    }

    private string GenerateSignatureBase(string url, string HTTP_Method, List<string> parameters)
    {
        if(parameters == null)
        {
            Debug.Log("Invalid parameters supplied: null. Running without parameters.");
            return GenerateSignatureBase(url, HTTP_Method);            
        }
        if (parameters.Count != 0)
        {
            foreach (string parameter in parameters)
            {
                //Debug.Log(parameters);
                string[] elements = parameter.Split(new char[] { '=' });
                if (elements.Length % 2 != 0)
                {
                    Debug.LogWarning("Invalid parameter supplied");
                }
                else
                {
                    try
                    {
                        urlParamsDictionary.Add(elements[0], elements[1]);
                    }
                    catch(ArgumentException argExc)
                    {
                        Debug.LogWarning("Parameter key already in dictionary: " + elements[0]);
                    }
                }
            }
        }

        return GenerateSignatureBase(url, HTTP_Method);
    }

    private string GenerateSignatureBase(string url, string HTTP_Method)
    { 
        string parameterString = String.Empty;
        
        int totalKeys = urlParamsDictionary.Keys.Count;
        int keyCount = 0;
        foreach (KeyValuePair<string, string> keyValuePair in urlParamsDictionary)
        {
           // Debug.Log("Appended to output: " + keyValuePair.Key + "=" + keyValuePair.Value);
            parameterString += keyValuePair.Key + "=" + keyValuePair.Value;

            if (++keyCount != totalKeys) // indicates the last item is found
                parameterString += "&";
        }

        // NEWMAN CHANGE - Changed from '?' to '&'
        outputString += "&" + parameterString;
        Debug.Log(parameterString);
        Debug.Log("Output String: " + outputString);

        string signatureBase = HTTP_Method.ToUpper() + "&" + Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(parameterString);
        
        Debug.Log("Signature Base: " + signatureBase);
        return signatureBase;
    }

    private string GenerateSignature(string signatureBase)
    {
        string signatureKey = "";
        if (oauthToken != null && oauthTokenSecret != null)
        {
            signatureKey = Uri.EscapeDataString(oauthConsumerSecret) + "&" + Uri.EscapeDataString(oauthTokenSecret);
            Debug.Log("OAuth token secret is not null");
        }
        else
        {
            signatureKey = Uri.EscapeDataString(oauthConsumerSecret) + "&";
        }
        HMACSHA1 hmacsha1 = new HMACSHA1(new ASCIIEncoding().GetBytes(signatureKey));

        //hash the values
        string signature = Convert.ToBase64String(hmacsha1.ComputeHash(new ASCIIEncoding().GetBytes(signatureBase)));

        //Debug.Log("**Generate Signature: " + signature);
        //signature = UrlEncode(signature);
        //Debug.Log("Encoded Signature: " + signature);
        outputString += "&oauth_signature=" + signature;

        oauthSignature = signature;

        return signature;
    }

    /// <summary>
    /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
    /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
    /// </summary>
    /// <param name="value">The value to Url encode</param>
    /// <returns>Returns a Url encoded string</returns>
    protected string UrlEncode(string value)
    {
        StringBuilder result = new StringBuilder();

        foreach (char symbol in value)
        {
            if (unreservedChars.IndexOf(symbol) != -1)
            {
                result.Append(symbol);
            }
            else
            {
                result.Append('%' + String.Format("{0:X2}", (int)symbol));
            }
        }

        return result.ToString();
    }

    public string GetEncodedOAuthSignature()
    {
        return UrlEncode(oauthSignature);
    }





    /// <summary>
    /// Escapes a string according to the URI data string rules given in RFC 3986.
    /// </summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped value.</returns>
    /// <remarks>
    /// The <see cref="EscapeUriDataStringRfc3986"/> method is <i>supposed</i> to take on
    /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
    /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
    /// host actually having this configuration element present.
    /// </remarks>
    internal static string EscapeUriDataStringRfc3986(string value)
    {
        // Start with RFC 2396 escaping by calling the .NET method to do the work.
        // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
        // If it does, the escaping we do that follows it will be a no-op since the
        // characters we search for to replace can't possibly exist in the string.
        StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

        // Upgrade the escaping to RFC 3986, if necessary.
        for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
        {
            escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
        }

        // Return the fully-RFC3986-escaped string.
        return escaped.ToString();
    }
}