echo "Running C++ version..."
(
    cd cpp/build
    cmake ..
    cmake --build . --config Release
    ./bin/Release/ViconClient.exe 10 > ../../results/cpp.csv
)

echo "Running .NET version..."
(
    cd dotnet
    dotnet run 10 > ../results/dotnet.csv
)

echo "Running Python version..."
(
    cd python
    python main.py 10 > ../results/python.csv
)

awk '
FNR == 1 { next }  # skip header line
{
    sum[FILENAME] += $2
    count[FILENAME]++
}
END {
    for (file in sum) {
        printf "%-20s avg = %.2f ms\n", file, sum[file] / count[file]
    }
}
' ./results/*.csv > results/results.txt