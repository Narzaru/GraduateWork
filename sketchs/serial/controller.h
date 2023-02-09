#ifndef SERIAL_CONTROLLER_H_
#define SERIAL_CONTROLLER_H_

#include <Arduino.h>

#include "serial_driver.h"
#include "protocol_defines.h"
#include "serial_protocol.h"

class Controller {
public:
  Controller(protocol::SerialDriver &driver) : driver_(driver) {}

  void ProcessData(protocol::Data *data) {
      if (data->Command == PROTOCOL_COMMAND_ECHO) {
        driver_.WriteData(*data);
      } else if (data->Command == PROTOCOL_COMMAND_SET_POINTS) {
        memset(&data->Payload, 0, sizeof(data->Payload));
        memcpy(&data->Payload.Message, "POINTS SET", 10);
        data->Command = PROTOCOL_COMMAND_SET_POINTS_SUCCESS;
        driver_.WriteData(*data);
      } else if (data->Command == PROTOCOL_COMMAND_MESSAGE_ERROR) {
        data->Command = PROTOCOL_COMMAND_MESSAGE_ERROR;
        driver_.WriteData(*data);
      }
  }

private:
  protocol::SerialDriver &driver_;
};

#endif  // SERIAL_CONTROLLER_H_