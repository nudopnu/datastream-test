using System.Diagnostics;
using System.Threading.Channels;
using ViconDataStreamSDK.DotNET;

var client = new Client();
string host = "localhost:801";

client.Connect(host);
client.SetStreamMode(StreamMode.ServerPush);

// Set proper orientation
client.SetAxisMapping(Direction.Left, Direction.Up, Direction.Forward);

// Enable all the data types
// client.EnableLightweightSegmentData();
// client.EnableSegmentData();
client.EnableMarkerData();
// client.EnableUnlabeledMarkerData();
// client.EnableMarkerRayData();
// client.EnableDeviceData();
// client.EnableCentroidData();

int logEveryNFrames = 20; // log once per second at 200Hz
int frameCounter = 0;

var logQueue = Channel.CreateUnbounded<string>();

_ = Task.Run(async () =>
{
    await foreach (var message in logQueue.Reader.ReadAllAsync())
    {
        Console.WriteLine(message);
    }
});

while (true)
{
  var sw = Stopwatch.StartNew();
  // should be blocking:
  var result = client.GetFrame().Result;
  if (result == Result.NoFrame)
  {
    continue;
  }
  var frameNumber = client.GetFrameNumber().FrameNumber;
  // Console.WriteLine($"{result}: Got a frame {frameNumber}");
  sw.Stop();
  // Console.WriteLine($"Loop took {sw.ElapsedMilliseconds} ms");
  if (++frameCounter % logEveryNFrames == 0)
  {
    logQueue.Writer.TryWrite($"Tick at frame {frameNumber}, took {sw.ElapsedMilliseconds}ms");
  }
}

