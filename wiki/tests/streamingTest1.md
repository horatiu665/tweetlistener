[Prev](restTest1.md)[Next](streamingTest2.md)
# Streaming Test

Not possible with Twitter Console app

Could not gain access through the Console app to the `stream.twitter.com/1.1/` service. Any attempts to use the requests which do not require special permission resulted in response failure, some error or endless “working...” loading time.
The firehose request is only allowed by special permissions. It theoretically is a streaming live feed of tweets, but it might be too much to process for a simple prototype app.

The reason for streaming not working in Console is probably because it is a continuous data flow rather than a limited one that can be displayed in the output. I must devise another type of test in order to understand how the Streaming API is used to gather Tweets. Since the REST API is a good first step towards gathering data, it could be used as a prototype, but it might be worth investigating the Streaming just a little more before implementing anything, because it is referenced in many sources as the proper way of dealing with a large amount of data.

Project where Streaming is used and described: 

https://medium.com/@mroth/how-i-built-emojitracker-179cfd8238ac 

## C# and tweetinvi

The C# implementation was pretty straightforward using a library called [tweetinvi](http://tweetinvi.codeplex.com ) which handles most of the request making and unpacking of data. A console app was developed in which a stream of tweets is simply printed to the console, along with examination of events firing. The library is open-source, but there were some difficulties when trying to recompile it so I gave up on that. The documentation is limited, but it is compensated by a great feedback/support forum and great coding style.

A further attempt could be to connect this with a web application in which the tweets could be used for something specific, such as a game, data visualization tool or other purpose.

Info about filters to the data is available in the [official docs](https://dev.twitter.com/streaming/overview/request-parameters#track )

[Prev](restTest1.md)[Next](streamingTest2.md)
