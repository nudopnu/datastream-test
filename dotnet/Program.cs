using System.Diagnostics;
using ViconDataStreamSDK.DotNET;
using System.Threading.Channels;

var client = new Client();
string host = "localhost:801";

client.Connect(host);
client.SetStreamMode(StreamMode.ServerPush);

// Set proper orientation
client.SetAxisMapping(Direction.Left, Direction.Up, Direction.Forward);

// Enable necessary channels
client.EnableLightweightSegmentData();
client.EnableSegmentData();

// Setup logging frequency
int logEveryNFrames = 20;
int frameCounter = 0;
int outputFrameCounter = 0;
int maxOutputFrames = 100;

// Start a Thread to read strings for logging
var logQueue = Channel.CreateUnbounded<string>();
_ = Task.Run(async () =>
{
  await foreach (var message in logQueue.Reader.ReadAllAsync())
  {
    Console.WriteLine(message);
  }
});
logQueue.Writer.TryWrite("frameNumber\telapsedMilliseconds");

while (outputFrameCounter < maxOutputFrames)
{
  // Start timer
  var sw = Stopwatch.StartNew();

  // Should be blocking:
  var result = client.GetFrame().Result;
  if (result == Result.NoFrame)
  {
    continue;
  }

  // On success get some data and stop timer
  var frameNumber = client.GetFrameNumber().FrameNumber;
  sw.Stop();

  // Send a log every nth frame
  if (++frameCounter % logEveryNFrames == 0)
  {
    logQueue.Writer.TryWrite($"{frameNumber}\t{sw.ElapsedMilliseconds}");
    outputFrameCounter++;
  }
}

