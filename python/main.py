import sys, time
from vicon_dssdk.ViconDataStream import Client

client = Client()
host = "localhost:801"

client.Connect(host)
client.SetStreamMode(Client.StreamMode.EServerPush)

# Set proper orientation
client.SetAxisMapping(Client.AxisMapping.ELeft, Client.AxisMapping.EUp, Client.AxisMapping.EForward)

# Enable necessary channels
client.EnableLightweightSegmentData()
client.EnableSegmentData()

# Test Setup
frameCounter = 0
maxOutputFrames = 100
if len(sys.argv) > 1:
    maxOutputFrames = int(sys.argv[1])

frames: list[tuple[int, int]] = []
while frameCounter <= maxOutputFrames:
    # Start timer
    start = time.time()

    # Should be blocking:
    result = client.GetFrame()
    if not result:
        continue

    # On success get some data and stop timer
    frameNumber = client.GetFrameNumber()
    end = time.time()
    elapsedMilliseconds = (end - start) * 1000

    frames.append((frameNumber, elapsedMilliseconds))
    frameCounter += 1

for frame_number, duration in frames:
    print(f"{frame_number}\t{duration}")