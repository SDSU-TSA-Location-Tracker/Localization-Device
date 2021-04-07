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

byte scannedMobileData[8] = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};

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
      break;
  }
  digitalWrite(EXTERNAL_LED, LOW);
}

void mobileMale(void) {
  BLE_Name[0] = 'M';
  BLE_Name[1] = 'O';
  BLE_Name[3] = '0';
  BLE_Name[6] = 'M';
  BLE_Name[7] = 'A';
  Serial.println(BLE_Name);
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Mobile Node");
  BLE.setManufacturerData(0,1);
  BLE.advertise();
}

void mobileFemale(void) {
  BLE_Name[0] = 'M';
  BLE_Name[1] = 'O';
  BLE_Name[3] = '0';
  BLE_Name[6] = 'F';
  BLE_Name[7] = 'E';
  //Serial.println(BLE_Name);
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Mobile Node");
  BLE.setManufacturerData(0,1);
  BLE.advertise();
}

void anchor(void) {
  BLE_Name[0] = 'A';
  BLE_Name[1] = 'N';
  BLE_Name[6] = 'N';
  BLE_Name[7] = 'A';
  //Serial.println(BLE_Name);
  BLE.setLocalName(BLE_Name);
  BLE.setDeviceName("Anchor Node");
  BLE.advertise();
}

void setup() {
  //Serial.begin(9600);
  //Serial.print("Firmware Version: ");
  //Serial.print(VERSION);
  //Serial.print("\n");

  pinMode(PUSH_BUTTON, INPUT);
  pinMode(SWITCH_MODE_M, INPUT);
  pinMode(SWITCH_MODE_A, INPUT);
  pinMode(SWITCH_MODE_F, INPUT);
  pinMode(EXTERNAL_LED, OUTPUT);

  digitalWrite(EXTERNAL_LED, LOW);

  if (!BLE.begin())
  {
    //Serial.println("Starting Anchor BLE Failed!");
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
      lastButtonMode = ButtonMode;
      ButtonMode = 'A';
      anchor();
    }
    BLE.scan();
    BLEDevice scannedNode = BLE.available();
    if (scannedNode.localName() == "MO-00-MA")
    {
      scannedMobileData[0] = 0x11;
      scannedMobileData[1] = (byte)scannedNode.rssi();
      BLE.setManufacturerData(scannedMobileData, 8);
      BLE.advertise();
    }
    if (scannedNode.localName() == "MO-01-MA")
    {
      scannedMobileData[2] = 0x11;
      scannedMobileData[3] = (byte)scannedNode.rssi();
      BLE.setManufacturerData(scannedMobileData, 8);
      BLE.advertise();
    }
    if (scannedNode.localName() == "MO-00-FE")
    {
      scannedMobileData[4] = 0x11;
      scannedMobileData[5] = (byte)scannedNode.rssi();
      BLE.setManufacturerData(scannedMobileData, 8);
      BLE.advertise();
    }
    if (scannedNode.localName() == "MO-01-FE")
    {
      scannedMobileData[6] = 0x11;
      scannedMobileData[7] = (byte)scannedNode.rssi();
      BLE.setManufacturerData(scannedMobileData, 8);
      BLE.advertise();
    }
  }
  if (digitalRead(SWITCH_MODE_M) == HIGH && ButtonMode != 'M')
  {
    lastButtonMode = ButtonMode;
    ButtonMode = 'M';
    mobileMale();
  }
  if (digitalRead(SWITCH_MODE_F) == HIGH && ButtonMode != 'F')
  {
    lastButtonMode = ButtonMode;
    ButtonMode = 'F';
    mobileFemale();
  }
}
