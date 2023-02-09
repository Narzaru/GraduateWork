#ifndef SERIAL_CONTROLLER_H_
#define SERIAL_CONTROLLER_H_

#include <Arduino.h>

#include "serial_driver.h"
#include "protocol_defines.h"
#include "serial_protocol.h"

class Controller {
public:
  Controller(protocol::SerialDriver &driver) : driver_(driver) {}

  // По задумке это место должно являться просто стейт машиной.
  // Некая точка входа в обработку данных.
  void ProcessData(protocol::Data *data) {
      if (data->Command == PROTOCOL_COMMAND_ECHO) {
        EchoHandler(data);
      } else if (data->Command == PROTOCOL_COMMAND_SET_POINTS) {
        SetPointsHandler(data);
      } else if (data->Command == PROTOCOL_COMMAND_MESSAGE_ERROR) {
        MessageErrorHandler(data);
      }
  }

private:
  // Методы для каждого случая
  void EchoHandler(protocol::Data *data) {
    driver_.WriteData(*data);
  }

  void SetPointsHandler(protocol::Data *data) {
    memset(&data->Payload, 0, sizeof(data->Payload));
    memcpy(&data->Payload.Message, "POINTS SET", 10);
    data->Command = PROTOCOL_COMMAND_SET_POINTS_SUCCESS;
    driver_.WriteData(*data);
  }

  void MessageErrorHandler(protocol::Data *data) {
    data->Command = PROTOCOL_COMMAND_MESSAGE_ERROR;
    driver_.WriteData(*data);
  }

  protocol::SerialDriver &driver_;
};

#endif  // SERIAL_CONTROLLER_H_