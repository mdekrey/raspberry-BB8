scp -r -C BB8/bin/Release/net5.0/linux-arm/publish/* pi@raspberrypi:~/bb8/
ssh pi@raspberrypi "chmod 755 ~/bb8/BB8"
