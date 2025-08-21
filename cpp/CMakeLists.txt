cmake_minimum_required(VERSION 3.10)
project(ViconClient)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

# Source file
add_executable(ViconClient main.cpp)

# Include current dir for headers
target_include_directories(ViconClient PRIVATE ${CMAKE_CURRENT_SOURCE_DIR})

# Link the Vicon SDK static import library
target_link_libraries(ViconClient PRIVATE
    ${CMAKE_CURRENT_SOURCE_DIR}/ViconDataStreamSDK_CPP.lib
)

# On build, copy all required DLLs next to the executable
add_custom_command(TARGET ViconClient POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
        ${CMAKE_CURRENT_SOURCE_DIR}/ViconDataStreamSDK_CPP.dll
        ${CMAKE_CURRENT_SOURCE_DIR}/boost_chrono-vc141-mt-x64-1_75.dll
        ${CMAKE_CURRENT_SOURCE_DIR}/boost_filesystem-vc141-mt-x64-1_75.dll
        ${CMAKE_CURRENT_SOURCE_DIR}/boost_python27-vc141-mt-x64-1_75.dll
        ${CMAKE_CURRENT_SOURCE_DIR}/boost_system-vc141-mt-x64-1_75.dll
        ${CMAKE_CURRENT_SOURCE_DIR}/boost_thread-vc141-mt-x64-1_75.dll
        $<TARGET_FILE_DIR:ViconClient>
)
