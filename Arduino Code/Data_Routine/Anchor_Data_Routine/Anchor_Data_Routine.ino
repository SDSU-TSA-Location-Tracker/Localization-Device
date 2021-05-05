#include <ArduinoBLE.h>

// I/O pins
const int INTERNAL_LED = 13;
const int EXTERNAL_LED = 2;
const int PUSH_BUTTON = 3;
const int SWITCH_MODE_A = 10;
const int SWITCH_MODE_B = 9;
const int SWITCH_MODE_C = 8;

volatile char ButtonMode;

// Uncomment only the name of the anchor node the code is being flashed to
volatile char currentLocation[3] = {"A0"};
char anchorName[9] = {"AN-A0-NA"};
//volatile char currentLocation[3] = {"A4"};
//char anchorName[9] = {"AN-A4-NA"};
//volatile char currentLocation[3] = {"E0"};
//char anchorName[9] = {"AN-E0-NA"};
//volatile char currentLocation[3] = {"E4"};
//char anchorName[9] = {"AN-E4-NA"};


volatile byte locationIndex;

// Variables will change:
int ledState = HIGH;         // the current state of the output pin
int buttonState;             // the current reading from the input pin
int lastReadingState = LOW;   // the previous reading from the input pin

int delayTime = 250;
unsigned long currentTime = 0;

// the following variables are unsigned longs because the time, measured in
// milliseconds, will quickly become a bigger number than can be stored in an int.
long unsigned int lastDebounceTime = 0;  // the last time the output pin was toggled
long unsigned int debounceDelay = 40;    // the debounce time; increase if the output flickers

char locationArray[25][3] = {"A0", "A1", "A2", "A3", "A4",
                             "B0", "B1", "B2", "B3", "B4",
                             "C0", "C1", "C2", "C3", "C4",
                             "D0", "D1", "D2", "D3", "D4",
                             "E0", "E1", "E2", "E3", "E4"
                            };

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
  switch (ButtonMode) {
    case 'A':
      locationIndex++;
      locationIndex %= 25;
      currentLocation[0] = locationArray[locationIndex][0];
      currentLocation[1] = locationArray[locationIndex][1];
      Serial.print("Current Location: ");
      Serial.print(currentLocation[0]);
      Serial.print(currentLocation[1]);
      Serial.print("\n");
      break;
    case 'B':
      digitalWrite(EXTERNAL_LED, HIGH);
      currentTime = millis();
      while (millis() < currentTime + 500);
      digitalWrite(EXTERNAL_LED, LOW);
      break;
    case 'C':
      BLE.stopAdvertise();
      anchorName[3] = currentLocation[0];
      anchorName[4] = currentLocation[1];
      BLEroutine();
      break;
    default:
      Serial.print("Unsure of Mode Selected\n");
      break;
  }
}

// begins the BLE protocol and advertises just the name of the device
void BLEroutine(void)
{
  BLE.setLocalName(anchorName);
  BLE.advertise();
  Serial.print("Advertising Name:");
  Serial.print(anchorName);
  Serial.print("\n");
}

void setup() {
  Serial.begin(9600);

  if (!BLE.begin())
  {
    Serial.println("Starting BLE failed");
    while (1);
  }
  // Initialize all pin modes
  pinMode(EXTERNAL_LED, OUTPUT);
  pinMode(PUSH_BUTTON, INPUT);
  pinMode(SWITCH_MODE_A, INPUT);
  pinMode(SWITCH_MODE_B, INPUT);
  pinMode(SWITCH_MODE_C, INPUT);

  ButtonMode = 'C'; // default to single location increment mode

  BLEroutine();
}

void loop() {
  lastReadingState = debounce(PUSH_BUTTON, lastReadingState);
  if (digitalRead(SWITCH_MODE_A) == HIGH && ButtonMode != 'A')
  {
    Serial.println("Reset Location Service State");
    ButtonMode = 'A';
  }

  if (digitalRead(SWITCH_MODE_B) == HIGH && ButtonMode != 'B')
  {
    Serial.println("Battery Test State");
    ButtonMode = 'B';
  }
  if (digitalRead(SWITCH_MODE_C) == HIGH && ButtonMode != 'C')
  {
    Serial.println("Cycle Row State");
    ButtonMode = 'C';
  }
  currentTime = millis();
  while (millis() < currentTime + delayTime); // delay without interrupt conflict
}
