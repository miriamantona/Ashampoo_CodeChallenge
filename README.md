# Readme

## Introduction

This application lists all the drives and logical volumes on the user's system. Upon clicking a button, the app searches through all the files on the selected drive/volume and finds all directories that directly contain individual files larger than 10 MB.
Processing is done in multiple threads in parallel to increase performance. It is possible to pause and resume the search by pressing a button.
Any discovered directory is immediately shown in a result list in the main window while the search is still running. The app also displays two numbers for each found directory: the count of all files (of any size) in the directory, as well as the combined size
of these files. The contents of sub-directories are not included in these numbers.

The app remains responsive at all times. Any changes in the data or the result list do not cause any flickering or disruptions to the layout.

The application uses asynchronous programming and multithreading to perform the search operation in parallel, ensuring responsiveness even during resource-intensive tasks.


## Getting Started

1. Clone the repository to your local machine.
2. Open the project in Visual Studio or your preferred C# IDE.
3. Build and run the application.

## Usage

1. Launch the application.
2. The main window will display a list of drives and logical volumes.
3. Click the "Select Folder" button to choose a drive/volume for searching.
4. The search operation will start automatically. The progress will be displayed in real-time.
5. Discovered directories containing large files will be listed along with file count and total size.
6. Use the "Pause" button to temporarily halt the search, and "Resume" to continue.
7. The application will remain responsive throughout the process.
