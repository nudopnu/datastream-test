#include "DataStreamClient.h"
#include <iostream>
#include <chrono>
#include <thread>
#include <atomic>
#include <queue>
#include <mutex>
#include <condition_variable>

using namespace ViconDataStreamSDK::CPP;
using namespace std;

atomic<bool> running{true};
queue<string> logQueue;
mutex logMutex;
condition_variable logCV;

void logWorker()
{
    while (running)
    {
        unique_lock<mutex> lock(logMutex);
        logCV.wait(lock, []
                   { return !logQueue.empty() || !running; });

        while (!logQueue.empty())
        {
            cout << logQueue.front() << endl;
            logQueue.pop();
        }
    }
}

void log(const string &msg)
{
    {
        lock_guard<mutex> lock(logMutex);
        logQueue.push(msg);
    }
    logCV.notify_one();
}

int main(int argc, char** argv)
{
    Client client;
    const string host = "localhost:801";

    auto result = client.Connect(host);
    if (result.Result != Result::Success)
    {
        cerr << "Failed to connect to Vicon server.\n";
        return 1;
    }

    client.SetStreamMode(StreamMode::ServerPush);
    client.SetAxisMapping(Direction::Left, Direction::Up, Direction::Forward);
    client.EnableLightweightSegmentData();
    client.EnableSegmentData();

    thread logger(logWorker);
    log("frameNumber\telapsedMilliseconds");

    int logEveryNFrames = 20;
    int frameCounter = 0;
    int outputFrameCounter = 0;
    int maxOutputFrames = 100;
    if (argc >= 2) {
        maxOutputFrames = stoi(argv[1]);
    }

    while (outputFrameCounter < maxOutputFrames)
    {
        auto start = chrono::steady_clock::now();

        if (client.GetFrame().Result != Result::Success)
        {
            continue;
        }

        const Output_GetFrameNumber frameInfo = client.GetFrameNumber();
        int frameNumber = frameInfo.FrameNumber;

        auto end = chrono::steady_clock::now();
        auto duration = chrono::duration_cast<chrono::milliseconds>(end - start).count();

        if (++frameCounter % logEveryNFrames == 0)
        {
            log(to_string(frameNumber) + "\t" + to_string(duration));
            outputFrameCounter++;
        }
    }

    running = false;
    logCV.notify_all();
    logger.join();
    return 0;
}