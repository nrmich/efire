/* EFIRE Senior Design Project, Boston University
John Marcao
This file is used to allocate memory to the PRU and enable it.
*/

#include <stdio.h>
#include <stdlib.h>
#include <prussdrv.h>
#include <pruss_intc_mapping.h>

#define PRU_0	0
#define PRU_1  1

int main (void)
{
   if(getuid()!=0){
      printf("You must run this program as root. Exiting.\n");
      exit(EXIT_FAILURE);
   }

   // Initialize structure used by prussdrv_pruintc_intc
   // PRUSS_INTC_INITDATA is found in pruss_intc_mapping.h
   tpruss_intc_initdata pruss_intc_initdata = PRUSS_INTC_INITDATA;

   // Allocate and initialize memory
   prussdrv_init ();
   prussdrv_open (PRU_EVTOUT_0);

//   Pointer to the PRU Data memory. Unsure if this is working properly.
//   unsigned int *pruDataMem;
//   prussdrv_map_prumem (&pruDataMem);

   // Map PRU's interrupts
   prussdrv_pruintc_init(&pruss_intc_initdata);

   // Load and execute the PRU program on the PRU
   prussdrv_exec_program (PRU_0, "./pru_0_clk.bin");
   prussdrv_exec_program (PRU_1, "./pru_1_clk.bin");
   printf("EBB fixed-frequency clock PRU program now running\n");

  // int n = prussdrv_pru_wait_event (PRU_EVTOUT_0);  // This assumes the PRU generates an interrupt
                                                    // connected to event out 0 immediately before halting
  // printf("PRU program completed, event number %d.\n", n);

   prussdrv_exit ();
   return EXIT_SUCCESS;
}
