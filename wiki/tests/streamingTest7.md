# New test 19-02-2015
- Stress test, using WPF application with in-app logging
- Filter: google. runtime: `2015-02-19 12:41:05 - 2015-02-26 09:36:43`
- Tweets harvested: 2522472 
- Errors: many
- Tweets missed: >2000, probably way more because of disconnects that lasted for a long while (few hours at most) - needs further analysis of logs
- Disconnects were handled semi-gracefully (via brutal reconnects), and the application was running fine at the end of the test.

## Problems
- Log file: 1.19 GB - quite excessive. from now on, log must be split into multiple files.

Several error types:
```
System.ArgumentNullException at Tweetinvi.Streams.Helpers.StreamResultGenerator.<StartStreamAsync>d__7.MoveNext()
```
  - this likely occurs when the stream was not started yet reads were attempted, and it is a Tweetinvi problem
  - this happens because the stream is assigned as follows: _currentReader = await InitWebRequest(_currentWebRequest); at 110

```
System.IO.IOException: Received an unexpected EOF or 0 bytes from the transport stream. 
```

  - this one is new. it also happens in StreamResultGenerator when assigning var jsonResponse = requestTask.Result; at 142

```
Exception at StartStreamTask() thread: System.Threading.Tasks.TaskCanceledException: A task was canceled.
```

  - this happens multiple times, and sometimes the error is printed multiple times in a row in the same stack trace, meaning that either 1. the event is listened to by the same function multiple times, or 2. the thread is calling itself and somehow the recursion can be traced leading to long error messages.

```
System.NotSupportedException: The stream does not support concurrent IO read or write operations. at StreamResultGenerator.cs:line 142
```

- Not an error, but Limit messages: between `20-02-2015 17:01:33 and 22-02-2015 00:00:27`, many *“Tweets missed: x”* messages were received, where x incremented from 1 to 743 before disconnecting a second after the last message. This 2 day period of lost tweets was also present later twice, in the periods `22-02-2015 00:03:19 - 23-02-2015 23:12:54` and `23-02-2015 23:56:12 - 26-02-2015 04:33:25`. many tweets were saved during this period so the limit messages were functioning as expected.
- There was another error, which prevented the logs from writing the stream counters after the first disconnect. might have something to do with the reconnection of the stream, as most of the events are never rebound.

```
Exception at StartStreamTask() thread: System.Threading.Tasks.TaskCanceledException: A task was canceled.
```
  - repeatedly many times, at very short intervals. most likely my messing around with “Restart Stream” button this morning.

