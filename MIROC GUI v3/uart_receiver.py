#!/usr/bin/env python
# EFIRE UART Receiver for UART Communication with BBB

# UART Dependencies
import mraa
import struct
import time
import websocket

# WS Dependencies
from autobahn.twisted.websocket import WebSocketServerProtocol, WebSocketServerFactory
import sys 
from twisted.python import log
from twisted.internet import reactor

class MyServerProtocol(WebSocketServerProtocol):
	def onMessage(self, payload, isBinary):
		if isBinary:
			print "Binary message recieved: ", len(payload), " bytes."
		else:
			print "Text message recieved: ", payload.decode('utf8')
		self.sendMessage(payload, isBinary)

	def onConnect(self, request):
		print "Client Connecting: ", request.peer

	def onOpen(self):
		print "Websocket Connection open"

	def onClose(self, wasClean, code, reason):
		print "Websocket connection closed: ", reason

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

# Set up WS
log.startLogging(sys.stdout)
factory = WebSocketServerFactory("ws://127.0.0.1:9000")
factory.protocol = MyServerProtocol
reactor.listenTCP(9000, factory)
reactor.run()

# Start a neverending loop waiting for data to arrive.
# When programming to actually do stuff, alter this while loop
while True:

	# Checks if there is any data in the UART RX buffer
	if u.dataAvailable():
		print "Looooop"
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

	# Send Data to WS
