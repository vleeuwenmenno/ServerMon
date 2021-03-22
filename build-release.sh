#!/bin/bash
echo "Cleaning up release folder ..."
rm -rf bin/Release/

if [ ! -d buid/ ]; then
  mkdir build
fi

find build/ -type f -not -name '*.zip' -delete

echo "Running dotnet clean ..."
dotnet clean

echo "Restoring dotnet packages/references ..."
dotnet restore

echo "Compiling project as single file with Release profile ..."
dotnet publish -c Release --runtime ubuntu.20.04-x64 -p:PublishSingleFile=false --self-contained true

echo "Fetching binaries to build/"
mv ./bin/Release/net5.0/ubuntu.20.04-x64/publish/* ./build/

echo "Removing docs as we don't need them for a server ..."
rm -rf build/docs/
rm -rf build/LoginAPI/

echo "Removing debug symbols ..."
cd build/
rm *.pdb

echo "Compressing binaries ..."
version=$(./ServerMon -v | cut -d ' ' -f 3-)

zip -r ServerMon-v1.2.3.zip * -x *.zip
cd ..

echo "Cleaning up ..."
rm -rf bin/Release/
rm -rf build/config/
find build/ -type f -not -name '*.zip' -delete


echo "Opening folder and zip file with default file manager, we're all done here!"
xdg-open build/