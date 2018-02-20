#! /bin/sh

project="EllaTheGame"
# NOTE the command args below make the assumption that your Unity project folder is
#  a subdirectory of the repo root directory, e.g. for this repo "unity-ci-test" 
#  the project folder is "UnityProject". If this is not true then adjust the 
#  -projectPath argument to point to the right location.

## Run the editor unit tests


## Make the builds
# Recall from install.sh that a separate module was needed for Windows build support
echo "Attempting build of $project for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity 
	-batchmode 
	-nographics 
	-silent-crashes 
	-logFile $(pwd)/unity.log 
	-projectPath $(pwd) 
	-buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" 
	-quit

rc1=$?
echo "Build logs (Windows)"
cat $(pwd)/unity.log

echo "Attempting build of $project for the OSX"
/Applications/Unity/Unity.app/Contents/MacOS/Unity 
	-batchmode 
	-nographics 
	-silent-crashes 
	-logFile $(pwd)/unity.log 
	-projectPath $(pwd) 
	-buildOSXUniversalPlayer "$(pwd)/Build/osx/$project.app" 
	-quit

rc2=$?
echo "Build logs (OSX)"
cat $(pwd)/unity.log

exit $(($rc1|$rc2))