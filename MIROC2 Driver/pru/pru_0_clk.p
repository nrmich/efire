// ********************************************************************************
// EFIRE Senior Design Project, Boston University
// John Marcao
// PRU_0 Instructions. The tasks assigned to PRU_0 are:
// 	- Output CLK and START signals to MIROC2
//	- Keep track of each ADC's state and position
// This file has been adapted from Derek Molloy's book "Exploring BeagleBone Black"
// *ONLY FOR TESTING PURPOSES*
// ********************************************************************************
// Register Usage
// R0: TEMP
// R1: DELAY, the number of cycles delayed by the clocks(50% duty cycle, so High for DELAY ins, low for DELAY ins)
// R2: START_CYCLES, the number of cycles the START signal is high.
// R3: ADC_CYCLES, the number of cyclces needed by the ADC to perform one ADC operation
// R4: 
// ..
// R20: TRIGGER_DET, status of the trigger detector from PRU_1. Is updated each time MAINLOOP is called. (Note: This will need to be repositioned
//		with the addition of 3 clocks, each meant to run off of diff trigger signals.)
// R21: CLK and START control register
// R22: CLK state
// ..
// R30: EGPO, output pins of enhanced GPIO pins
// R31: R:EGPI/W:INTC, (when read) input pins of enhanced GPIO pins, (when written) interrupt channels to ARM processor
// *********************************************************************************

.origin 0                        // start of program in PRU memory
.entrypoint START                // program entry point (for a debugger)

#define	DELAY   47  		 // choose the delay value to suit the frequency required							 // 1 gives a 20MHz clock signal, increase from there
#define START_CYCLES 2
#define ADC_CYCLES 19

#define XID_SCRATCH_1 10	// Scratchpads for sharing info across PRUs
#define XID_SCRATCH_2 11
#define XID_SCRATCH_3 12
#define XID_SCRATCH_D 14	// Direct connection scratchpad (manually modify other PRU registers - does not work for r30/r31)

START:
	// Enable OCP Master Port. This is needed to allow the PRU direct memory access to GPIOs
	LBCO	r0, C4, 4, 4	// Load SYSCFG reg into r0 (using c4 const addr)
	CLR 	r0, r0, 4		// Clear bit 4 (STANDBY_INIT)
	SBCO	r0, C4, 4, 4	// Store the modified r0 back at the load addr

	// Load the Constants needed for operation
	MOV	r1, DELAY
	MOV	r2, START_CYCLES
	MOV r3, ADC_CYCLES	
	ZERO &r20, 4 // Clear 4 bytes at &r20
	ZERO &r22, 4

MAINLOOP:
	XIN XID_SCRATCH_1, r25, 4	// Read r20 from Scratch1
	QBEQ MAINLOOP, r25.w0, 0	// 0 = Clock on every trigger instance
								// r20 = if the triggers have changed since the last cycle
	MOV r20, r25

	// Activate the leftmost free CLK and it's associated Start signal
	CLK_0_ASSIGN:
		QBBS STARTCLOCK, r22.t0 // If CLK0 is busy, go to CLK1
		OR r21, r21, 0b00001001	// Set's CLK0 and START0 control bits high
		SET r22.t0 // Clock is busy
		MOV r2, START_CYCLES	// Clock 0 counter
		MOV r3, ADC_CYCLES		// Clock 0 counter
		QBA STARTCLOCK
//	CLK_1_ASSIGN:
//		QBBS CLK_2_ASSIGN, r22.t1 // If CLK1 is busy, go to CLK2
//		OR r21, r21, 0b00100010 // Set's CLK1 and START1 control bits high
//		SET r22.t1  // Clock is busy
//		MOV r4, START_CYCLES	// Clock 1 counter
//		MOV r5, ADC_CYCLES		// Clock 1 counter
//		QBA STARTCLOCK
//	CLK_2_ASSIGN:
//		QBBS STARTCLOCK, r22.t2 // If All clocks are busy, do not use new clock
//		OR r21, r21, 0b10000100 // Set's CLK2 and START2 control bits high
//		SET r22.t2 // Clock is busy
//		MOV r6, START_CYCLES	// Clock 2 counter
//		MOV r7, ADC_CYCLES		// Clock 2 counter
//		QBA STARTCLOCK
	
STARTCLOCK:
	MOV	r0, r1		// load the delay r1 into temp r0
	OR r30, r30, r21 // Set CLKs + Starts high

DELAYON:
	SUB	r0, r0, 1	// decrement the counter by 1 and loop (next line)
	QBNE	DELAYON, r0, 0	// loop until the delay has expired (equals 0)
	MOV	r0, r1		// re-load the delay r1 into temporary r0
	AND r30, r30, 0b11111000  // Clear the CLK bits
	
DELAYOFF:
	SUB	r0, r0, 1	// decrement the counter by 1 and loop (next line)
	QBNE	DELAYOFF, r0, 0	// loop until the delay has expired (equals 0)
	MOV	r0, r1		// re-load the delay r1 into temporary r0

	SUB r2, r2, 1	// Decrement the counters
	SUB r3, r3, 1
	SUB r4, r4, 1
	SUB r5, r5, 1
	SUB r6, r6, 1
	SUB r7, r7, 1

	QBEQ DISABLESTART_0, r2, 0
	QBEQ DISABLECLOCK_0, r3, 0
	QBEQ DISABLESTART_1, r4, 0
	QBEQ DISABLECLOCK_1, r5, 0
	QBEQ DISABLESTART_2, r6, 0
	QBEQ DISABLECLOCK_2, r7, 0

	QBA	MAINLOOP	// start again, so the program will not exit

END:
	HALT			// halt the pru program -- never reached


DISABLESTART_0:
	CLR r30.t3	// Clear output
	CLR r21.t3	// Clear control
	QBA STARTCLOCK

DISABLECLOCK_0:
	CLR r21.t0	// Clear control (clock will be cleared in next loop)
	CLR r22.t0 	// Set CLK_0 as free
	QBA MAINLOOP

DISABLESTART_1:
	CLR r30.t5	// Clear output
	CLR r21.t5	// Clear control
	QBA STARTCLOCK

DISABLECLOCK_1:
	CLR r21.t1	// Clear control (clock will be cleared in next loop)
	CLR r22.t1 	// Set CLK_0 as free
	QBA MAINLOOP

DISABLESTART_2:
	CLR r30.t7	// Clear output
	CLR r21.t7	// Clear control
	QBA STARTCLOCK

DISABLECLOCK_2:
	CLR r21.t2	// Clear control (clock will be cleared in next loop)
	CLR r22.t2 	// Set CLK_0 as free
	QBA MAINLOOP
