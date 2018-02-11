# Ella - The Game

[![Build Status](https://travis-ci.org/Crazy-Marvin/EllaTheGame.svg?branch=master)](https://travis-ci.org/Crazy-Marvin/EllaTheGame)
[![Build Status Windows](https://ci.appveyor.com/api/projects/status/qqhmlau1cvom1d4c?svg=true)](https://ci.appveyor.com/project/CrazyMarvin/ellathegame)
[![Build Status Mobile](https://www.bitrise.io/app/ff4291ef82a9aa0f/status.svg?token=BPHko7oZ426o6PJNZtKV8A&branch=master)](https://www.bitrise.io/app/ff4291ef82a9aa0f)
[![License](https://img.shields.io/github/license/Crazy-Marvin/EllaTheGame.svg)](LICENSE.txt)
[![Last commit](https://img.shields.io/github/last-commit/Crazy-Marvin/EllaTheGame.svg)](https://github.com/Crazy-Marvin/EllaTheGame/)
[![Releases](https://img.shields.io/github/downloads/Crazy-Marvin/EllaTheGame/total.svg)](https://github.com/Crazy-Marvin/EllaTheGame/releases)
[![codecov](https://codecov.io/gh/Crazy-Marvin/EllaTheGame/branch/master/graph/badge.svg)](https://codecov.io/gh/Crazy-Marvin/EllaTheGame)

# Building for ANDROID

## Setting up the Android SDK Tools
- Go to http://www.oracle.com/technetwork/java/javase/downloads/index.html to download the most recent JDK.    Choose the one with the highest version number.
- Simply run the installer and follow instructions in the wizard to install it.

NEXT WE NEED TO INSTALL THE ANDROID SDK TOOLS
- Go to http://developer.android.com/sdk/index.html
- Download the Android SDK Tools or (the command line tools), not the full android studio just the command line tools
- Unzip the downloaded file, put the directory in an accessible location, you’ll need to tell Unity where is this  directory later
- Open the directory that contains the Android SDK Tools, and navigate to tools. Double click the file called android to run it.
- Click Install [x] packages to start the installation process. You will be prompted to accept the licenses for these packages.
- The installation will take some time to complete.
## Now we will tell Unity where we installed the Android SDK Tools.
- Go to top menu, navigate to Unity > Preferences (on OSX) or Edit > Preferences (on Windows).
- In the Preferences window, navigate to External Tools, and scroll down to Android section.
- Where it says SDK, click Browse, navigate to where you put the directory containing Android SDK Tools and click Choose the path may look like this (C:/Users/[userName]/AppData/Local/Android/sdk).
- Where it says JDK, click Browse, navigate to where you put the directory containing JDK Tools and click Choose the path may look like this (C:/Program Files/Java/jdk1.8.0_121).
## Finally, Preparing your Unity project for building to Android
 - In Unity, open the Build Settings from the top menu (File > Build Settings).
 - Select Android from the list of platforms on the left and choose Switch Platform at the bottom of the window.
 - Open the player settings in the inspector panel (Edit > Project Settings > Player).
 - Expand the section at the bottom called Other Settings, and enter your chosen Package Name where it says Package Name.
 - Now comeback open the Build Settings from the top menu (File > Build Settings), and click on Build.
 for detailed tutorial about building to ANDROID visit this link https://unity3d.com/learn/tutorials/topics/mobile-touch/building-your-unity-game-android-device-testing
 
 # Building for IOS
 
 Building for IOS is a long process, but Unity created a good Guide for building for IOS.
 - First, you should have a Mac and Xcode installed. If you don't have a mac install it on a Virtual Machine or rent an online server and install Xcode. Once you did that follow this Unity tutorial.
 https://unity3d.com/learn/tutorials/topics/mobile-touch/building-your-unity-game-ios-device-testing
 
  # Building for WINDOWS
  
  - In Unity, open the Build Settings from the top menu (File > Build Settings).
  - In Platforms Make sure to select (PC, MAC & LinuxStabdalone)
  - In Target Platform Select Windows then click Build.
  
# Building for LINUX
  
  - In Unity, open the Build Settings from the top menu (File > Build Settings).
  - In Platforms Make sure to select (PC, MAC & LinuxStabdalone)
  - In Target Platform Select Linux then click Build.
  
# Building for MAC OS X
  
  - In Unity, open the Build Settings from the top menu (File > Build Settings).
  - In Platforms Make sure to select (PC, MAC & LinuxStabdalone)
  - In Target Platform Select MACOSX then click Build.
  
    
# Building for WEBGL

- In Unity, open the Build Settings from the top menu (File > Build Settings).
- In Platforms Make sure to select Webgl and click switch platform
- Click Build.
  
 
 
