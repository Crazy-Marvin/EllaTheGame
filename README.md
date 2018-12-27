# Ella - The Game

> Ella - The Game is an open source endless runner for Android, Linux, Windows, macOS & WebGL.

[![Travis](https://img.shields.io/travis/Crazy-Marvin/EllaTheGame.svg?style=flat-square)](https://travis-ci.org/Crazy-Marvin/EllaTheGame)
![Azure Pipelines](https://img.shields.io/azure-devops/build/Crazy-Marvin/4bc2392e-76d1-4d7a-9249-c8bdc7cd3747/1.svg?style=flat-square)
[![License](https://img.shields.io/github/license/Crazy-Marvin/EllaTheGame.svg?style=flat-square)](LICENSE.txt)
[![Last commit](https://img.shields.io/github/last-commit/Crazy-Marvin/EllaTheGame.svg?style=flat-square)](https://github.com/Crazy-Marvin/EllaTheGame/)
[![Releases](https://img.shields.io/github/downloads/Crazy-Marvin/EllaTheGame/total.svg?style=flat-square)](https://github.com/Crazy-Marvin/EllaTheGame/releases)
![Codecov](https://img.shields.io/codecov/c/github/Crazy-Marvin/EllaTheGame.svg?style=flat-square)

# About

Ella, the smart and adorable dog, is lost and is doing whatever it takes to find the way out. In this free jumping and running dog game, Ella is running automatically and as the screen is moving forward you need to jump over the obstacles and collect as many coins and gifts as you can. So, you’ve got two objectives in this side-scrolling adventure game: avoid hitting obstacles and hurdles and collect coins.

![Ella - The Game GIF](https://media.giphy.com/media/3oeNXAHQjYmLSvdzUU/giphy.gif)
        
# Installation

You can get the Android, Linux, Windows, macOS & WebGL [builds on GitHub](https://github.com/Crazy-Marvin/EllaTheGame/releases/), click on the badges below or build it yourself. Experimental builds are available for [UWP](https://youtu.be/f20saEKeSpE) ([Windows](https://www.microsoft.com/windows) 10 Desktop/Mobile & [Xbox](https://www.xbox.com/)) and [Apple](https://www.apple.com/) iOS.
It is not planned to support other [platforms](https://unity3d.com/unity/features/multiplatform) than Android, Linux, Windows, macOS & WebGL from my side, but Unity offers the possibility to build the game for many other platforms too and there might be one or another experimental build.

<a href="https://play.google.com/store/apps/details?id=rocks.poopjournal.Ella">
    <img alt="Get it on Google Play"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36810046-fa306856-1cc9-11e8-808e-6eb8a81783c7.png" />
        </a>  
<a href="https://f-droid.org/packages/rocks.poopjournal.Ella/">
    <img alt="Get it on F-Droid"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36919296-19b8524e-1e5d-11e8-8962-48463b1cec8a.png" />
        </a>
<a href="https://poopjournal.rocks/EllaTheGame/play/">
    <img alt="Get it on WebGL"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36810049-fac5dc74-1cc9-11e8-81e5-a2565ffd1d83.png" />
        </a>  
        <br>
 <a href="https://github.com/Crazy-Marvin/EllaTheGame/releases/">
    <img alt="Get it on Linux"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36810047-fa774906-1cc9-11e8-94da-ec2db1c37813.png" />
        </a>
  <a href="https://github.com/Crazy-Marvin/EllaTheGame/releases/">
    <img alt="Get it on Windows"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36810050-faf040c2-1cc9-11e8-8ace-b32a036cab81.png" />
        </a>
  <a href="https://github.com/Crazy-Marvin/EllaTheGame/releases/">
    <img alt="Get it on macOS"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36919363-43e3acee-1e5d-11e8-8378-5a313c27320a.png" />
        </a> 
     
     
:exclamation: ___Experimental:___

__Those experimental builds are done and uploaded by a third party.__

<a href="https://github.com/Crazy-Marvin/EllaTheGame/releases/">
    <img alt="Get it Xbox"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36939531-43710c04-1f32-11e8-8ef3-a77743570306.png" />
        </a>
<a href="https://github.com/Crazy-Marvin/EllaTheGame/releases/">
    <img alt="Get it on iOS"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36919325-304905bc-1e5d-11e8-9a11-fd61610049e3.png" />
        </a> 
<a href="https://github.com/Crazy-Marvin/EllaTheGame/releases/">
    <img alt="Get it on macOS"
        height="80"
        src="https://user-images.githubusercontent.com/15004217/36810048-fa97faf2-1cc9-11e8-88b4-adf66d6e2cc6.png" />
        </a> 


# Building

## Building for ANDROID

### Setting up the Android SDK Tools
- Go to http://www.oracle.com/technetwork/java/javase/downloads/index.html to download the most recent JDK.    Choose the one with the highest version number.
- Simply run the installer and follow instructions in the wizard to install it.

NEXT WE NEED TO INSTALL THE ANDROID SDK TOOLS
- Go to http://developer.android.com/sdk/index.html
- Download the Android SDK Tools or (the command line tools), not the full android studio just the command line tools
- Unzip the downloaded file, put the directory in an accessible location, you’ll need to tell Unity where is this  directory later
- Open the directory that contains the Android SDK Tools, and navigate to tools. Double click the file called android to run it.
- Click Install [x] packages to start the installation process. You will be prompted to accept the licenses for these packages.
- The installation will take some time to complete.
### Now we will tell Unity where we installed the Android SDK Tools.
- Go to top menu, navigate to Unity > Preferences (on OSX) or Edit > Preferences (on Windows).
- In the Preferences window, navigate to External Tools, and scroll down to Android section.
- Where it says SDK, click Browse, navigate to where you put the directory containing Android SDK Tools and click Choose the path may look like this (C:/Users/[userName]/AppData/Local/Android/sdk).
- Where it says JDK, click Browse, navigate to where you put the directory containing JDK Tools and click Choose the path may look like this (C:/Program Files/Java/jdk1.8.0_121).
### Finally, Preparing your Unity project for building to Android
 - In Unity, open the Build Settings from the top menu (File > Build Settings).
 - Select Android from the list of platforms on the left and choose Switch Platform at the bottom of the window.
 - Open the player settings in the inspector panel (Edit > Project Settings > Player).
 - Expand the section at the bottom called Other Settings, and enter your chosen Package Name where it says Package Name.
 - Now comeback open the Build Settings from the top menu (File > Build Settings), and click on Build.
 for detailed tutorial about building to ANDROID visit this link https://unity3d.com/learn/tutorials/topics/mobile-touch/building-your-unity-game-android-device-testing
 
 ## Building for iOS
 
 Building for iOS is a long process, but Unity created a good Guide for building for iOS.
 - First, you should have a Mac and Xcode installed. If you don't have a Mac install it on a Virtual Machine or rent an online server and install Xcode. Once you did that follow this Unity tutorial.
 https://unity3d.com/learn/tutorials/topics/mobile-touch/building-your-unity-game-ios-device-testing

 
  ## Building for WINDOWS
  
  - In Unity, open the Build Settings from the top menu (File > Build Settings).
  - In Platforms Make sure to select (PC, MAC & LinuxStabdalone)
  - In Target Platform Select Windows then click Build.
  
  ## Building for LINUX
  
  - In Unity, open the Build Settings from the top menu (File > Build Settings).
  - In Platforms Make sure to select (PC, MAC & LinuxStabdalone)
  - In Target Platform Select Linux then click Build.
  
  ## Building for MAC OS X
  
  - In Unity, open the Build Settings from the top menu (File > Build Settings).
  - In Platforms Make sure to select (PC, MAC & LinuxStabdalone)
  - In Target Platform Select MACOSX then click Build.
  
    
  ## Building for WEBGL

- In Unity, open the Build Settings from the top menu (File > Build Settings).
- In Platforms Make sure to select Webgl and click switch platform
- Click Build.
  
# Contribute
  
Contributions are always welcome! Please read the [contribution guidelines](https://github.com/Crazy-Marvin/EllaTheGame/blob/master/CONTRIBUTING.md) first.
  
# License
  
[MIT](https://www.tldrlegal.com/l/mit) © Crazy Marvin
 
 
