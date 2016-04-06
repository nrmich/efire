/* EFIRE Senior Design Project, Boston University
John Marcao
This file is used to allocate memory to the PRU and enable it.
*/
#include <stdio.h>
#include <stdlib.h>
#include <prussdrv.h>
#include <pruss_intc_mapping.h>
#include <fcntl.h>
#include <termios.h>
#include <signal.h>

#define PRU_0 0
#define PRU_1 1

/* Global Variables */
int file;

/* Func Prototypes */
void efireExit(void);
void INThandler(int sig);

int main (int argc, char *argv[]){
	// Confirm root status (UART/PRU will not run otherwise)
	if(getuid()!=0){
		printf("You must run this program as root. Exiting.\n");
		exit(EXIT_FAILURE);
	}

	// Set Collision Limit
	// By default, it is unlimited. User may supply an argument to limit it.
	int collisionLimit;
	if(argc > 1){
		if(argc == 2){
			collisionLimit = atoi(argv[1]);	// Assumes user knows what they are doing and will only put in an int
		} else {
			printf("Invalid argument: Collisions will not be limited.\n");
			collisionLimit = -1;
		}
	} else {
		collisionLimit = -1;
	}

	// For catching SIGINT and avoiding UART errors
	signal(SIGINT, INThandler);

	// UART Initialization
	int count;
	if ((file = open("/dev/ttyO2", O_RDWR | O_NOCTTY | O_NDELAY))<0){
		perror("UART: Failed to open the file.\n");
		exit(EXIT_FAILURE);
	}
	struct termios options;
	tcgetattr(file, &options);
	options.c_cflag = B19200 | CS8 | CREAD | CLOCAL;
	options.c_iflag = IGNPAR | ICRNL;
	options.c_lflag &= ~(ICANON|ECHO);
	tcflush(file, TCIFLUSH);
	tcsetattr(file, TCSANOW, &options);

	// Initialize PRU structures
	tpruss_intc_initdata pruss_intc_initdata = PRUSS_INTC_INITDATA;
	prussdrv_init ();
	prussdrv_open (PRU_EVTOUT_0);

	// Declare and assign pointers to PRU Memory
	unsigned char *pruMem[2];
	prussdrv_map_prumem (0, (void*) &pruMem[0]);
	if (pruMem[0] == NULL){
		printf("PRU: Data Memory not mapped!\nExiting.");
		exit(EXIT_FAILURE);
	}
	pruMem[1] = pruMem[0] + 1;

	// Map PRU's interrupts
	prussdrv_pruintc_init(&pruss_intc_initdata);

	// Load and execute the PRU program on the PRU
	prussdrv_exec_program (PRU_0, "./pru_0_clk.bin");
	prussdrv_exec_program (PRU_1, "./pru_1_clk.bin");
	printf("EBB fixed-frequency clock PRU program now running\n");

	// Data Collection Loop
	int i = 0;
	while(i != collisionLimit){
		// Increment collision counter
		i++;

		// Wait for interrupt from PRU
		printf("Waiting for intc...\n");
		prussdrv_pru_wait_event (PRU_EVTOUT_0);
		prussdrv_pru_clear_event (PRU_EVTOUT_0, PRU0_ARM_INTERRUPT);

		// Print data for user (Future feature: Disable printing to stdout unless a -DEBUG argument is passed)
		printf("Collision %d\n", i);
		printf("\tValue:   %d\n", *pruMem[0]);
		printf("\tChannel: %d\n", *pruMem[1] + 1);

		// Store data for transmission
		unsigned char transmit[2];
		transmit[0] = *pruMem[0];
		transmit[1] = *pruMem[1] + 1; //Add 1 to channel for readability

		// Transmit data over UART
		if ((count = write(file, &transmit, 2))<0){  //send the string
			perror("Failed to write to the output\n");
		}

		usleep(1000); 	// Sleep 1ms for MBM to read data (otherwise overflows UART buffer)
						// This value can be lowered to increase speed, at the cost of security
	}

	// Clear PRU memory pointers
	*pruMem[0] = 0;
	*pruMem[1] = 0;
	// Disable PRUs and close UART
	efireExit();

	/* Exit -- This should not be reached
	 efireExit() will ensure UART and PRU are disabled. */
	return EXIT_FAILURE;
}

void efireExit(void){
	int ctrl = 0;
	if (prussdrv_pru_disable(PRU_0) != 0){
		ctrl = 1;
		printf("ERROR: PRU_0 did not disable.");
	}
	if (prussdrv_pru_disable(PRU_1) != 0){
		ctrl = 1;
		printf("ERROR: PRU_1 did not disable.");
	}
	prussdrv_exit (); // Always returns 0
	
	tcflush(file, TCIOFLUSH);
	if (close(file) != 0){
		ctrl = 1;
		printf("ERROR: UART did not close.");
	}

	if(ctrl == 1){
		printf("ERROR EXITING: Something went really wrong.");
		exit(EXIT_FAILURE);
	}
	
	printf("Exited Succesfully.\n");
	exit(EXIT_SUCCESS);
}

void  INThandler(int sig)
{
     printf("\nSIGINT Detected: Closing PRU and UART Safely\n");
     efireExit();
}