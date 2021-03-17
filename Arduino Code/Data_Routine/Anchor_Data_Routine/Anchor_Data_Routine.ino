#include <ArduinoBLE.h>

// I/O pins
const int INTERNAL_LED = 13;
const int EXTERNAL_LED = 2;
const int PUSH_BUTTON = 3;
const int SWITCH_MODE_A = 12;
const int SWITCH_MODE_B = 11;
const int SWITCH_MODE_C = 10;

volatile char ButtonMode;
volatile char currentLocation[3] = {"A0"};
char anchorName[9] = {"AN-A0-NA"};
volatile byte locationIndex;

// Variables will change:
int ledState = HIGH;         // the current state of the output pin
int buttonState;             // the current reading from the input pin
int lastReadingState = LOW;   // the previous reading from the input pin


// the following variables are unsigned longs because the time, measured in
// milliseconds, will quickly become a bigger number than can be stored in an int.
long unsigned int lastDebounceTime = 0;  // the last time the output pin was toggled
long unsigned int debounceDelay = 50;    // the debounce time; increase if the output flickers

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

      if (buttonState == HIGH && ButtonMode == 'A')
      {
        buttonAction();
        digitalWrite(INTERNAL_LED, HIGH);
        digitalWrite(EXTERNAL_LED, LOW);
      }
      else if (buttonState == HIGH && ButtonMode == 'B')
      {
        buttonAction();
        digitalWrite(EXTERNAL_LED, HIGH);
        digitalWrite(INTERNAL_LED, HIGH);
      }
      else if (buttonState == HIGH && ButtonMode == 'C')
      {
        anchorName[3] = 'E';
        anchorName[4] = '4';
        buttonAction();
        digitalWrite(EXTERNAL_LED, LOW);
        digitalWrite(INTERNAL_LED, HIGH);
      }
      else if (buttonState == LOW)
      {
        digitalWrite(EXTERNAL_LED, LOW);
        digitalWrite(INTERNAL_LED, LOW);
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
      locationIndex = locationIndex + 5;
      locationIndex %= 25;
      currentLocation[0] = locationArray[locationIndex][0];
      currentLocation[1] = locationArray[locationIndex][1];
      Serial.print("Current Location: ");
      Serial.print(currentLocation[0]);
      Serial.print(currentLocation[1]);
      Serial.print("\n");
      break;
    case 'C':
      BLE.stopAdvertise();
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
  if(BLE.advertise())
  {
    Serial.print("Advertising Name:");
    Serial.print(anchorName);
    Serial.print("\n");
  }
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

  ButtonMode = 'A'; // default to single location increment mode

  attachInterrupt(digitalPinToInterrupt(SWITCH_MODE_A), cycleRowState, RISING);
  attachInterrupt(digitalPinToInterrupt(SWITCH_MODE_B), cycleColState, RISING);
  attachInterrupt(digitalPinToInterrupt(SWITCH_MODE_C), resetLocState, RISING);

  BLEroutine();
}

void loop() {
  lastReadingState = debounce(PUSH_BUTTON, lastReadingState);
}

void cycleRowState() {
  ButtonMode = 'A';
}

void cycleColState() {
  ButtonMode = 'B';
}

void resetLocState() {
  ButtonMode = 'C';
}