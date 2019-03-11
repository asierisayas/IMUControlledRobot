//Note that the motor class must be included
//Link is below
//https://developer.mbed.org/cookbook/Motor
#include "mbed.h"
#include "Motor.h"
#include <string>
 
//Create both left and right motor objects
Motor mright(p21, p5, p6); // pwm, fwd, rev
Motor mleft(p22, p7, p8);
//Create the Bluetooth serial in
Serial blue(p28,p27);
//Create two analog in objects for the IR sensors
AnalogIn IRSensor1(p19);
AnalogIn IRSensor2(p20);
 
//Buffer to read in serial data for left and right motor values
char left[3];
char right[3];
//Initialize IR sensor values
float Ir1 = IRSensor1.read();
float Ir2 = IRSensor2.read();
//Initialize speed to write to motors
float mLeft = 0;
float mRight = 0;
 
int main()
{
    while(1) {
 
        //Read in serial data for left and right motors
        blue.gets(left,3);
        blue.gets(right,3);
        //Convert serial data to floats that can be written to the motors
        mLeft = atof(left)/100;
        mRight = atof(right)/100;
        //Read in current IR sensor values
        Ir1 = IRSensor1.read();
        Ir2 = IRSensor2.read();
        
        //If the robot is too close to an object, either the left or right wheel will shutt
        //off to force the robot to turn. Otherwise, it will move based directly on the
        //incoming serial data
        if((Ir1>0.23)||(Ir2>0.23)) {
            if(mLeft>=mRight) {
                mleft.speed(mLeft);
                mright.speed(0);
            } else {
                mleft.speed(0);
                mright.speed(mRight);
            }
        } else {
            mleft.speed(mLeft);
            mright.speed(mRight);
        }
 
 
    }
}
