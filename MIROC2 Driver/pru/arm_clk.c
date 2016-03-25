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
#include <sys/ioctl.h>
#include <stdint.h>
#include <linux/spi/spidev.h>

#define SPI_PATH "/dev/spidev1.0"

#define PRU_0	0
#define PRU_1 1

int transfer(int fd, unsigned char send[], unsigned char recieve[], int length){
  // Establish Trasfer characteristics
  struct spi_ioc_transfer transfer;
  transfer.tx_buf = (unsigned long) send;
  transfer.rx_buf = (unsigned long) recieve;
  transfer.len = length;
  transfer.speed_hz = 1000000;
  transfer.bits_per_word = 8;
  transfer.delay_usecs = 0;

  // Send the SPI Message
  int status = ioctl(fd, SPI_IOC_MESSAGE(1), &transfer);
  if (status < 0){
    perror("SPI: SPI_IOC_MESSAGE failed");
    return -1;
  }
  return status;
}

int main (void)
{
   if(getuid()!=0){
      printf("You must run this program as root. Exiting.\n");
      exit(EXIT_FAILURE);
   }

   // Initalize SPI 
   int fd;                        //file handle and loop counter
   uint8_t bits = 8, mode = 3;             //8-bits per word, SPI mode 3
   uint32_t speed = 1000000; 
   // The following calls set up the SPI bus properties
   if ((fd = open(SPI_PATH, O_RDWR))<0){
      perror("SPI Error: Can't open device.");
      return -1;
   }
   if (ioctl(fd, SPI_IOC_WR_MODE, &mode)==-1){
      perror("SPI: Can't set SPI mode.");
      return -1;
   }
   if (ioctl(fd, SPI_IOC_RD_MODE, &mode)==-1){
      perror("SPI: Can't get SPI mode.");
      return -1;
   }
   if (ioctl(fd, SPI_IOC_WR_BITS_PER_WORD, &bits)==-1){
      perror("SPI: Can't set bits per word.");
      return -1;
   }
   if (ioctl(fd, SPI_IOC_RD_BITS_PER_WORD, &bits)==-1){
      perror("SPI: Can't get bits per word.");
      return -1;
   }
   if (ioctl(fd, SPI_IOC_WR_MAX_SPEED_HZ, &speed)==-1){
      perror("SPI: Can't set max speed HZ");
      return -1;
   }
   if (ioctl(fd, SPI_IOC_RD_MAX_SPEED_HZ, &speed)==-1){
      perror("SPI: Can't get max speed HZ.");
      return -1;
   }

   // Initialize PRU structures
   tpruss_intc_initdata pruss_intc_initdata = PRUSS_INTC_INITDATA;
   prussdrv_init ();
   prussdrv_open (PRU_EVTOUT_0);

  // Establish pointers to PRU Memory
  unsigned char *pruDataMem_0;
  prussdrv_map_prumem (0, &pruDataMem_0);
  if (pruDataMem_0 == NULL){
    printf("Data Memory not mapped!\nExiting.");
    exit(EXIT_FAILURE);
  }
  unsigned char *pruAddrMem_0 = pruDataMem_0 + 4; // shifted by 4 bytes
  unsigned char pruMem[] = {0,0,0};
  unsigned char recieve[] = {0,0,0};

   // Map PRU's interrupts
   prussdrv_pruintc_init(&pruss_intc_initdata);

   // Load and execute the PRU program on the PRU
   prussdrv_exec_program (PRU_0, "./pru_0_clk.bin");
   prussdrv_exec_program (PRU_1, "./pru_1_clk.bin");
   printf("EBB fixed-frequency clock PRU program now running\n");

   printf("SPI Mode is: %d\n", mode);
   printf("SPI Bits is: %d\n", bits);
   printf("SPI Speed is: %d\n", speed);
   printf("PRU_0 Data Mem starts at: %p \n", pruDataMem_0);

   int i;
   for(i = 0;; i++){
    // Data Collection Loop
    printf("Waiting for intc...\n");
    prussdrv_pru_wait_event (PRU_EVTOUT_0);
    pruMem[0] = *pruDataMem_0;
    pruMem[1] = *(pruAddrMem_0 + 1);
    pruMem[2] = *pruAddrMem_0;
    prussdrv_pru_clear_event (PRU_EVTOUT_0, PRU0_ARM_INTERRUPT);
    printf("Value: %d\n", pruMem[0]);
    printf("Channel: 0x%X%.2X\n", pruMem[1], pruMem[2]);
    transfer(fd, pruMem, recieve, 3);
   }

   prussdrv_pru_disable(PRU_0);
   prussdrv_pru_disable(PRU_1);
   prussdrv_exit ();
   return EXIT_SUCCESS;
}
