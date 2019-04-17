using Media.Plugin.Sample.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace Media.Plugin.Sample
{
	public partial class MediaPage : ContentPage
	{
		private string JSON_PHOTOS_FILE = "data.txt";
		public string filepath { get; set; }
		const string uriBase =
		"https://westeurope.api.cognitive.microsoft.com/vision/v2.0/analyze";
		const string subscriptionKey = "dd88925488774574b455e3b0dd57bfca";
		ObservableCollection<Photo> photos = new ObservableCollection<Photo>();
		ObservableCollection<MediaFile> files = new ObservableCollection<MediaFile>();

		Photo Photo { get; set; }
			
		public MediaPage()
		{
			InitializeComponent();

			files.CollectionChanged += Files_CollectionChanged;
	
			Photo = new Photo();

			/// <summary>
			/// Allows user to choose take photo with their camera that they want to analyze 
			/// </summary>
			/// <param name="sender">The object calling the method.</param>
			/// /// <param name="args">The event that triggered the call.</param>
			takePhoto.Clicked += async (sender, args) =>
			{
				// Remove any existing photos
				files.Clear();
				// If camera cannot be accessed 
				if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
				{
					// Display this message
					await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
					return;
				}
				// Calling TakePhotoAsync method which is in charge of taking photos using the devices camera
				var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
				{
					// Specified options 
					PhotoSize = PhotoSize.Medium,
					Directory = "Sample",
					Name = "test.jpg"
				});
				//If file doesn't exist, exit method
				if (file == null)
					return;
				// Get and store the file path of the image
				filepath = file.Path;
				// Add file to the list
				files.Add(file);
			};
			
			/// <summary>
			/// Allows user to choose photo from their gallery that they want to analyze 
			/// </summary>
			/// <param name="sender">The object calling the method.</param>
			/// /// <param name="args">The event that triggered the call.</param>
			pickPhoto.Clicked += async (sender, args) =>
			{
				// Remove any existing photos
				files.Clear();
				// If camera cannot be accessed 
				if (!CrossMedia.Current.IsPickPhotoSupported)
				{
					// Display this message
					await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
					return;
				}
				// Calling TakePhotoAsync method which is in charge of taking photos using the devices camera
				var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
				{
					PhotoSize = PhotoSize.Medium
				});
				//If file doesn't exist, exit method
				if (file == null)
					return;
				// Get and store the file path of the image
				filepath = file.Path;
				// Add file to the list 
				files.Add(file);
			};
			
			/// <summary>
			/// Checks that file exists and makes the call to the MakeAnalysisRequest method  
			/// </summary>
			/// <param name="sender">The object calling the method.</param>
			/// /// <param name="args">The event that triggered the call.</param>
			analyze.Clicked += async (sender, args) =>
				{
					if (File.Exists(filepath))
					{
						// Call the REST API method.
						MakeAnalysisRequest(filepath).Wait();
					}
					else
					{
						Debug.Write("\nInvalid file path");
					}

				};
			
			/// <summary>
			/// Makes a request to REST API to analyze the image and return the results in a JSON response
			/// </summary>
			/// <param name="imageFilePath">The image file to make request on.</param>
			async Task MakeAnalysisRequest(string imageFilePath)
			{			
				try
				{
					// Instantiate HttpClient class
					HttpClient client = new HttpClient();

					// Request headers.
					client.DefaultRequestHeaders.Add(
						"Ocp-Apim-Subscription-Key", subscriptionKey);

					// Request parameters.
					// The Analyze Image method returns information about the following
					// visual features:
					// Description: describes the image content with a complete
					//              sentence in supported languages.

					string requestParameters =
						"visualFeatures=Description";

					// Assemble the URI for the REST API method.
					string uri = uriBase + "?" + requestParameters;

					HttpResponseMessage response;

					// Read the contents of the specified local image
					// into a byte array.
					byte[] byteData = GetImageAsByteArray(imageFilePath);
					// Add the byte array as an octet stream to the request body.
					using (var content = new ByteArrayContent(byteData))
					{
						// This example uses the "application/octet-stream" content type.
						// The other content types you can use are "application/json"
						// and "multipart/form-data".
						content.Headers.ContentType =
							new MediaTypeHeaderValue("application/octet-stream");

						// Asynchronously call the REST API method.
						response = await client.PostAsync(uri, content).ConfigureAwait(false);
					}

					// Asynchronously get the JSON response.
					string contentString = await response.Content.ReadAsStringAsync();
					// If the contentString is not empty
					if (contentString != "")
					{
						// Deserializes the JSON to a .NET object.
						Photo = JsonConvert.DeserializeObject<Photo>(contentString);
						// Add Photo to list of Photos
						photos.Add(Photo);
						// Since DataBinding is failing for me,
						// I'm gonna have to display response as an alert!
						Device.BeginInvokeOnMainThread(() =>
						{
							DisplayAlert("Photo Description", contentString, "OK");
						});
						
					}
					// Display the JSON response
					Debug.Write("\nResponse:\n\n{0}\n",
							JToken.Parse(contentString).ToString());
				}
				catch (Exception e)
				{
					Debug.Write("\n" + e.Message);
				}
				// Save the photos to a file
				SavePhotosData(photos);
			}
			
			/// <summary>
			/// Returns the contents of the specified file as a byte array.
			/// </summary>
			/// <param name="imageFilePath">The image file to read.</param>
			/// <returns>The byte array of the image data.</returns>
			byte[] GetImageAsByteArray(string imageFilePath)
			{
				// Open a read-only file stream for the specified file.
				using (FileStream fileStream =
					new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
				{
					// Read the file's contents into a byte array.
					BinaryReader binaryReader = new BinaryReader(fileStream);
					return binaryReader.ReadBytes((int)fileStream.Length);
				}
			}
			
			/// <summary>
			/// Stores the contents of the List of photos to a file file 
			/// </summary>
			/// <param name="saveList">The list of objects you want to save.</param>
			void SavePhotosData(ObservableCollection<Photo> saveList)
			{
				// need the path to the file
				string path = Environment.GetFolderPath(
					Environment.SpecialFolder.LocalApplicationData);
				string filename = Path.Combine(path, JSON_PHOTOS_FILE);
				// use a stream writer to write the list
				using (var writer = new StreamWriter(filename, false))
				{
					// Converts object into JSON string
					string jsonText = JsonConvert.SerializeObject(saveList);
					// Write line to file
					writer.WriteLine(jsonText);
				}
			}


			void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				if (files.Count == 0)
				{
					ImageList.Children.Clear();
					return;
				}
				if (e.NewItems.Count == 0)
					return;

				var file = e.NewItems[0] as MediaFile;
				var image = new Image { WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.AspectFit };
				image.Source = ImageSource.FromStream(() =>
				{
					var stream = file.GetStream();
					return stream;
				});
				ImageList.Children.Add(image);
			}
		}
		
		}
	}
