/* EFIRE Senior Design Project, Boston University
John Marcao
This file is used to allocate memory to the PRU and enable it.
*/

#include <stdio.h>
#include <stdlib.h>
#include <prussdrv.h>
#include <pruss_intc_mapping.h>
#include <fcntl.h>
#include <unistd.h>
#include <stdint.h>
#include <termios.h>
#include <string.h>
#include <math.h>

#define PRU_0 0
#define PRU_1 1

int main (int argc, char *argv[])
{
   if(getuid()!=0){
	  printf("You must run this program as root. Exiting.\n");
	  exit(EXIT_FAILURE);
   }

   // UART Init
   int file, count;
   if ((file = open("/dev/ttyO2", O_RDWR | O_NOCTTY | O_NDELAY))<0){    // Modified ttyO4 -> ttyO2 for efire purposes
	  perror("UART: Failed to open the file.\n");
	  return -1;
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

  // Establish pointers to PRU Memory
  unsigned char *pruDataMem_0;
  prussdrv_map_prumem (0, (void*) &pruDataMem_0);
  if (pruDataMem_0 == NULL){
	printf("PRU: Data Memory not mapped!\nExiting.");
	exit(EXIT_FAILURE);
  }
  unsigned char *pruMem[2];
  pruMem[0] = pruDataMem_0;
  pruMem[1] = (pruDataMem_0 + 1);

   // Map PRU's interrupts
   prussdrv_pruintc_init(&pruss_intc_initdata);

   // Load and execute the PRU program on the PRU
   prussdrv_exec_program (PRU_0, "./pru_0_clk.bin");
   prussdrv_exec_program (PRU_1, "./pru_1_clk.bin");
   printf("EBB fixed-frequency clock PRU program now running\n");

	// Data Collection Loop
   int i = 0;
   int error_count = 0;
   while(i < 100){
	printf("Waiting for intc...\n");
	prussdrv_pru_wait_event (PRU_EVTOUT_0);
	prussdrv_pru_clear_event (PRU_EVTOUT_0, PRU0_ARM_INTERRUPT);
	printf("Value: %d\n", *pruMem[0]);
	printf("Channel: %d\n", *pruMem[1] + 1);

	unsigned char transmit[2];
	transmit[0] = *pruMem[0];
	transmit[1] = *pruMem[1] + 1; //Add 1 for readability

	if ((count = write(file, &transmit, 2))<0){        //send the string
		perror("Failed to write to the output\n");
	}
	usleep(1000); // Sleep 1ms for MBM to read data (otherwise overflows UART buffer)
	//i++;
 }

	prussdrv_pru_disable(PRU_0);
	prussdrv_pru_disable(PRU_1);
	close(file);
	prussdrv_exit ();
	return EXIT_SUCCESS;
}
