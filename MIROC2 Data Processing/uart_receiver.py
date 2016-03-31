#!/usr/bin/env python
# EFIRE UART Receiver for UART Communication with BBB

import mraa
import struct
import time

# Initialize UART
u=mraa.Uart(0)

# Set UART parameters
# NOTE: Do not alter Baud rate unless also altering BBB baud rate
u.setBaudRate(19200)
u.setMode(8, mraa.UART_PARITY_NONE, 1)
u.setFlowcontrol(False, False)

# Collision counter 
count = 0
start_time = time.time()
elapsed_time = time.time()

# Start a neverending loop waiting for data to arrive.
# When programming to actually do stuff, alter this while loop
while True:
	# Checks if there is any data in the UART RX buffer
	if u.dataAvailable():
		# Read two bytes from the RX Buffer
		# BBB has controls to prevent overloading the bufgfer and causing values to missalign
		adcv = struct.unpack("B", u.read(1))[0]
		addr = struct.unpack("B", u.read(1))[0]
		
		# Print values (for debugging)
		print(adcv, addr)

		# Keep track of number of collisions
		count += 1
		resolution = time.time() - elapsed_time - start_time
		elapsed_time = time.time() - start_time
		print 'Count: ', count
		print 'Elapsed time: ', elapsed_time
		print 'Resolution: ', resolution
		print 'Collisions/sec: ', count / elapsed_time
