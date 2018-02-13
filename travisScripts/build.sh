#! /bin/sh

# NOTE the command args below make the assumption that your Unity project folder is
#  a subdirectory of the repo root directory, e.g. for this repo "unity-ci-test" 
#  the project folder is "UnityProject". If this is not true then adjust the 
#  -projectPath argument to point to the right location.

## Run the editor unit tests
echo "Running editor unit tests for ${Ella-the-game}"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
	-logFile $(pwd)/unity.log \
	-projectPath "$(pwd)/${Ella-the-game}" \
	-runEditorTests \
	-editorTestsResultFile $(pwd)/test.xml \
	-quit

rc0=$?
echo "Unit test logs"
cat $(pwd)/test.xml
# exit if tests failed
if [ $rc0 -ne 0 ]; then { echo "Failed unit tests"; exit $rc0; } fi

## Make the builds
# Recall from install.sh that a separate module was needed for Windows build support
echo "Attempting build of ${Ella-the-game} for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
	-logFile $(pwd)/unity.log \
	-projectPath "$(pwd)/${Ella-the-game}" \
	-buildWindowsPlayer "$(pwd)/Build/windows/${Ella-the-game}.exe" \
	-quit

rc1=$?
echo "Build logs (Windows)"
cat $(pwd)/unity.log

echo "Attempting build of ${Ella-the-game} for the OSX"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
	-batchmode \
	-nographics \
	-silent-crashes \
	-logFile $(pwd)/unity.log \
	-projectPath "$(pwd)/${Ella-the-game}" \
	-buildOSXUniversalPlayer "$(pwd)/Build/osx/${Ella-the-game}.app" \
	-quit

rc2=$?
echo "Build logs (OSX)"
cat $(pwd)/unity.log

exit $(($rc1|$rc2))