#include <ArduinoBLE.h>

const float VERSION = 1.0;

const byte EXTERNAL_LED = 2;
const byte PUSH_BUTTON = 3;
const byte SWITCH_MODE_F = 10;
const byte SWITCH_MODE_A = 9;
const byte SWITCH_MODE_M = 8;

volatile char ButtonMode;
volatile char lastButtonMode;

// Uncomment only the name of the anchor node being flashed to
char BLE_Name[9] = "AN-A0-NA";
volatile char currentLocation[3] = "A0";
volatile byte locationIndex = 0;
//char BLE_Name[9] = "AN-A4-NA";
//volatile char currentLocation[3] = "A4";
//volatile byte locationIndex = 4;
//char BLE_Name[9] = "AN-E0-NA";
//volatile char currentLocation[3] = "E0";
//volatile byte locationIndex = 20;
//char BLE_Name[9] = "AN-E4-NA";
//volatile char currentLocation[3] = "E4";
//volatile byte locationIndex = 24;


char locationArray[25][3] = {"A0", "A1", "A2", "A3", "A4",
                             "B0", "B1", "B2", "B3", "B4",
                             "C0", "C1", "C2", "C3", "C4",
                             "D0", "D1", "D2", "D3", "D4",
                             "E0", "E1", "E2", "E3", "E4"
                            };

// the following variables are unsigned longs because the time, measured in
// milliseconds, will quickly become a bigger number than can be stored in an int.
long unsigned int lastDebounceTime = 0;  // the last time the output pin was toggled
long unsigned int debounceDelay = 50;    // the debounce time; increase if the output flickers

long unsigned int currentMillis = 0;
long unsigned int previousMillis = 0;
int period = 1000;
int scanCount = 0;
int ledState = HIGH;
int buttonState;
int lastReadingState = LOW;

bool scanState = true;

byte scannedMobileData[4] = {0x00, 0x00, 0x00, 0x00};

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
      anchor();
      break;
    case 'M':
      mobileMale();
      break;
    case 'F':
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
  BLE_Name[4] = '0';
  BLE_Name[6] = 'M';
  BLE_Name[7] = 'A';
  BLE.stopAdvertise();
  BLE.setLocalName(BLE_Name);
  BLE.advertise();
}

void mobileFemale(void) {
  BLE_Name[0] = 'M';
  BLE_Name[1] = 'O';
  BLE_Name[3] = '0';
  BLE_Name[4] = '0';
  BLE_Name[6] = 'F';
  BLE_Name[7] = 'E';
  BLE.stopAdvertise();
  BLE.setLocalName(BLE_Name);
  BLE.advertise();
}

void anchor(void) {
  BLE_Name[0] = 'A';
  BLE_Name[1] = 'N';
  BLE_Name[3] = currentLocation[0];
  BLE_Name[4] = currentLocation[1];
  BLE_Name[6] = 'N';
  BLE_Name[7] = 'A';
  BLE.stopAdvertise();
  BLE.setLocalName(BLE_Name);
  BLE.advertise();
}

void setup() {
  pinMode(PUSH_BUTTON, INPUT);
  pinMode(SWITCH_MODE_M, INPUT);
  pinMode(SWITCH_MODE_A, INPUT);
  pinMode(SWITCH_MODE_F, INPUT);
  pinMode(EXTERNAL_LED, OUTPUT);

  digitalWrite(EXTERNAL_LED, LOW);
  
  if (!BLE.begin())
  {
    while (1)
    {
      digitalWrite(EXTERNAL_LED, ledState);
      if (currentMillis - previousMillis >= 500) {
        previousMillis = currentMillis;
        ledState != ledState;
      }
    }
  }
  BLE.setManufacturerData(scannedMobileData, 4);
}

void loop() {
  lastReadingState = debounce(PUSH_BUTTON, lastReadingState);
  if (digitalRead(SWITCH_MODE_A)==HIGH)
  {
    if (ButtonMode != 'A')
    {
      lastButtonMode = ButtonMode;
      ButtonMode = 'A';
      anchor();
    }
    if (scanState)
    {
      digitalWrite(EXTERNAL_LED, HIGH);
      BLE.stopAdvertise();
      BLE.scan();
      currentMillis = millis();
      while (millis() < (currentMillis + period))
      {
        scannedNode = BLE.available();
        if (scannedNode.localName() == "MO-00-MA")
        {
          if (scannedMobileData[1] != (byte)scannedNode.rssi())
          {
            scannedMobileData[0] = 0x11;
            scannedMobileData[1] = (byte)scannedNode.rssi();
            BLE.setManufacturerData(scannedMobileData, 4);
          }
        }
        if (scannedNode.localName() == "MO-00-FE")
        {
          if (scannedMobileData[3] != (byte)scannedNode.rssi())
          {
            scannedMobileData[2] = 0x11;
            scannedMobileData[3] = (byte)scannedNode.rssi();
            BLE.setManufacturerData(scannedMobileData, 4);
          }
        }
      }
      scanState = !scanState;
      BLE.stopScan();
      digitalWrite(EXTERNAL_LED, LOW);
      BLE.advertise();
      currentMillis = millis();
    }
    if (millis() > (currentMillis + period))
    {
      scanState = !scanState;
    }
  }
1
}
