using System.Diagnostics;
using ViconDataStreamSDK.DotNET;

var client = new Client();
string host = "localhost:801";

client.Connect(host);
client.SetStreamMode(StreamMode.ServerPush);

// Set proper orientation
client.SetAxisMapping(Direction.Left, Direction.Up, Direction.Forward);

// Enable necessary channels
client.EnableSegmentData();

// Test Setup
int frameCounter = 0;
int maxOutputFrames = 100;
if (args.Length > 0)
{
  maxOutputFrames = int.Parse(args[0]);
}

List<(uint, long)> frames = [];
while (frameCounter < maxOutputFrames)
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
  var elapsedTime = sw.ElapsedMilliseconds;

  frames.Add((frameNumber, elapsedTime));
  frameCounter += 1;
}

foreach (var (frameNumber, duration) in frames)
{
  Console.WriteLine($"{frameNumber}\t{duration}");
}