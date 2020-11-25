CwsSystemInfo

Crestron System Info via Web

## Overview 

This is a project that can be used to interact with Crestrons Sandbox version of an API called Crestron CWS(Crestron Web Scripting).  Very useful in my use-case as a Mac/Linux user to get most of the info from a Crestron Control System without needing to fire up a Windows VM just to utilize Crestron Toolbox to get an overview of the system.

An html file can be created and loaded on the processor.  for example if the processor's IP is 10.10.10.41 and the html file is called 'sysinfo.html', you visit 10.10.10.41/sysinfo.html in your browser to view the data

## Requirements

This was created in C# / Simpl# Pro for Crestron
In order to compile or make changes to the ControlSystem.cs file the following is required:

For 3-Series Processors:
- Visual Studio 2008 Professional with SP1
- Visual Studio Simpl# Pro PlugIn from Crestron

4-series development you can download the Nuget package via Rider or VisualStudio 2019

SCREENSHOT OF WEBPAGE
