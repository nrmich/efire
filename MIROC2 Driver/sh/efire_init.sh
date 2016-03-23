#!/bin/bash/
#Initialize the GPIOs used by the EFIRE Driver
echo "Initalizing EFIRE/MIROC2 Driver Software"
echo "Initalizing GPIO Pins"
pushd /sys/class/gpio > /dev/null

echo "ADDR Pins"
sh -c "echo 22 > export"
echo out > ./gpio22/direction
echo 0 > ./gpio22/value
GPIO22_dir="$(cat ./gpio22/direction)"
GPIO22_val="$(cat ./gpio22/value)"
echo "GPIO22 direction is ${GPIO22_dir} and value is ${GPIO22_val}"
sh -c "echo 10 > export"
echo out > ./gpio10/direction
echo 0 > ./gpio10/value
GPIO10_dir="$(cat ./gpio10/direction)"
GPIO10_val="$(cat ./gpio10/value)"
echo "GPIO10 direction is ${GPIO10_dir} and value is ${GPIO10_val}"
sh -c "echo 11 > export"
echo out > ./gpio11/direction
echo 0 > ./gpio11/value
GPIO11_dir="$(cat ./gpio11/direction)"
GPIO11_val="$(cat ./gpio11/value)"
echo "GPIO11 direction is ${GPIO11_dir} and value is ${GPIO11_val}"
sh -c "echo 9 > export"
echo out > ./gpio9/direction
echo 0 > ./gpio9/value
GPIO9_dir="$(cat ./gpio9/direction)"
GPIO9_val="$(cat ./gpio9/value)"
echo "GPIO9 direction is ${GPIO9_dir} and value is ${GPIO9_val}"
sh -c "echo 8 > export"
echo out > ./gpio8/direction
echo 0 > ./gpio8/value
GPIO8_dir="$(cat ./gpio8/direction)"
GPIO8_val="$(cat ./gpio8/value)"
echo "GPIO8 direction is ${GPIO8_dir} and value is ${GPIO8_val}"

echo "P&H Reset"
sh -c "echo 60 > export"
echo out > ./gpio60/direction
echo 0 > ./gpio60/value
GPIO60_dir="$(cat ./gpio60/direction)"
GPIO60_val="$(cat ./gpio60/value)"
echo "GPIO60 direction is ${GPIO60_dir} and value is ${GPIO60_val}"

echo "MUX Enable Pins"
sh -c "echo 48 > export"
echo out > ./gpio48/direction
echo 0 > ./gpio48/value
GPIO48_dir="$(cat ./gpio48/direction)"
GPIO48_val="$(cat ./gpio48/value)"
echo "GPIO48 direction is ${GPIO48_dir} and value is ${GPIO48_val}"
sh -c "echo 50 > export"
echo out > ./gpio50/direction
echo 0 > ./gpio50/value
GPIO50_dir="$(cat ./gpio50/direction)"
GPIO50_val="$(cat ./gpio50/value)"
echo "GPIO50 direction is ${GPIO50_dir} and value is ${GPIO50_val}"
sh -c "echo 51 > export"
echo out > ./gpio51/direction
echo 0 > ./gpio51/value
GPIO51_dir="$(cat ./gpio51/direction)"
GPIO51_val="$(cat ./gpio51/value)"
echo "GPIO51 direction is ${GPIO51_dir} and value is ${GPIO51_val}"
popd > /dev/null
echo "GPIO Pins Initalized"
echo "Initalizing DTO"
sh -c "echo EFIRE-Overlay > $SLOTS"
cat $SLOTS | grep EFIRE-Overlay
echo "DTO Initalized"
read -p "Would you like to start the TEST_CLK? [y/n]	" INPUT
if [[ "$INPUT" == y* ]]
	then
		echo "Starting TEST_CLK"
		pushd /root/efire/clock/mod > /dev/null
		./fixedclock
		read -p "Press enter to end TEST_CLK"
		./halt_cmd
		popd > /dev/null
elif [[ "$INPUT" == n* ]]
	then
		echo "Not running TEST_CLK. TEST_CLK can be run from /efire/clock/mod/"
fi
echo "Initalization Complete"
