// Interrupt safe EventManager
// see: github.com/igormiktor/arduino-EventManager
// #define EVENTMANAGER_EVENT_QUEUE_SIZE   16
#include <EventManager.h>
EventManager myEventManager;
#define HEART_BEAT_DETECTED  1 // Define an event code

// Seeed Ethernet / SD Shield
#include <DhcpV2_0.h>
#include <DnsV2_0.h>
#include <EthernetClientV2_0.h>
#include <EthernetServerV2_0.h>
#include <EthernetUdpV2_0.h>
#include <EthernetV2_0.h>
#include <utilV2_0.h>
#include <SD.h>
#include <SPI.h>
#define W5200_CS  10
//#define SDCARD_CS 4
//File log_file;

// Seeed RGB LCD Display
#include <Wire.h>
#include <rgb_lcd.h>
rgb_lcd lcd;

// Seeed Push Button
#define BUTTON_PRESS_DETECTED  2 // Define an event code

// Seeed Heart Rate Ear Clip
unsigned char counter = 0;
unsigned int heart_rate = 0;
unsigned long temp[21];
unsigned long sub = 0;
unsigned long current_time;
volatile unsigned char state = LOW;
bool data_effect = true;
const int max_heartpluse_duty = 2000; //you can change it follow your system's request.2000 meams 2 seconds. System return error if the duty overtrip 2 second.

// Electrodermal response
const int GSR = A2;
int electrodermal_reading;

// Logging constants
//unsigned char file_counter = 0;

void setup() {

  // set up the LCD
  lcd.begin(16, 2); // Number and rows
  lcd.println("Waiting for serial connection...");

  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  lcd.clear();

  // set up the Push Button
  pinMode(3, INPUT); // initialize the pushbutton pin as an input:


  // set up the pushbutton interrupt
  //Serial.println(F("Setting up pushbutton"));
  myEventManager.addListener( BUTTON_PRESS_DETECTED, myListener ); // set up interrupt-safe event listener
  int button_pin = 3;
  pinMode(button_pin, INPUT);
  attachInterrupt(digitalPinToInterrupt(button_pin), buttonPressDetected, RISING); //set interrupt 1,digital port 3

  // set up SD card
  /*Serial.print(F("Initializing SD card..."));
  pinMode(W5200_CS, OUTPUT); // CS is pin 4, SS must be left as output
  digitalWrite(W5200_CS, HIGH); // disconnect w5200
  pinMode(SDCARD_CS, OUTPUT);
  if (!SD.begin(SDCARD_CS)) {
    Serial.println(F("SD Card initialization failed!"));
    return;
  }
  Serial.println(F("SD Card Initialized."));
  */
  startNewLog();

  // set up the Heart Rate monitor
  myEventManager.addListener( HEART_BEAT_DETECTED, myListener ); // set up interrupt-safe event listener
  Serial.println("Waiting for heart rate to be put on");
  delay(2000);
  initializeHeartRateArray();
  Serial.println("Heart rate test begin.");
  int heart_interrupt_id = digitalPinToInterrupt(2);
  attachInterrupt(heart_interrupt_id, heartRateBeatDetected, RISING); //set interrupt 0,digital port 2

}

void loop() {

  //process events in the event queue and dispatch them to listeners
  myEventManager.processEvent();

}

// This is an interrupt-safe event handler.
void myListener( int eventCode, int eventParam ) {

  Serial.flush();

  if (eventCode == HEART_BEAT_DETECTED) {
    current_time = millis();

    // Grab the GSR (electrodermal reading)
    // Ideally this would happen at the same time as the heartbeat, but
    // it should be close enough for our purposes
    electrodermal_reading = analogRead(GSR);

    // Update LCD
    lcd.clear();
    if (state == LOW) {
      lcd.setRGB(50, 0, 0);
    } else {
      lcd.setRGB(180, 0, 0);
    }
    lcd.setCursor(0, 0);
    lcd.println("BPM: " + (String)heart_rate);
    lcd.setCursor(0, 1);
    lcd.println("EDR: " + (String)electrodermal_reading);


    //Serial.print("Writing to heart.txt...");
    //log_file.println((String)current_time + ",beat," + (String)heart_rate + "," + (String)electrodermal_reading);

    // Send data to serial too
    Serial.println((String)current_time + "," + (String)heart_rate + "," + (String)electrodermal_reading);

    //Serial.println(current_time + ", beat" + heart_rate);

  } else if (eventCode == BUTTON_PRESS_DETECTED) {

    Serial.println(F("Button pressed!"));
    startNewLog();

  }

}

void startNewLog() {
  /*if (log_file) {
    // Close log file
    log_file.close();
  }
  String file_name = "LOG" + (String)file_counter + ".TXT";
  char file_name_char_array[file_name.length()];
  file_name.toCharArray(file_name_char_array, file_name.length() + 1);

  while (SD.exists(file_name_char_array)) {
    // Increment log file count number
    file_counter++;
    file_name = "LOG" + (String)file_counter + ".TXT";
    file_name_char_array[file_name.length()];
    file_name.toCharArray(file_name_char_array, file_name.length() + 1);
  }

  // Open new log file
  log_file = SD.open(file_name_char_array, FILE_WRITE);
  //write to the file after it's successfully opened or created:
  if (log_file) {
    log_file.println("Time,IsBeat?,BPM,EDR");
    Serial.println("Starting new logging set..." + (String)file_name_char_array);
    Serial.println("Time,IsBeat?,BPM,EDR");
  } else {
    Serial.println(F("Could not start new log. :-("));
    return;
  }*/

  //Serial.println("Starting log...");
  //Serial.println("Time,IsBeat?,BPM,EDR");
}


void calculateHeartRate()//calculate the heart rate
{
  if (data_effect)
  {
    heart_rate = 1200000 / (temp[20] - temp[0]); //60*20*1000/20_total_time
    //Serial.print("Heart_rate_is:\t");
    //Serial.println(heart_rate);
    //current_time = millis();
    //Serial.println(current_time);
    //connection->write(2, (uint8_t*)&heart_rate);
    //ADB::poll();
  }
  data_effect = 1; //sign bit
}
// This is an interrupt -- keep it minimal!
void buttonPressDetected() {
  // notify listener that a beat has occurred
  current_time =  millis();
  myEventManager.queueEvent(BUTTON_PRESS_DETECTED, current_time );
}

// This is an interrupt -- keep it minimal!
void heartRateBeatDetected() {

  current_time =  millis();

  temp[counter] = current_time;
  state = !state;

  //Serial.println(counter,DEC);
  //Serial.println(temp[counter]);
  switch (counter)
  {
    case (0):
      sub = temp[counter] - temp[20];
      //Serial.println(sub);
      break;
    default:
      sub = temp[counter] - temp[counter - 1];
      //Serial.println(sub);
      break;
  }
  if (sub > max_heartpluse_duty) //set 2 seconds as max heart pluse duty
  {
    data_effect = 0; //sign bit
    counter = 0;
    Serial.println(F("Heart rate measure error: test will restart!"));
    initializeHeartRateArray();
  }
  if (counter == 20 && data_effect)
  {
    counter = 0;
    calculateHeartRate();
  }
  else if (counter != 20 && data_effect)
    counter++;
  else
  {
    counter = 0;
    data_effect = 1;
  }

  // notify listener that a beat has occurred
  myEventManager.queueEvent(HEART_BEAT_DETECTED, current_time );
}

void initializeHeartRateArray() {
  for (unsigned char i = 0; i != 20; ++i)
  {
    temp[i] = 0;
  }
  temp[20] = millis();
}

