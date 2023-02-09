#include "serial_driver.h"
#include "controller.h"

protocol::SerialDriver serial_driver(Serial);
Controller controller(serial_driver);
bool is_data_proccessed;
protocol::Data data;

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
  Serial.write("Inited!");
}

//// макаронный код, пока нормального решения не придумано...
// флаг, чтобы раз за цикл заходить и по чуть-чуть двигать моторы
bool is_moving;
int i = 0;
void loop() {
  // если есть необработанная комманда
  if (!is_data_proccessed) {
    controller.ProcessData(&data);
    is_data_proccessed = true;
  }
  // если есть необработнная комманда для мк
  if (controller.IsHaveCommand()) {
    if (controller.get_command() == State::kSetPoints) {
      auto data = controller.get_data();
      // validate points
      // if points valid - true, else - false
      controller.SetPointsCompleted(false, &data);
    } else if (controller.get_command() == State::kMoveToPoints) {
      is_moving = true;
      controller.MoveToPointsInProgress();
    }
  }
  if (is_moving) {
    delay(1000);
    ++i;
    if (i >= 5) {
      i = 0;
      is_moving = false;
      controller.MoveToPointsCompleted(true, &data);
    }
  }
}
