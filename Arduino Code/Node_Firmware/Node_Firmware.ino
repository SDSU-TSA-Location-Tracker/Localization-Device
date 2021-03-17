#include <ArduinoBLE.h>

const float VERSION = 1.0;

const byte EXTERNAL_LED = 2;
const byte PUSH_BUTTON = 3;
const byte SWITCH_MODE_A = 11;
const byte SWITCH_MODE_B = 10;
const byte SWITCH_MODE_C = 9;

volatile char MODE;
volatile char currentLocation[3] = "A0";
volatile byte locationIndex = 0;
char BLE_Name[9];

char locationArray[25][3] = {"A0", "A1", "A2", "A3", "A4",
                             "B0", "B1", "B2", "B3", "B4",
                             "C0", "C1", "C2", "C3", "C4",
                             "D0", "D1", "D2", "D3", "D4",
                             "E0", "E1", "E2", "E3", "E4"
                            };

unsigned long currentMillis = 0;
unsigned long previousMillis = 0;
unsigned int LED_STATE;

BLEService MobileServiceA("87adced0-7bf9-11eb-9439-0242ac130002");
BLEService MobileServiceB("87adced9-7bf9-11eb-9439-0242ac130002");

BLEByteCharacteristic IndexCharA("87adced1-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLEByteCharacteristic RSSICharA("87adced2-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLECharCharacteristic GenderCharA("87adced3-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);

BLEByteCharacteristic IndexCharB("87adced8-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLEByteCharacteristic RSSICharB("87adced7-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLECharCharacteristic GenderCharB("87adced6-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);

BLEDevice scannedNode;

void setup() {
  Serial.begin(9600);
  Serial.print("Firmware Version: ");
  Serial.print(VERSION);
  Serial.print("\n");

  pinMode(PUSH_BUTTON, INPUT);
  pinMode(SWITCH_MODE_A, INPUT);
  pinMode(SWITCH_MODE_B, INPUT);
  pinMode(SWITCH_MODE_C, INPUT);
  pinMode(EXTERNAL_LED, OUTPUT);

  attachInterrupt(digitalPinToInterrupt(PUSH_BUTTON), buttonIncrement, RISING);
  attachInterrupt(digitalPinToInterrupt(SWITCH_MODE_A), mobileMale, RISING);
  attachInterrupt(digitalPinToInterrupt(SWITCH_MODE_B), anchor, RISING);
  attachInterrupt(digitalPinToInterrupt(SWITCH_MODE_C), mobileFemale, RISING);

  LED_STATE = LOW;
  digitalWrite(EXTERNAL_LED, LED_STATE);
}

void loop() {
  if (MODE == 'B')
  {
    BLE.scanForName("MO-00-MA");
    scannedNode = BLE.available();
    if (scannedNode)
    {
      IndexCharA.writeValue((byte)0x00);
      RSSICharA.writeValue((byte)scannedNode.rssi());
      GenderCharA.writeValue((char)'M');
    }
    BLE.stopScan();
    BLE.scanForName("MO-01-MA");
    scannedNode = BLE.available();
    if (scannedNode)
    {
      IndexCharB.writeValue((byte)0x01);
      RSSICharB.writeValue((byte)scannedNode.rssi());
      GenderCharB.writeValue((char)'M');
    }
    BLE.stopScan();
    BLE.scanForName("MO-00-FE");
    scannedNode = BLE.available();
    if (scannedNode)
    {
      IndexCharA.writeValue((byte)0x00);
      RSSICharA.writeValue((byte)scannedNode.rssi());
      GenderCharA.writeValue((char)'F');
    }
    BLE.stopScan();
    BLE.scanForName("MO-01-FE");
    scannedNode = BLE.available();
    if (scannedNode)
    {
      IndexCharB.writeValue((byte)0x01);
      RSSICharB.writeValue((byte)scannedNode.rssi());
      GenderCharB.writeValue((char)'F');
    }
    BLE.stopScan();
  }
  else {
    BLE.stopScan();
  }
}


void buttonIncrement(void) {
  if (MODE == 'B')
  {
    if (currentLocation != "E0")
    {
      locationIndex++;
      currentLocation[0] = locationArray[locationIndex][0];
      currentLocation[1] = locationArray[locationIndex][1];
    }
    else
    {
      locationIndex = 0;
      currentLocation[0] = locationArray[locationIndex][0];
      currentLocation[1] = locationArray[locationIndex][1];
    }
    BLE_Name[3] = currentLocation[0];
    BLE_Name[4] = currentLocation[1];
  }
  else
  {
    if (BLE_Name[4] == '1')  BLE_Name[4] = '0';
    else BLE_Name[4] = '1';
  }
  BLE.stopAdvertise();
  BLE.setLocalName(BLE_Name);
  BLE.advertise();
}

void mobileMale(void) {
  BLE.end();
  if (!BLE.begin())
  {
    Serial.println("Starting Mobile Male BLE Failed!");
    while (1)
    {
      digitalWrite(EXTERNAL_LED, LED_STATE);
      if (currentMillis - previousMillis >= 500) {
        previousMillis = currentMillis;
        LED_STATE != LED_STATE;
      }
    }
  }
  MODE = 'A';
  BLE_Name[0] = 'M';
  BLE_Name[1] = 'O';
  BLE_Name[2] = '-';
  BLE_Name[3] = '0';
  BLE_Name[4] = '0';
  BLE_Name[5] = '-';
  BLE_Name[6] = 'M';
  BLE_Name[7] = 'A';
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Mobile Node");
  BLE.advertise();
}

void mobileFemale(void) {
  BLE.end();
  if (!BLE.begin())
  {
    Serial.println("Starting Mobile Female BLE Failed!");
    while (1)
    {
      digitalWrite(EXTERNAL_LED, LED_STATE);
      if (currentMillis - previousMillis >= 500) {
        previousMillis = currentMillis;
        LED_STATE != LED_STATE;
      }
    }
  }
  MODE = 'C';
  BLE_Name[0] = 'M';
  BLE_Name[1] = '0';
  BLE_Name[2] = '-';
  BLE_Name[3] = '0';
  BLE_Name[4] = '0';
  BLE_Name[5] = '-';
  BLE_Name[6] = 'F';
  BLE_Name[7] = 'E';
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Mobile Node");
  BLE.advertise();
}

void anchor(void) {
  BLE.end();
  if (!BLE.begin())
  {
    Serial.println("Starting Anchor BLE Failed!");
    while (1)
    {
      digitalWrite(EXTERNAL_LED, LED_STATE);
      if (currentMillis - previousMillis >= 500) {
        previousMillis = currentMillis;
        LED_STATE != LED_STATE;
      }
    }

  }
  MODE = 'B';
  BLE_Name[0] = 'A';
  BLE_Name[1] = 'N';
  BLE_Name[2] = '-';
  BLE_Name[3] = 'A';
  BLE_Name[4] = '0';
  BLE_Name[5] = '-';
  BLE_Name[6] = 'N';
  BLE_Name[7] = 'A';
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Anchor Node");
  BLE.addService(MobileServiceA);
  MobileServiceA.addCharacteristic(IndexCharA);
  MobileServiceA.addCharacteristic(RSSICharA);
  MobileServiceA.addCharacteristic(GenderCharA);
  BLE.setAdvertisedService(MobileServiceA);
  BLE.addService(MobileServiceB);
  MobileServiceB.addCharacteristic(IndexCharB);
  MobileServiceB.addCharacteristic(RSSICharB);
  MobileServiceB.addCharacteristic(GenderCharB);
  BLE.setAdvertisedService(MobileServiceB);
  BLE.advertise();
  IndexCharA.broadcast();
  RSSICharA.broadcast();
  GenderCharA.broadcast();
  IndexCharB.broadcast();
  RSSICharB.broadcast();
  GenderCharB.broadcast();
}
