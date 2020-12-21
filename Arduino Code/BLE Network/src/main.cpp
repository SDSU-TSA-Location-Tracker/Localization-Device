#include <Arduino.h>
#include <ArduinoBLE.h>

#define RED 22
#define BLUE 24
#define GREEN 23
#define LED_PWR 25u

// website for UUID https://www.uuidgenerator.net/version1

// create a service id to act as a category for specific information
// UUID is unique for each bluetooth device's services

BLEService ledservice("6995d586-4351-11eb-b378-0242ac130002"); // Device 1
//BLEService ledservice("f596897a-432b-11eb-b378-0242ac130002"); // Device 2

// establish a characteristic with the same UUID with read and write priveledges from the connected device
BLECharCharacteristic switchChar("f596897a-432b-11eb-b378-0242ac130002", BLERead | BLEWrite);


// When a device connects to the arduino, enter here
void blePeripheralConnectHandler(BLEDevice central)
{
  // print the address of the connected device and turn on blue led
  Serial.print("Connect event, central: ");
  Serial.println(central.address());
  delay(100);
  Serial.print("RSSI Value: ");
  Serial.println(central.rssi());
  digitalWrite(BLUE, LOW);
}

void blePeripheralDisconnectHandler(BLEDevice central)
{
  // print the address of the disconnected device and turn off blue led
  Serial.print("Disconnect event, central: ");
  Serial.println(central.address());
  digitalWrite(BLUE, HIGH);
}

void switchCharacteristicWritten(BLEDevice central, BLECharacteristic characteristic)
{
  // enter upon characteristic write request
  Serial.print("Characteristic event, written: ");

  // if the new written data is 1 enter and turn on power led
  if(switchChar.value() == 1)
  {
    Serial.println("LED ON");
    digitalWrite(LED_PWR, HIGH);
  }

  // any written value other than 1 turn off power led
  else
  {
    Serial.println("LED OFF");
    digitalWrite(LED_PWR, LOW);
  }
}

void setup() {
  // begin serial protocol with 9600 baud rate
  Serial.begin(9600);

  // make the blue led pin an output
  pinMode(BLUE, OUTPUT);
  pinMode(LED_PWR, OUTPUT);

  digitalWrite(BLUE, LOW);
  digitalWrite(LED_PWR, LOW);
  // Begins the BLE protocol and will print msg if failure
  if(!BLE.begin())
  {
    Serial.println("starting BLE failed");
    while(1);
  }

  // set name and UUID of device on bluetooth advertise list
  BLE.setLocalName("Arduino 1"); // Device 1
  //BLE.setLocalName("Arduino 2"); // Device 2
  BLE.setAdvertisedService(ledservice);

  // add characteristic and service to BLE device
  ledservice.addCharacteristic(switchChar);
  BLE.addService(ledservice);

  // initialize the types of events and their handler names
  BLE.setEventHandler(BLEConnected, blePeripheralConnectHandler);
  BLE.setEventHandler(BLEDisconnected, blePeripheralDisconnectHandler);
  switchChar.setEventHandler(BLEWritten, switchCharacteristicWritten);

  // initialize the characteristic to be 0
  switchChar.setValue(0);

  // begin advertising and print status in serial monitor
  BLE.advertise();
  Serial.println("Bluetooth device active, waiting for connections...");
}

void loop() {

  // poll for events to occur
  BLE.poll();
  BLE.scan(); // scans for any nearby advertising device
  BLEDevice peripheral = BLE.available(); // save one of the nearby devices to peripheral

  if(peripheral)
  {
    // if the nearby device has a visable local name and is "Arduino 2"
    // print the name of the device and its rssi value
    if(peripheral.hasLocalName())
    {
      if(peripheral.localName()=="Arduino 2")
      {
        Serial.print("Local name: ");
        Serial.println(peripheral.localName());
        Serial.print("RSSI: ");
        Serial.println(peripheral.rssi());
      }
    }
  }
}