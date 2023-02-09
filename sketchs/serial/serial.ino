#include "serial_driver.h"
#include "controller.h"

protocol::SerialDriver serial_driver(Serial);
Controller controller(serial_driver);
bool is_data_proccessed;
protocol::Data data;
protocol::PointsSet points;

void serialEvent() {
  data = serial_driver.ReadData();
  if (is_data_proccessed == false) {
    data.Command = PROTOCOL_COMMAND_COMMAND_IN_PROGRESS;
    memset(&data.Payload, 0, sizeof(data.Payload));
    serial_driver.WriteData(data);
  } else {
    is_data_proccessed = false;
  }
  Serial.flush();
}

void setup() {
  Serial.begin(115200);
  while (!Serial) {}
}

void loop() {
  if (!is_data_proccessed) {
    controller.ProcessData(&data);
  }
  is_data_proccessed = true;
}
