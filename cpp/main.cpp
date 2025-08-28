#include "DataStreamClient.h"
#include <iostream>
#include <chrono>
#include <utility>

using namespace ViconDataStreamSDK::CPP;
using namespace std;

int main(int argc, char **argv)
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

    // Set proper orientation
    client.SetAxisMapping(Direction::Left, Direction::Up, Direction::Forward);

    // Enable necessary channels
    client.EnableSegmentData();

    // Test Setup
    int frameCounter = 0;
    int maxOutputFrames = 100;
    if (argc >= 2)
    {
        maxOutputFrames = stoi(argv[1]);
    }

    vector<pair<int, double>> frames;
    while (frameCounter < maxOutputFrames)
    {
        auto start = chrono::steady_clock::now();

        if (client.GetFrame().Result != Result::Success)
        {
            continue;
        }

        const Output_GetFrameNumber frameInfo = client.GetFrameNumber();
        int frameNumber = frameInfo.FrameNumber;

        auto end = chrono::steady_clock::now();
        auto elapsedMilliseconds = chrono::duration_cast<chrono::milliseconds>(end - start).count();

        frames.emplace_back(frameNumber, elapsedMilliseconds);
        frameCounter++;
    }

    for (const auto &[frame, duration] : frames)
    {
        cout << frame << "\t" << duration << " ms" << endl;
    }

    return 0;
}