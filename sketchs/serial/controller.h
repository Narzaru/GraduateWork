#ifndef SERIAL_CONTROLLER_H_
#define SERIAL_CONTROLLER_H_

#include <Arduino.h>

#include "serial_driver.h"
#include "protocol_defines.h"
#include "serial_protocol.h"

enum class State {
  kNone,
  kSetPoints,
  kMoveToPoints,
  kMoveToPointsInProgress
};

class Controller {
public:
  Controller(protocol::SerialDriver &driver) : driver_(driver), state_(State::kNone) {}

  // Некая точка входа в обработку данных, принятие решения, что надо делать.
  // Конкретная реализация дейсвтия управлению мотором и.д не должна производиться тут.
  // Тут только общение со внешним устройством.
  void ProcessData(protocol::Data *data) {
    command_data_ = *data;
    if (data->Command == PROTOCOL_COMMAND_ECHO) {
      EchoHandler(data);
    } else if (data->Command == PROTOCOL_COMMAND_SET_POINTS) {
      SetPointsHandler(data);
    } else if (data->Command == PROTOCOL_COMMAND_MESSAGE_ERROR) {
      MessageErrorHandler(data);
    } else if (data->Command == PROTOCOL_COMMAND_MOVE) {
      MoveToPointsHandler(data);
    }
  }

  void SetPointsCompleted(bool success, protocol::Data *data) {
    memset(&data->Payload, 0, sizeof(data->Payload));
    if (success) {
      memcpy(&data->Payload.Message, "POINTS SET", 10);
      data->Command = PROTOCOL_COMMAND_SET_POINTS_SUCCESS;
    } else {
      memcpy(&data->Payload.Message, "POINTS ERROR", 12);
      data->Command = PROTOCOL_COMMAND_SET_POINTS_FAILD;
    }
    driver_.WriteData(*data);
    state_ = State::kNone;
  }

  void MoveToPointsCompleted(bool success, protocol::Data *data) {
    memset(&data->Payload, 0, sizeof(data->Payload));
    if (success) {
      memcpy(&data->Payload.Message, "MOVED", 5);
      data->Command = PROTOCOL_COMMAND_MOVE_COMPLETED;
    } else {
      memcpy(&data->Payload.Message, "MOVE BLOCKED", 12);
      data->Command = PROTOCOL_COMMAND_MOVE_FAILD;
    }
    driver_.WriteData(*data);
    state_ = State::kNone;
  }

  void MoveToPointsInProgress() {
    state_ = State::kMoveToPointsInProgress;
  }

  bool IsHaveCommand() const {
    return !(state_ == State::kNone || state_ == State::kMoveToPointsInProgress);
  }
   
  const protocol::Data& get_data() const {
    return command_data_;
  }

  State get_command() const {
    return state_;
  }

private:
  // Методы для каждой команды
  void EchoHandler(protocol::Data *data) {
    driver_.WriteData(*data);
  }

  void SetPointsHandler(protocol::Data *data) {
    state_ = State::kSetPoints;
  }

  void MoveToPointsHandler(protocol::Data *data) {
    state_ = State::kMoveToPoints;
  }

  void MessageErrorHandler(protocol::Data *data) {
    data->Command = PROTOCOL_COMMAND_MESSAGE_ERROR;
    driver_.WriteData(*data);
  }

  protocol::SerialDriver &driver_;
  protocol::Data command_data_;
  State state_;
};

#endif  // SERIAL_CONTROLLER_H_