Image Processing using Microsoft Cognitive Services Computer Vision API

This project is designed for Android, iOS and UWP to take an image provided by the user and return information about the visual content found in the image. This application will allow you to analyse a locally stored image using the Computer Vision's REST API. The API will extract visual features and content based on that image. 

Steps: 
1. The user chooses to either take a photo using their camera, or upload an image stored in their gallery/documents.
2. The image is then displayed on the screen to show the user which photo they have chosen.  
3. Then the 'Analyze' button should be pressed to make the call to the API and analyze the image of their choice. 
4. Once the all of the processing has completed, the details SHOULD be displayed under the image.. 
But unfortunately after searching and trying every single way that must be possible to bind the data to the XAML page for displaying, I still don't understand the internals of how it all works and after 2 whole days of working on the binding alone, I decided to put it at rest and come back to it which of course I never ended up having enough time to do. So I was left with the only option of just using an Alert to display the JSON data to the user. 
It doesn't look great, but it was my only option in the end so I guess it will have to do until I work on understanding Xamarin data binding.


How it's done 

1. Allowing user to take/upload photos

We never touched on using camera capabilities in class so after a few Google searches and reading of articles, I stumbled across this gem of a repository on github. 

https://github.com/jamesmontemagno/MediaPlugin

Basically its a program that provides you with all of the functionality needed for taking photos and picking them from a gallery which could be used cross platforms so using the same code for Android, iOS and UWP which sounded ideal for me.
So I decided to clone this repository and followed the steps for setting up. There and then I was able to use my laptop and phone to choose and take photos and they would then be displayed on the screen. 
I then starting messing about with the cloned project and was thinking of ideas on what I could do with images uploaded by the user. 
Since I had already started some work from just messing around with Xamarin and seeing what I could do with it, I decided to work from this project as it was just too difficult to try getting all of the content I was then using from the cloned project and set it up in a new project. So this is why the solution I have uploaded is the same name. It is some of the same code but not all the same as my project.

2. Setting up Azure account to create a Cognitive Service and obtain a Subscription Key 

To gain access to the Computer Vision API, you must have an active Azure subscription to create new services. 
You must login to your Azure account and in your Dashboard you have an option to create a new service.
This is where you can find the Cognitive Services Computer Vision API and set it up with your region and you will then be provided with the keys needed to use the services. 

Links that helped me with set up
https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/
https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/
https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/home

3. Get the analysis of the specified image file by using the Computer Vision REST API.

The cloud-based Computer Vision API provides developers with access to advanced algorithms for processing images and returning information. By uploading an image or specifying an image URL, Microsoft Computer Vision algorithms can analyze visual content in different ways based on inputs and user choices. Learn how to analyze visual content in different ways with quickstarts, tutorials, and samples.

This is done by creating a HttpClient object which will do all the communicating between the app and the API.
First the subscription ket and uriBase must be declared as they are needed for accessing the API.
You must use the same Azure region in your REST API method as you used to get your subscription keys.
Then the filepath for the image is passed the a method which deals with getting the analysis.
In this method, the HttpClient is created, it is given the headers and parameters needed for the request. 
There are many different parameters which can be passed, but I just kept it simple and used the one. 
The image is then converted into a byte array using a BinaryReader so the image can be passed to the API. 
Once this has all been done, the HttpResponseMessage should be returned with all of the information on the analysis of the image. 


This link really helped with coding it all into the program, but it is just code for generating a simple Console Application 
https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/QuickStarts/CSharp-analyze