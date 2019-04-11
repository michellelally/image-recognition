using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Media.Plugin.Sample
{
	public partial class MediaPage : ContentPage
	{

		public string filepath { get; set; }

		const string uriBase =
		"https://westeurope.api.cognitive.microsoft.com/vision/v2.0/analyze";
		const string subscriptionKey = "7071ff9c11e24a59a1c5d411ffa551ef";

		ObservableCollection<MediaFile> files = new ObservableCollection<MediaFile>();


		public MediaPage()
		{
			InitializeComponent();

			takePhoto.Clicked += async (sender, args) =>
			{
				files.Clear();
				if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
				{
					await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
					return;
				}

				var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
				{
					Directory = "Test",
					SaveToAlbum = true,
					CompressionQuality = 75,
					CustomPhotoSize = 50,
					PhotoSize = PhotoSize.MaxWidthHeight,
					MaxWidthHeight = 2000,
					DefaultCamera = CameraDevice.Front
				});

				if (file == null)
					return;

				await DisplayAlert("File Location", file.Path, "OK");

				filepath = file.Path;

				files.Add(file);
			};

			pickPhoto.Clicked += async (sender, args) =>
			{
				if (!CrossMedia.Current.IsPickPhotoSupported)
				{
					await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
					return;
				}
				var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
				{
					PhotoSize = PhotoSize.Medium
				});


				if (file == null)
					return;

				filepath = file.Path;

			};


			analyze.Clicked += async (sender, args) =>
				{


					if (File.Exists(filepath))
					{
						// Call the REST API method.
						Debug.Write("\nWait a moment for the results to appear.\n");
						MakeAnalysisRequest(filepath).Wait();
					}
					else
					{
						Debug.Write("\nInvalid file path");
					}

				};

			async Task MakeAnalysisRequest(string imageFilePath)
			{
				Debug.Write("In MakeAnalysisRequest");
				try
				{
					HttpClient client = new HttpClient();

					// Request headers.
					client.DefaultRequestHeaders.Add(
						"Ocp-Apim-Subscription-Key", subscriptionKey);

					// Request parameters. A third optional parameter is "details".
					// The Analyze Image method returns information about the following
					// visual features:
					// Categories:  categorizes image content according to a
					//              taxonomy defined in documentation.
					// Description: describes the image content with a complete
					//              sentence in supported languages.
					// Color:       determines the accent color, dominant color, 
					//              and whether an image is black & white.
					string requestParameters =
						"visualFeatures=Categories,Description,Color";

					// Assemble the URI for the REST API method.
					string uri = uriBase + "?" + requestParameters;

					HttpResponseMessage response;

					// Read the contents of the specified local image
					// into a byte array.
					Debug.Write("Calling GetImageAsByteArray");
					byte[] byteData = GetImageAsByteArray(imageFilePath);
					if (byteData == null)
						Debug.Write("(byteData == null)");
					Debug.Write("Finished GetImageAsByteArray");
					// Add the byte array as an octet stream to the request body.
					using (var content = new ByteArrayContent(byteData))
					{
						Debug.Write("inside using byte array content");
						// This example uses the "application/octet-stream" content type.
						// The other content types you can use are "application/json"
						// and "multipart/form-data".
						content.Headers.ContentType =
							new MediaTypeHeaderValue("application/octet-stream");

						// Asynchronously call the REST API method.
						Debug.Write("call the REST API method.");
						response = await client.PostAsync(uri, content).ConfigureAwait(false);
						Debug.Write("Finished response : \n" + response);
					}

					// Asynchronously get the JSON response.
					string contentString = await response.Content.ReadAsStringAsync();

					// Display the JSON response.
					Debug.Write("\nResponse:\n\n{0}\n",
						JToken.Parse(contentString).ToString());
				}
				catch (Exception e)
				{
					Debug.Write("\n" + e.Message);
				}
			}

			/// <summary>
			/// Returns the contents of the specified file as a byte array.
			/// </summary>
			/// <param name="imageFilePath">The image file to read.</param>
			/// <returns>The byte array of the image data.</returns>
			byte[] GetImageAsByteArray(string imageFilePath)
			{
				Debug.Write("In GetImageAsByteArray");
				// Open a read-only file stream for the specified file.
				using (FileStream fileStream =
					new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
				{
					// Read the file's contents into a byte array.
					BinaryReader binaryReader = new BinaryReader(fileStream);
					return binaryReader.ReadBytes((int)fileStream.Length);
				}
			}

		}
	}
}