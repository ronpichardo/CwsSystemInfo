# CwsSystemInfo

Crestron System Info via Web

## Overview 

This is a project that can be used to interact with Crestrons Sandbox version of an API called Crestron CWS(Crestron Web Scripting).  Very useful in my use-case as a Mac/Linux user to get most of the info from a Crestron Control System without needing to fire up a Windows VM just to utilize Crestron Toolbox to get an overview of the system.

An html file can be created and loaded on the processor.  for example if the processor's IP is 10.10.10.41 and the html file is called 'sysinfo.html', you visit 10.10.10.41/sysinfo.html in your browser to view the data

Once the program is compiled, it can be loaded to any other slot of a Crestron Processor.  It does not affect the currently running program, which is great since the program can run independently as an overview of the current system.

Example of loading Compiled Code via the Crestron Toolbox Software:
1. Select the menu item under program in order to get the pop up shown in the image below
2. Click on "Browse.."
![uploadingprogram](https://user-images.githubusercontent.com/63974878/104031729-1fc95100-519b-11eb-9e1d-4c0cf20d4846.png)

3. Find the path to the compiled "ShowVersion.cpz" file, shown in the image below( you may need to click the file type to show cpz, lpz is the default )
4. Select "Open", and then hit Send
![SelectImage](https://user-images.githubusercontent.com/63974878/104031774-2e176d00-519b-11eb-9fe8-232a7a3e8b68.png)

## Requirements

This was created in C# / Simpl# Pro for Crestron
In order to compile or make changes to the ControlSystem.cs file the following is required:

For 3-Series Processors:
- Visual Studio 2008 Professional with SP1
- Visual Studio Simpl# Pro PlugIn from Crestron

4-series development you can download the Nuget package via Rider or VisualStudio 2019

## Example from Webpage
![image](https://user-images.githubusercontent.com/63974878/100181601-a30f3b80-2ea8-11eb-9d68-c8e073785a6f.png)

## Postman example of hitting the API Route

Here is an example of using Postman to make sure the route is working prior to creating an html file for displaying the data

![image](https://user-images.githubusercontent.com/63974878/100182142-eddd8300-2ea9-11eb-96c0-ac8bd7ce2d91.png)
