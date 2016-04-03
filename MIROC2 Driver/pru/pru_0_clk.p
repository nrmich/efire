// ********************************************************************************
// EFIRE Senior Design Project, Boston University
// Written by John Marcao
// PRU_0 Instructions. The tasks assigned to PRU_0 are:
// 	- Output CLK and START signals to MIROC2
//	- Keep track of each ADC's state and position
//	- Collect data from MIROC2 ADC
//	- Save data to memory and speak to ARM
// ********************************************************************************
// Register Usage
// R0: TEMP
// R1: DELAY, the number of cycles delayed by the clocks(50% duty cycle, so High for DELAY ins, low for DELAY ins)
// R2: START_CYCLES_0, the number of cycles the START signal is high.
// R3: CLK_CYCLES_0, the number of cyclces needed by the ADC to perform one ADC operation
// R4: START_CYCLES_1, the number of cycles the START signal is high.
// R5: CLK_CYCLES_1, the numsber of cyclces needed by the ADC to perform one ADC operation
// R6: START_CYCLES_2, the number of cycles the START signal is high.
// R7: CLK_CYCLES_2, the number of cyclces needed by the ADC to perform one ADC operation
// ..
// R10: GPIO_1 Clear Data
// R11: GPIO_1 Set Data
// R12: MUX0 Addr
// R13: MUX1 Addr
// R14: MUX2 Addr
// R15: P&H Reset Addr
// ..
// R21: CLK and START control register
// ..
// R23: ADC_0 DATA
// ..
// R25: Talking Channel for PRU0 and PRU1
// ..
// R30: EGPO, output pins of enhanced GPIO pins
// R31: R:EGPI/W:INTC, (when read) input pins of enhanced GPIO pins, (when written) interrupt channels to ARM processor
// *********************************************************************************

.origin 0                        	// start of program in PRU memory
.entrypoint START                	// program entry point (for a debugger)

#define DELAY 46  					// choose the delay value to suit the frequency required
#define START_CYCLES 1
#define CLK_CYCLES 19
#define PnHR_CYCLES 8 				// CAREFUL: THIS VALUE MUST BE EQUAL TO CLK_CYCLES MINUS 11 for proper MIROC2 operation
#define ADC_CYCLES 8

#define XID_SCRATCH_1 10			// Scratchpads for sharing info across PRUs
#define XID_SCRATCH_2 11
#define XID_SCRATCH_3 12
#define XID_SCRATCH_D 14			// Direct connection scratchpad (manually modify other PRU registers - does not work for r30/r31)

#define GPIO_0 0x44e07000			// Addreses for each GPIO Bank
#define GPIO_1 0x4804c000
#define GPIO_2 0x481ac000
#define GPIO_3 0x481ae000

#define GPIO_CLEARDATAOUT 0x190 	// For clearing GPIO registers to 0
#define GPIO_SETDATAOUT 0x194 		// For setting GPIO registers to 1
#define GPIO_DATAOUT 0x138			// For reading the GPIO register

#define MUX0 1<<16 					// MUX Enable 0
#define MUX1 1<<18 					// MUX Enable 1
#define MUX2 1<<19 					// MUC Enable 2
#define PnHR 1<<28 					// Peak & Hold Reset

#define PRU0_R31_VEC_VALID 32  		// allows notification of program completion
#define PRU_EVTOUT_0    3       	// the event number that is sent back

#define DATAMEMORY_ADDR 0x00000000

START:
	// Enable OCP Master Port. This is needed to allow the PRU direct memory access to GPIOs
	LBCO r0, C4, 4, 4				// Load SYSCFG reg into r0 (using c4 const addr)
	CLR  r0, r0, 4					// Clear bit 4 (STANDBY_INIT)
	SBCO r0, C4, 4, 4				// Store the modified r0 back at the load addr

	// Load the Constants needed for operation
	MOV r1, DELAY

	// Load Memory Address
	MOV r9, 0x00000000 // Store base address in r9

	// Load GPIO Address and Offset
	MOV r10, GPIO_1 | GPIO_CLEARDATAOUT
	MOV r11, GPIO_1 | GPIO_SETDATAOUT
	MOV r12, MUX0
	MOV r13, MUX1
	MOV r14, MUX2
	MOV r15, PnHR

	// Clear Talking Register -- Just a precaution (Are registers initalized to 0?)
	ZERO &r25, 4

MAINLOOP:
	XIN XID_SCRATCH_1, r25.w0, 2	// Read Trigger Status from Scratchpad
	XOUT XID_SCRATCH_1, r25.w2, 2	// Write Clock Status to Scratchpad
	QBEQ MAINLOOP, r25, 0			// Loop back if no triggers detected and no clocks active

	// Activate the leftmost free CLK and it's associated Start signal
	CLK_0_ASSIGN:
		QBBS STARTCLOCK, r25.t16 	// If CLK0 is busy, go to CLK1
		LMBD r24.b0, r25.w0, 1		// Find Address and store it
		OR r21, r21, 0b00001001		// Set's CLK0 and START0 control bits high
		SET r25.t16					// Clock is busy

		// Set MUX Enable
		SBBO r12, r11, 0, 4			// Set MUX0 High
		SBBO r13, r10, 0, 4 		// Set MUX1 Low
		SBBO r14, r10, 0, 4 		// Set MUX2 Low
		SBBO r15, r10, 0, 4 		// Set P&H Reset Low

		// Store Counters
		MOV r2, START_CYCLES		// Start Cycles
		MOV r3, CLK_CYCLES			// ADC Cycles

		QBA STARTCLOCK				// Start the Clock -- All prep is complete

// 	Suggestion from Prof. Kia: Test the clocks by keeping them exlusively high. IE: Only 1 clock goes high at a time.
//	This is more of a sanity test. Otherwise, try to make sure the QBBS statements are good. Can also use

// Currently, we are expecting the need for only 1 Clock. After testing, if it is found that 1 clock is insuffecient,
// 		efforts will be made to add the other 2 clocks. This, however, comes at the cost of much more complexity. For
//		the sake of time, we leave this alone for now.

//	CLK_1_ASSIGN:
//		QBBS STARTCLOCK, r25.t17 	// If CLK1 is busy, go to CLK2
//		OR r21, r21, 0b00100010 	// Set's CLK1 and START1 control bits high
//		SET r25.t17  // Clock is busy
//		MOV r4, START_CYCLES		// Clock 1 counter
//		MOV r5, CLK_CYCLES			// Clock 1 counter
//		QBA STARTCLOCK
//	CLK_2_ASSIGN:
//		QBBS STARTCLOCK, r25.t18	// If All clocks are busy, do not use new clock
//		OR r21, r21, 0b10000100 	// Set's CLK2 and START2 control bits high
//		SET r25.t18 // Clock is busy
//		MOV r6, START_CYCLES		// Clock 2 counter
//		MOV r7, CLK_CYCLES			// Clock 2 counter
//		QBA STARTCLOCK

STARTCLOCK:
	MOV r0, r1						// load the delay r1 into temp r0
	OR r30, r30, r21 				// Set CLKs + Starts high

DELAYON:
	SUB	r0, r0, 1					// decrement the counter by 1 and loop (next line)
	QBNE	DELAYON, r0, 0			// loop until the delay has expired (equals 0)
	MOV	r0, r1						// re-load the delay r1 into temporary r0
	AND r30, r30, 0b11111000  		// Clear the CLK bits

DELAYOFF:
	SUB	r0, r0, 1					// decrement the counter by 1 and loop (next line)
	QBNE	DELAYOFF, r0, 0			// loop until the delay has expired (equals 0)
	MOV	r0, r1						// re-load the delay r1 into temporary r0

	SUB r2, r2, 1					// Decrement CLK0 ADC Counter
	SUB r3, r3, 1					// Dec. CLK0 START Counter
//	SUB r4, r4, 1					// Dec. CLK1 ADC Counter
//	SUB r5, r5, 1					// Dec. CLK1 START Counter
//	SUB r6, r6, 1					// Dec. CLK2 ADC Counter
//	SUB r7, r7, 1					// Dec. CLK2 START Counter

	// Collect Data if ADC_CYCLES > r3
	QBGT DATACOLLECT_0, r3, ADC_CYCLES
//	QBGT DATACOLLECT_1, r5, ADC_CYCLES
//	QBGT DaTACOLLECT_2, r7, ADC_CYCLES

	// Check to see if Peak and Hold should be called
	QBEQ PnHR_CLK0, r3, PnHR_CYCLES
//	QBEQ PnHR_CLK1, r5, PnHR_CYCLES
//	QBEq PnHR_CLK2, r7, PnHR_CYCLES

DATACOLLECT_RET:
	// If any counters are 0, disable them.
	// Note the current setup will run into issues if two counters expire at the same time
	// However, with the timings the MIROC2 uses, this will never happen
	QBEQ DISABLESTART_0, r2, 0
	QBEQ DISABLECLOCK_0, r3, 0
//	QBEQ DISABLESTART_1, r4, 0
//	QBEQ DISABLECLOCK_1, r5, 0
//	QBEQ DISABLESTART_2, r6, 0
//	QBEQ DISABLECLOCK_2, r7, 0

	QBA	MAINLOOP					// Start again, so the program will not exit -- It is stopped by the host program
END:
	HALT							// Halt the pru program -- never reached

// CLK_0 CONTROL SIGNALS
DISABLESTART_0:
	CLR r21.t3						// Clear control
	CLR r30.t3						// Clear output
	QBA STARTCLOCK

DISABLECLOCK_0:
	// Pass Data to Host
	SBBO r23.b0, r9, 0, 1 			// Store data from ADC_0 to &r9
	SBBO r24.b0, r9, 1, 1			// Store data for Address to &r9+4b
	MOV R31.b0, PRU0_R31_VEC_VALID | PRU_EVTOUT_0 // Send interrupt to host

	// Clear Control Signals
	CLR r21.t0						// Clear control signal
	CLR r30.t0 						// Clear clock
	CLR r25.t16 					// Set CLK_0 as free

	// Clear Data Cache
	MOV r23, 0x00000000
	QBA MAINLOOP

PnHR_CLK0:
	SBBO r12, r10, 0, 4 			// Set MUX0 Low
	SBBO r15, r11, 0, 4 			// Set P&H Reset High
	QBA MAINLOOP

DATACOLLECT_0:
	LSL r23, r23, 1 				// Shift data collection by 1
	QBBS DATACOLLECT_RET, r31.t14 	// Go back to mainloop if input is high (inverted serial from MIROC2)
	OR r23, r23, 0x00000001 		// Set LSB to 1 if input is low (inverted serial from MIROC2)
	QBA DATACOLLECT_RET

// CLK_1 CONTROL SIGNALS
DISABLESTART_1:
	CLR r30.t5						// Clear output
	CLR r21.t5						// Clear control
	QBA STARTCLOCK

DISABLECLOCK_1:
	CLR r21.t1						// Clear control (clock will be cleared in next loop)
	CLR r25.t17 					// Set CLK_1 as free
	QBA MAINLOOP

// CLK_2 CONTROL SIGNALS
DISABLESTART_2:
	CLR r30.t7						// Clear output
	CLR r21.t7						// Clear control
	QBA STARTCLOCK

DISABLECLOCK_2:
	CLR r21.t2						// Clear control (clock will be cleared in next loop)
	CLR r25.t18 					// Set CLK_2 as free
	QBA MAINLOOP



// Memory Example -- For Reference Only
//	SBBO r23, r9, 0, 4 				// Store 4 bytes  of data form r23 at address in r9 offset by 0
//	Add r9, r9, 4 					// increment address by 4 bytes
//	MOV	R31.b0, PRU0_R31_VEC_VALID | PRU_EVTOUT_0 // Send interrupt to host