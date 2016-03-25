using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Devices.Enumeration;
//using System.Windows.Devices.Spi;

namespace Team_E_FIRE_Data_Processing_Program_BBB
{

    // /// <summary>
    // /// Sample app that reads data over either I2C or SPI from an attached ADXL345 accelerometer
    // /// </summary>
    // public sealed partial class MainPage : Page
    // {
    //     /* Important! Change this to either Protocol.I2C or Protocol.SPI based on how your accelerometer is wired   */     

    //     private const byte ACCEL_REG_POWER_CONTROL = 0x2D;  /* Address of the Power Control register                */
    //     private const byte ACCEL_REG_DATA_FORMAT = 0x31;    /* Address of the Data Format register                  */
    //     private const byte ACCEL_REG_X = 0x32;              /* Address of the X Axis data register                  */
    //     private const byte ACCEL_REG_Y = 0x34;              /* Address of the Y Axis data register                  */
    //     private const byte ACCEL_REG_Z = 0x36;              /* Address of the Z Axis data register                  */


    //     private const byte SPI_CHIP_SELECT_LINE = 0;        /* Chip select line to use                              */
    //     private const byte ACCEL_SPI_RW_BIT = 0x80;         /* Bit used in SPI transactions to indicate read/write  */
    //     private const byte ACCEL_SPI_MB_BIT = 0x40;         /* Bit used to indicate multi-byte SPI transactions     */

    //     private SpiDevice SPIAccel;
    //     private Timer periodicTimer;

    //     public MainPage()
    //     {
    //         this.InitializeComponent();

    //         /* Register for the unloaded event so we can clean up upon exit */
    //         Unloaded += MainPage_Unloaded;

    //         /* Initialize the I2C bus, accelerometer, and timer */
    //         InitAccel();
    //     }

    //     private void InitAccel()
    //     {
    //InitSPIAccel();
    //     }

    //     /* Initialization for SPI accelerometer */
    //     private async void InitSPIAccel()
    //     {
    //         try {
    //             var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
    //             settings.ClockFrequency = 5000000;                              /* 5MHz is the rated speed of the ADXL345 accelerometer                     */
    //             settings.Mode = SpiMode.Mode3;                                  /* The accelerometer expects an idle-high clock polarity, we use Mode3    
    //                                                                              * to set the clock polarity and phase to: CPOL = 1, CPHA = 1         
    //                                                                              */

    //             string aqs = SpiDevice.GetDeviceSelector();                     /* Get a selector string that will return all SPI controllers on the system */
    //             var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the SPI bus controller devices with our selector string             */
    //             SPIAccel = await SpiDevice.FromIdAsync(dis[0].Id, settings);    /* Create an SpiDevice with our bus controller and SPI settings             */
    //             if (SPIAccel == null)
    //             {
    //                 Text_Status.Text = string.Format(
    //                     "SPI Controller {0} is currently in use by " +
    //                     "another application. Please ensure that no other applications are using SPI.",
    //                     dis[0].Id);
    //                 return;
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Text_Status.Text = "SPI Initialization failed. Exception: " + ex.Message;
    //             return;
    //         }

    //         /* 
    //          * Initialize the accelerometer:
    //          *
    //          * For this device, we create 2-byte write buffers:
    //          * The first byte is the register address we want to write to.
    //          * The second byte is the contents that we want to write to the register. 
    //          */
    //         byte[] WriteBuf_DataFormat = new byte[] { ACCEL_REG_DATA_FORMAT, 0x01 };        /* 0x01 sets range to +- 4Gs                         */
    //         byte[] WriteBuf_PowerControl = new byte[] { ACCEL_REG_POWER_CONTROL, 0x08 };    /* 0x08 puts the accelerometer into measurement mode */

    //         /* Write the register settings */
    //         try
    //         {  
    //             SPIAccel.Write(WriteBuf_DataFormat);
    //             SPIAccel.Write(WriteBuf_PowerControl);
    //         }
    //         /* If the write fails display the error and stop running */
    //         catch (Exception ex)
    //         {
    //             Text_Status.Text = "Failed to communicate with device: " + ex.Message;
    //             return;
    //         }

    //         /* Now that everything is initialized, create a timer so we read data every 100mS */
    //         periodicTimer = new Timer(this.TimerCallback, null, 0, 100);
    //     }

    //     private void TimerCallback(object state)
    //     {
    //         string xText, yText, zText;
    //         string statusText;

    ///* Read and format accelerometer data */
    //         try
    //         {
    //             Acceleration accel = ReadAccel();
    //             xText = String.Format("X Axis: {0:F3}G", accel.X);
    //             yText = String.Format("Y Axis: {0:F3}G", accel.Y);
    //             zText = String.Format("Z Axis: {0:F3}G", accel.Z);
    //             statusText = "Status: Running";
    //         }
    //         catch (Exception ex)
    //         {
    //             xText = "X Axis: Error";
    //             yText = "Y Axis: Error";
    //             zText = "Z Axis: Error";
    //             statusText = "Failed to read from Accelerometer: " + ex.Message;
    //         }

    //         /* UI updates must be invoked on the UI thread */
    //         var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
    //         {
    //             Text_X_Axis.Text = xText;
    //             Text_Y_Axis.Text = yText;
    //             Text_Z_Axis.Text = zText;
    //             Text_Status.Text = statusText;
    //         });
    //     }

    //     private Acceleration ReadAccel()
    //     {
    //         const int ACCEL_RES = 1024;         /* The ADXL345 has 10 bit resolution giving 1024 unique values                     */
    //         const int ACCEL_DYN_RANGE_G = 8;    /* The ADXL345 had a total dynamic range of 8G, since we're configuring it to +-4G */
    //         const int UNITS_PER_G = ACCEL_RES / ACCEL_DYN_RANGE_G;  /* Ratio of raw int values to G units                          */

    //         byte[] ReadBuf;                 
    //         byte[] RegAddrBuf;

    //         /* 
    //          * Read from the accelerometer 
    //          * We first write the address of the X-Axis register, then read all 3 axes into ReadBuf
    //          */

    //                 ReadBuf = new byte[6 + 1];      /* Read buffer of size 6 bytes (2 bytes * 3 axes) + 1 byte padding */
    //                 RegAddrBuf = new byte[1 + 6];   /* Register address buffer of size 1 byte + 6 bytes padding        */
    //                 /* Register address we want to read from with read and multi-byte bit set                          */
    //                 RegAddrBuf[0] =  ACCEL_REG_X | ACCEL_SPI_RW_BIT | ACCEL_SPI_MB_BIT ;
    //                 SPIAccel.TransferFullDuplex(RegAddrBuf, ReadBuf);
    //                 Array.Copy(ReadBuf, 1, ReadBuf, 0, 6);  /* Discard first dummy byte from read                      */
    //                 break;

    //         /* Check the endianness of the system and flip the bytes if necessary */
    //         if (!BitConverter.IsLittleEndian)
    //         {
    //             Array.Reverse(ReadBuf, 0, 2);
    //             Array.Reverse(ReadBuf, 2, 2);
    //             Array.Reverse(ReadBuf, 4, 2);
    //         }

    //         /* In order to get the raw 16-bit data values, we need to concatenate two 8-bit bytes for each axis */
    //         short AccelerationRawX = BitConverter.ToInt16(ReadBuf, 0);
    //         short AccelerationRawY = BitConverter.ToInt16(ReadBuf, 2);
    //         short AccelerationRawZ = BitConverter.ToInt16(ReadBuf, 4);

    //         /* Convert raw values to G's */
    //         Acceleration accel;
    //         accel.X = (double)AccelerationRawX / UNITS_PER_G;
    //         accel.Y = (double)AccelerationRawY / UNITS_PER_G;
    //         accel.Z = (double)AccelerationRawZ / UNITS_PER_G;

    //         return accel;
    //     }
    // }




    class SPI_Reader
    {

        public eStrike readBbbDatapoint()
        {
            return new eStrike();
        }
    }

}