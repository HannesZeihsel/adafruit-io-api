# .Net Library to use Adafruit IO API

This is my first public repository, so I appreciate any advice you have regarding the setup on github as well as the code and its documentation/comments itself or the general Design of the Library. 
If anyone wants to collaborate and work on this project please feel free to make comments, pull requests, etc. and contact me if you cannot get the project running. Every contribution will be appreciated and don't be afraid if you’re new and unsure, as I am a beginner myself, so don't hold back with questions and suggestions.

## Warning: Still in Development
> **Warning:** This Project is at its beginning, so only a few functionalities are currently supported and the design might change a lot. So new versions might break existing code. Feel free to use this Library for your own projects and but please keep that in mind when using the Library.

## Scope
This Library is intended to be used to provide easy access to your Adafruit IO Data. All the API functionality is exposed to be used easily. Additional abstraction is made, to provide an easy way to get the data you want into your Adafruit IO Feed and also back out of it. It is stored as .Json, so this can easily be used if other applications that are not using this library are accessing your feeds. See also the [How to Use](#HowToUse) section.
### Features 
Currently, the following features are supported:

In branch `Main`:

In branch `Development`:

## <a name="HowToUse"></a> How to Use
### Setup
To use this Library download it and add it to your Solution in Visual Studio (Or the IDE of your choice), and from your project add a reference to it in order to use it.
The code itself is documented, so you can generate 
First, you need to create your Adafruit IO Account Data (see https://io.adafruit.com/api/docs/#authentication)

    AdafruitIOAccount account = new AdafruitIOAccount("Username", "Key");
Then you can create a new Client to interact with Adafruit IO via the Adafruit IO HTTP API:

    AdafruitIOHttpClient client = new AdafruitIOHttpClient(account);
Then you are all set to start writing and reading to/from your feeds.
### Write Data To Feed
Consider you created a feed with the key/name "TestFeed".
To write simple string data to it you can use:

    await client.CreateDataAsync("testFeed", "testData")
And to write a more complex Type stored as .json to the feed you can use:

    Person p = new Person() { Name = "Doe", Vorname = "John" };
    await client.CreateDataAsync<Person>("TestFeed", p);
### GetDataFromFeed
To get the Data Back from your fictional feed "TestFeed" you can use the following:

    List<DataPoint<string>> data = await client.GetDataAsync<string>("TestFeed");
This will return a `List` of `DataPoint`s that contain the Data from the Feed in the Property `DataPoint.Value` as well as additional metadata about each `DataPoint` of your feed like the Expiration `DataPoint.Expiration` and more. 
If you know your Data Object is stored as a .Json string inside the Feed and you want to get it back already converted you may use this:

    List<DataPoint<Person>> data = await client.GetDataAsync<Person>("TestFeed");
In this case the Property `DataPoint.Value` of the `DataPoint`s in the `List` will be of type Person, filled with the converted Data from your feed.

Check the additional parameters of this method to narrow down your search to specific time periods or limit the number of results.

## Use Unit Tests
If you want to make use of the unit tests in this project (because you want to use them on your own or want to contribute) you have to change the file `AdafruitAccountData.cs` and change the following lines to reflect your own Adafruit IO Account Data (You have to create at least one feed in your account and paste its name/key here):

     //in AdafruitAccountData.cs change this to your data:
     public static readonly string AccountKey = "___Your KEY___";
     public static readonly string AccountUsername = "___Your Username___";
	 public static readonly string FeedName = "___Your Feed Name___";
Find out more about this at https://io.adafruit.com/api/docs/#authentication 

Rename the class and the File to AdafruitIOAccountData. This file will then be used to get the account credentials. Be careful to not upload this file with sensitive data (The file was added to the .gitignore list, but make sure that you do get the name right when renaming and check that it is not in the 'Changes' List)
(Unfortunately i could not reuse the same file as a file tracked by git cannot (easily) be untracked again, so I had to use this methode.)

## Intended Design
The design I currently try to archive is that I provide some simple implementation for each API command of the Adafruit IO API in its respective namespaces MQTT and HTTP. And then to provide even more abstraction in a combined "Top Level" representation of I .e.. feeds, that then enables the user to create new feeds, etc. with HTTP and implement triggers for new values, etc. with MQTT.
	
    AdafruitIO
    |    |-- Mqtt
    |    |    |-- MqttClient
    |    |    |--  ...
    |    |-- Http
    |    |    |-- HttpClient
    |    |    |-- ...
    |    |-- Feed
    |    |-- Groups
    |    |-- AdafruitIOAccount
    |    |-- Other top level functionality

## Version History
### V0.0.0