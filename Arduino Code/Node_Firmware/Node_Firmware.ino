#include <ArduinoBLE.h>

const float VERSION = 1.0;

const byte EXTERNAL_LED = 2;
const byte PUSH_BUTTON = 3;
const byte SWITCH_MODE_F = 10;
const byte SWITCH_MODE_A = 9;
const byte SWITCH_MODE_M = 8;

volatile char ButtonMode;
volatile char lastButtonMode;
volatile char currentLocation[3] = "A0";
volatile byte locationIndex = 0;
char BLE_Name[9] = "AN-A0-NA";

char locationArray[25][3] = {"A0", "A1", "A2", "A3", "A4",
                             "B0", "B1", "B2", "B3", "B4",
                             "C0", "C1", "C2", "C3", "C4",
                             "D0", "D1", "D2", "D3", "D4",
                             "E0", "E1", "E2", "E3", "E4"
                            };

// the following variables are unsigned longs because the time, measured in
// milliseconds, will quickly become a bigger number than can be stored in an int.
long unsigned int lastDebounceTime = 0;  // the last time the output pin was toggled
long unsigned int debounceDelay = 40;    // the debounce time; increase if the output flickers

long unsigned int currentMillis = 0;
long unsigned int previousMillis = 0;
int ledState = HIGH;
int buttonState;
int lastReadingState = LOW;

BLEService MobileServiceA("87adced0-7bf9-11eb-9439-0242ac130002");
BLEService MobileServiceB("87adced9-7bf9-11eb-9439-0242ac130002");

BLEByteCharacteristic IndexCharA("87adced1-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLEByteCharacteristic RSSICharA("87adced2-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLECharCharacteristic GenderCharA("87adced3-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);

BLEByteCharacteristic IndexCharB("87adced8-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLEByteCharacteristic RSSICharB("87adced7-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);
BLECharCharacteristic GenderCharB("87adced6-7bf9-11eb-9439-0242ac130002", BLERead | BLEBroadcast | BLENotify);

BLEDevice scannedNode;

int debounce(int pinDebounce, int lastButtonState)
{
  int reading = digitalRead(pinDebounce);

  // check to see if you just pressed the button
  // (i.e. the input went from LOW to HIGH), and you've waited long enough
  // since the last press to ignore any noise:

  // If the switch changed, due to noise or pressing:
  if (reading != lastButtonState) {
    // reset the debouncing timer
    lastDebounceTime = millis();
  }

  if ((millis() - lastDebounceTime) > debounceDelay) {
    // whatever the reading is at, it's been there for longer than the debounce
    // delay, so take it as the actual current state:

    // if the button state has changed:
    if (reading != buttonState) {
      buttonState = reading;

      if (buttonState == HIGH)
      {
        buttonAction();
      }
    }
  }
  return lastButtonState = reading;
}

void buttonAction()
{
  digitalWrite(EXTERNAL_LED, HIGH);
  switch (ButtonMode) {
    case 'A':
      locationIndex++;
      locationIndex %= 25;
      currentLocation[0] = locationArray[locationIndex][0];
      currentLocation[1] = locationArray[locationIndex][1];
      BLE_Name[3] = currentLocation[0];
      BLE_Name[4] = currentLocation[1];
      anchor();
      break;
    case 'M':
      if (BLE_Name[4] == '1') BLE_Name[4] = '0';
      else BLE_Name[4] = '1';
      mobileMale();
      break;
    case 'F':
      if (BLE_Name[4] == '1') BLE_Name[4] = '0';
      else BLE_Name[4] = '1';
      mobileFemale();
      break;
    default:
      Serial.print("Unsure of Mode Selected\n");
      break;
  }
  digitalWrite(EXTERNAL_LED, LOW);
}

void mobileMale(void) {
  Serial.println("Mobile Male Initialize");
  if (lastButtonMode = 'A')
  {
    BLE.end();
    if (!BLE.begin())
    {
      Serial.println("Starting Mobile Male BLE Failed!");
      while (1)
      {
        digitalWrite(EXTERNAL_LED, ledState);
        if (currentMillis - previousMillis >= 500) {
          previousMillis = currentMillis;
          ledState != ledState;
        }
      }
    }
  }
  else BLE.stopAdvertise();
  BLE_Name[0] = 'M';
  BLE_Name[1] = 'O';
  BLE_Name[3] = '0';
  BLE_Name[6] = 'M';
  BLE_Name[7] = 'A';
  Serial.println(BLE_Name);
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Mobile Node");
  BLE.advertise();
}

void mobileFemale(void) {
  Serial.println("Mobile Female Initialize");
  if (lastButtonMode = 'A')
  {
    BLE.end();
    if (!BLE.begin())
    {
      Serial.println("Starting Mobile Male BLE Failed!");
      while (1)
      {
        digitalWrite(EXTERNAL_LED, ledState);
        if (currentMillis - previousMillis >= 500) {
          previousMillis = currentMillis;
          ledState != ledState;
        }
      }
    }
  }
  else BLE.stopAdvertise();
  BLE_Name[0] = 'M';
  BLE_Name[1] = 'O';
  BLE_Name[3] = '0';
  BLE_Name[6] = 'F';
  BLE_Name[7] = 'E';
  Serial.println(BLE_Name);
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Mobile Node");
  BLE.advertise();
}

void anchor(void) {
  Serial.println("Anchor Initialize");
//  BLE.end();
//  if (!BLE.begin())
//  {
//    Serial.println("Starting Anchor BLE Failed!");
//    while (1)
//    {
//      digitalWrite(EXTERNAL_LED, ledState);
//      if (currentMillis - previousMillis >= 500) {
//        previousMillis = currentMillis;
//        ledState != ledState;
//      }
//    }
//  }
  BLE_Name[0] = 'A';
  BLE_Name[1] = 'N';
  BLE_Name[6] = 'N';
  BLE_Name[7] = 'A';
  Serial.println(BLE_Name);
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
  IndexCharA.broadcast();
  RSSICharA.broadcast();
  GenderCharA.broadcast();
  IndexCharB.broadcast();
  RSSICharB.broadcast();
  GenderCharB.broadcast();
  BLE.advertise();
}

void setup() {
  Serial.begin(9600);
  Serial.print("Firmware Version: ");
  Serial.print(VERSION);
  Serial.print("\n");

  pinMode(PUSH_BUTTON, INPUT);
  pinMode(SWITCH_MODE_M, INPUT);
  pinMode(SWITCH_MODE_A, INPUT);
  pinMode(SWITCH_MODE_F, INPUT);
  pinMode(EXTERNAL_LED, OUTPUT);

  digitalWrite(EXTERNAL_LED, LOW);

  if (!BLE.begin())
  {
    Serial.println("Starting Anchor BLE Failed!");
    while (1)
    {
      digitalWrite(EXTERNAL_LED, ledState);
      if (currentMillis - previousMillis >= 500) {
        previousMillis = currentMillis;
        ledState != ledState;
      }
    }
  }
}

void loop() {
  lastReadingState = debounce(PUSH_BUTTON, lastReadingState);
  if (digitalRead(SWITCH_MODE_A) == HIGH)
  {
    if (ButtonMode != 'A')
    {
      Serial.println("Reset Anchor Location State");
      lastButtonMode = ButtonMode;
      ButtonMode = 'A';
    }
    BLE.scan();
    scannedNode = BLE.available();
    if (scannedNode.localName() = "MO-00-MA")
    {
      IndexCharA.writeValue((byte)0x00);
      RSSICharA.writeValue((int)scannedNode.rssi());
      GenderCharA.writeValue((char)'M');
    }
    if (scannedNode.localName() = "MO-01-MA")
    {
      IndexCharB.writeValue((byte)0x01);
      RSSICharB.writeValue((int)scannedNode.rssi());
      GenderCharB.writeValue((char)'M');
    }
    if (scannedNode.localName() = "MO-00-FE")
    {
      IndexCharA.writeValue((byte)0x00);
      RSSICharA.writeValue((int)scannedNode.rssi());
      GenderCharA.writeValue((char)'F');
    }
    if (scannedNode.localName() = "MO-01-FE")
    {
      IndexCharB.writeValue((byte)0x01);
      RSSICharB.writeValue((int)scannedNode.rssi());
      GenderCharB.writeValue((char)'F');
    }
    BLE.stopScan();
  }
  if (digitalRead(SWITCH_MODE_M) == HIGH && ButtonMode != 'M')
  {
    Serial.println("Male Battery Test State");
    lastButtonMode = ButtonMode;
    ButtonMode = 'M';
  }
  if (digitalRead(SWITCH_MODE_F) == HIGH && ButtonMode != 'F')
  {
    Serial.println("Female Battery Test State");
    lastButtonMode = ButtonMode;
    ButtonMode = 'F';
  }
}
