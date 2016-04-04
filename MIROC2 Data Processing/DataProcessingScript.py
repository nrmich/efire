#!/usr/bin/python
'''
Boston University 2016
Senior Design Project
Team E-FIRE:
	Casey Pelkowsky
	Junhao Hua
	Nathaniel Michener
	John Marcao
	Derek Kenyon

The following code was written by Derek Kenyon.
'''

#DataProcessingScript to encompass functions for DataProcessingProgram
import time
import math
from random import randint

def CSV_file_initializer(targetFile):
	x = open(targetFile, 'w+')
	x.write("<strike number>, <diode struck>, <electron energy (keV)>, <strike time (ms)>")
	x.write("\n")
	x.close()

def write_to_CSV(targetFile, dataList):
	x = open(targetFile, 'a')
	x.write(dataList + '\n')
	x.close()

''' Receiving two bytes of data from BBB through a uart_mnm(byte1, byte2) function already made '''
dataList = []
energyList = []
s = '\n'
strikeNumber = 0
sum = 0
CSV_file_initializer('EFIRE.txt')
# !!!! Keep in mind, the loop is for testing; there will be a loop in the main program
for i in range (1,1001):
        '''
        #1 byte for energy and 1 byte for channel (NOT diode, must translate according to Analog Board)
        currentEnergy = b"00000011" #decimal value 3 for energy
        currentChannel = b"00000001" #decimale value 1 for channel
        
        #This will be in the main program within the loop, storing the last energy and channel value
        #lastEnergy = 
        #lastChannel =
        

        energy = int(currentEnergy, 2)
        print(energy)
        channel = int(currentChannel, 2)
        print(channel)
        '''
        electronEnergy = randint(0,1050)
        energyList.append(electronEnergy)
        channel = 4
        
        #Translate channel to diodeStruck
        def channel_to_diode(channel):
                return {
                        2 : 1,
                        3 : 11,
                        4 : 10,
                        6 : 12,
                        7 : 9,
                        8 : 2,
                        10 : 8,
                        11 : 3,
                        12 : 6,
                        14 : 4,
                        15 : 5,
                        16 : 6,
                }.get(channel, 'ERROR: Channel number receieved was out of bounds!')
        diodeStruck = channel_to_diode(channel)
        
        #Iterate strikeNumber number
        strikeNumber = strikeNumber + 1
        #Sum for standard deviation calculation later
        sum = sum + electronEnergy
        #Store strikeTime in a variable
        if (strikeNumber == 1):
                start = time.time()
                strikeTime = 0
        else:
                strikeTime = (time.time() - start) * 1000 #change s to ms

        #Append the data to the variable dataList and every 100 or so strikes, update the CSV file
        #With saving every 100 strikes, the time to complete 1000 strikes is ~25ms
        dataList.append(str(strikeNumber) + "," + str(diodeStruck) + "," + str(electronEnergy) + "," + str(strikeTime))
        if ((strikeNumber % 100) == 0):
                dataListUpdated = s.join(dataList)
                write_to_CSV('EFIRE.txt', dataListUpdated)
                dataList = []

''' Calculate the standard deviation, hypothetical electric field and FWHM '''
#Standard Deviation
print (strikeNumber)
mean = sum / strikeNumber
print (mean)
intSum = 0
for x in range (0, (strikeNumber)):
        intSum = intSum + (energyList[x] - mean)
print(intSum)
stdDev = math.sqrt(abs((1/strikeNumber)*(intSum)))
print (stdDev)

#Hypothetical Electric Field
eField = 123
#Full-Width Half-Max
fwhm = 3
#Store in CSV
calcList = []
calcList.append('Standard Deviation: ' + str(stdDev))
calcList.append('Electric Field: ' + str(eField))
calcList.append('Full-Width Half-Max: ' + str(fwhm))
calcListUpdated = s.join(calcList)
write_to_CSV('EFIREcalc.txt', 'Calculations based on the previous test in EFIRE.txt\n')
write_to_CSV('EFIREcalc.txt', calcListUpdated)
