import sys, threading, time, queue
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

# Setup logging frequency
logEveryNFrames = 20
frameCounter = 0
outputFrameCounter = 0
maxOutputFrames = 100
if len(sys.argv) > 1:
    maxOutputFrames = int(sys.argv[1])

# Start a Thread to read strings for logging
log_queue = queue.Queue()
def log_worker():
    while True:
        msg = log_queue.get()
        if msg is None:
            break  # exit signal
        print(msg)
        log_queue.task_done()
threading.Thread(target=log_worker, daemon=True).start()
log_queue.put("frameNumber\telapsedMilliseconds")

while outputFrameCounter <= maxOutputFrames:
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

    frameCounter += 1
    if (frameCounter % logEveryNFrames == 0):
        log_queue.put(f"{frameNumber}\t{elapsedMilliseconds:.2f}")
        outputFrameCounter += 1