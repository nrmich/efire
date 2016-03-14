\#!/bin/bash/
#Initialize the GPIOs used by the EFIRE Driver.
echo "Initalizing EFIRE/MIROC2 Driver Software"
echo "Initalizing GPIO Pins"
pushd /sys/class/gpio > /dev/null
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
		popd
elif [[ "$INPUT" == n* ]]
	then
		echo "Not running TEST_CLK. TEST_CLK can be run from /efire/clock/mod/"
fi
echo "Initalization Complete"
