using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;

public class AwsHelper : MonoBehaviour {

	public static AwsHelper Instance;

	public string IdentityPoolId = "";
	public string CognitoIdentityRegion = /*RegionEndpoint.USWest1.SystemName*/ RegionEndpoint.USEast1.SystemName;
	private RegionEndpoint _CognitoIdentityRegion
	{
		get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
	}
	public string S3Region = /*RegionEndpoint.USWest1.SystemName;*/ RegionEndpoint.USEast1.SystemName;
	private RegionEndpoint _S3Region
	{
		get { return RegionEndpoint.GetBySystemName(S3Region); }
	}
	public string S3BucketName = null;
	public string SampleFileName = null;
//	public Button GetBucketListButton = null;
//	public Button PostBucketButton = null;
//	public Button GetObjectsListButton = null;
//	public Button DeleteObjectButton = null;
//	public Button GetObjectButton = null;
	public Text ResultText = null;

	void Awake()
	{
		Instance = this;
//		RegionEndpoint.USWest2.
	}

	void Start()
	{
		UnityInitializer.AttachToGameObject(this.gameObject);
		Amazon.AWSConfigs.HttpClient = Amazon.AWSConfigs.HttpClientOption.UnityWebRequest;

//		GetBucketListButton.onClick.AddListener(() => { GetBucketList(); });
//		PostBucketButton.onClick.AddListener(() => { PostObject(); });
//		GetObjectsListButton.onClick.AddListener(() => { GetObjects(); });
//		DeleteObjectButton.onClick.AddListener(() => { DeleteObject(); });
//		GetObjectButton.onClick.AddListener(() => { GetObject(); });
	}

	#region private members

	private IAmazonS3 _s3Client;
	private AWSCredentials _credentials;

	private AWSCredentials Credentials
	{
		get
		{
			if (_credentials == null)
				_credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
			return _credentials;
		}
	}

	private IAmazonS3 Client
	{
		get
		{
			if (_s3Client == null)
			{
				// NEWMAN ADDED THIS MANUALLY
//				_s3Client = new AmazonS3Client ("AKIAJONYATMHZLVWQVEQ", "iB8XGgITm55MhlijbyfptJd8J4mW9TLnYOTK2gfQ", _S3Region); //(Credentials, _S3Region);
				_s3Client = new AmazonS3Client("AKIAJ5KW42OVK2VHVV4A", "aT4d1RCMUMlFZVIzclK3sWSKN6cWRlsIM5A4OoVN", _S3Region);
			}
			//test comment
			return _s3Client;
		}
	}

	#endregion

	#region Get Bucket List
	/// <summary>
	/// Example method to Demostrate GetBucketList
	/// </summary>
	public void GetBucketList()
	{
		print("Fetching all the Buckets");
		Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
			{
				print("\n");
				if (responseObject.Exception == null)
				{
					print("Got Response \nPrinting now \n");
					responseObject.Response.Buckets.ForEach((s3b) =>
						{
							print("bucket = " + s3b.BucketName + ", created date = " + s3b.CreationDate + "\n");
						});
				}
				else
				{
					print("Got Exception \n");
				}
			});
	}

	#endregion

	/// <summary>
	/// Get Object from S3 Bucket
	/// </summary>
	public void GetObject()
	{
		print ("fetching " + SampleFileName + " from bucket " + S3BucketName);
		Client.GetObjectAsync(S3BucketName, SampleFileName, (responseObj) =>
			{
				string data = null;
				var response = responseObj.Response;
				if (response.ResponseStream != null)
				{
					using (var fs = System.IO.File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + SampleFileName /*@"c:\some-folder\file-name.ext"*/))
					{
						byte[] buffer = new byte[81920];
						int count;
						while ((count = response.ResponseStream.Read(buffer, 0, buffer.Length)) != 0)
							fs.Write(buffer, 0, count);
						fs.Flush();
					}

					print("\nPulled s3 data to file");


//					using (StreamReader reader = new StreamReader(response.ResponseStream))
//					{
//						data = reader.ReadToEnd();
//					}
//					print("!!! DATA: " + data);
//					print("+= "\n !!! ";
//					print("+= data;





				}
				else
				{
					print("\nData is null");
				}
			});
	}

	/// <summary>
	/// Post Object to S3 Bucket. 
	/// </summary>
	public void PostObject()
	{
		print("Retrieving the file");

		string fileName = GetFileHelper();

//		print ("!!! FILE PATH: " + Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);
//		var stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
		var stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName, FileMode.Open, FileAccess.Read, FileShare.Read);


		print("\nCreating request object");

//		print("+= string.Format("\n TEST FORMATTING: {0}", "butt");

		var request = new PostObjectRequest()
		{
			Bucket = S3BucketName,
			Key = fileName,
			InputStream = stream,
			CannedACL = S3CannedACL.Private,
//			CannedACL = S3CannedACL.PublicReadWrite,// Private
			Region = _S3Region	
		};

		print("\nMaking HTTP post call");

		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{
					print("\nPosted Object Success");
					print("\nobject " + responseObj.Request.Key + " posted to bucket " + responseObj.Request.Bucket);
				}
				else
				{

					print ("!!! POST EXCEPTION: " + responseObj.Exception.Message);


					print("\nException while posting the result object");
					print("\nException: " + responseObj.Exception.Message);
//					print("+= "\nException while posting the result object";
//					print("+= "\nH: " + responseObj.Response. .Response.HttpStatusCode.ToString();
					print("\n receieved error " + responseObj.Response.HttpStatusCode.ToString());


				}
			});
	}

	/// <summary>
	/// Get Objects from S3 Bucket
	/// </summary>
	public void GetObjects()
	{
		print("Fetching all the Objects from " + S3BucketName);

		var request = new ListObjectsRequest()
		{
			BucketName = S3BucketName
		};

		Client.ListObjectsAsync(request, (responseObject) =>
			{
				print("\n");
				if (responseObject.Exception == null)
				{
					print("Got Response \nPrinting now \n");
					responseObject.Response.S3Objects.ForEach((o) =>
						{
							print(o.Key + "\n");
						});
				}
				else
				{
					print("Got Exception \n");
				}
			});
	}

	/// <summary>
	/// Delete Objects in S3 Bucket
	/// </summary>
	public void DeleteObject()
	{
		print ("deleting " + SampleFileName + " from bucket " + S3BucketName);
		List<KeyVersion> objects = new List<KeyVersion>();
		objects.Add(new KeyVersion()
			{
				Key = SampleFileName
			});

		var request = new DeleteObjectsRequest()
		{
			BucketName = S3BucketName,
			Objects = objects
		};

		Client.DeleteObjectsAsync(request, (responseObj) =>
			{
				print("\n");
				if (responseObj.Exception == null)
				{
					print("Got Response \n \n");

					print("deleted objects \n");

					responseObj.Response.DeletedObjects.ForEach((dObj) =>
						{
							print(dObj.Key);
						});
				}
				else
				{
					print("Got Exception \n");
				}
			});
	}


	#region helper methods

	private string GetFileHelper()
	{
		var fileName = SampleFileName;

		if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName))
		{
			var streamReader = File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);
			streamReader.WriteLine("This is a sample s3 file uploaded from unity s3 sample");
			streamReader.Close();
		}
		return fileName;
	}

	private string GetPostPolicy(string bucketName, string key, string contentType)
	{
		bucketName = bucketName.Trim();

		key = key.Trim();
		// uploadFileName cannot start with /
		if (!string.IsNullOrEmpty(key) && key[0] == '/')
		{
			throw new ArgumentException("uploadFileName cannot start with / ");
		}

		contentType = contentType.Trim();

		if (string.IsNullOrEmpty(bucketName))
		{
			throw new ArgumentException("bucketName cannot be null or empty. It's required to build post policy");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentException("uploadFileName cannot be null or empty. It's required to build post policy");
		}
		if (string.IsNullOrEmpty(contentType))
		{
			throw new ArgumentException("contentType cannot be null or empty. It's required to build post policy");
		}

		string policyString = null;
		int position = key.LastIndexOf('/');
		if (position == -1)
		{
			policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
				bucketName + "\"},[\"starts-with\", \"$key\", \"" + "\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
		}
		else
		{
			policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
				bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring(0, position) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
		}

		return policyString;
	}

	#endregion
}
