# Search_file_extension_delete
C# Program that will search for a file based on it's extension, logs it, then allows you to delete it
![alt tag](https://raw.githubusercontent.com/omgdanieltam/Search_file_extension_delete/master/find-delete.png)

#INTRODUCTION
I needed a program that will search down through directories for a file with a certain extension and log the exact location of the file but Windows Search was not working out to me that greatly. I decided to create my own program that will search through directories and find the files I want. The idea was mainly for ransomware (Cryptolocker) since those always changed the file's extension. Once we know where the files were, we could run a restore from an old backup and we will know exactly where the files are located at.

#HOW IT WORKS
You will input the top directory and the file extension (without the '.'). It should then ask for a location to save the text file with the location of files it found. From there, it will recursively walk through the subdirectories checking each and every file. Since we have a GUI, we will run it on a second thread (background worker) so that we can keep the GUI up to date with the current location.

Once the file is found, we will save it into a List<String>. Once we've gone through all the files and folders, it will then output the list into the text file from earlier. It should then give an option to delete the files. If you decide to delete the files, it will run through the list again and remove the files.

#SOURCES
https://zetalongpaths.codeplex.com/ -- ZetaLongPaths; .NET library for long file paths (exceeding 260 characters)
