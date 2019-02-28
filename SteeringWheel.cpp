//Steering Wheel Code
//Note that the LSM9DS1.h must be included
// Link to the MBED page is below
// https://developer.mbed.org/components/LSM9DS1-IMU/
#include "mbed.h"
#include "LSM9DS1.h"
#include <string>
 
 
DigitalOut myled(LED1);
Serial pc(USBTX, USBRX);
float acceleration = 0;
float steering= 0;
//Buffer for serial data that will be written to PC
char values [22];
//Store the Y and Z values for data
float y=0;
float z=0;
int main()
{
    LSM9DS1 IMU(p9, p10, 0xD6, 0x3C);
    IMU.begin();
    if (!IMU.begin()) {
        pc.printf("Failed to communicate with LSM9DS1.\n");
    }
    IMU.calibrate(1);
    
    wait(3);
    myled=1;
    //LED will turn on 3 seconds after IMU calibration
    //LED1 will turn on once MBED begins to write out data
    while(1) {
 
        while(!IMU.accelAvailable());
        IMU.readAccel();
 
        //These conditional statements create a "dead-zone" for the wheel, to make it less
        //sensitive and more usable. Additionally, the IMU data will never exceed .9 to so
        //as to limit the maximum speed and steering for the robot.
        if(IMU.calcAccel(IMU.ay)>.9) {
            y=.9;
        } else if (IMU.calcAccel(IMU.ay)<-.9) {
            y=-.9;
        } else if(IMU.calcAccel(IMU.ay)>.1||IMU.calcAccel(IMU.ay)<-.1) {
            y=IMU.calcAccel(IMU.ay);
        } else {
            y=0;
        }
        
        
        if(IMU.calcAccel(IMU.az)>.9) {
            z=.9;
        } else if((IMU.calcAccel(IMU.az)>.1)) {
            z=IMU.calcAccel(IMU.az);;
        } else {
            z=0;
        }
        //IMU data is written to char buffer, so it can be written to serial port
        //Exclamation point is a delimiter that will be used to parse the string
        //in the later C# GUI
        sprintf(values,"%8f!%8f",y, z);
        pc.printf("%s\n",values);
        //Limit the rate at which new data is sent
        wait(0.1);
 
    }
}
 