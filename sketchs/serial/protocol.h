#ifndef SERIAL_PROTOCOL_H_
#define SERIAL_PROTOCOL_H_

#include <Arduino.h>
#include <HardwareSerial.h>
#include "protocol_defines.h"


class ProtocolBuilder {
public:
  const byte *ToBytes(const protocol::Data &data) {
    memcpy(bytes_ + PROTOCOL_OFFSETS_COMMAND, &data.command, PROTOCOL_LIMITS_COMMAND_LENGTH);
    memcpy(bytes_ + PROTOCOL_OFFSETS_PAYLOAD_MOTOR_ID, &data.payload.motor.id, PROTOCOL_LIMITS_PAYLOAD_MOTOR_ID_LENGTH);
    memcpy(bytes_ + PROTOCOL_OFFSETS_PAYLOAD_MOTOR_POSITION_X, &data.payload.motor.position.x, PROTOCOL_LIMITS_PAYLOAD_MOTOR_POSITION_X_LENGTH);
    memcpy(bytes_ + PROTOCOL_OFFSETS_PAYLOAD_MOTOR_POSITION_Y, &data.payload.motor.position.y, PROTOCOL_LIMITS_PAYLOAD_MOTOR_POSITION_Y_LENGTH);
    return bytes_;
  }

  const protocol::Data &FromBytes(const byte *bytes, size_t message_length) {
    memset(&data_, 0, sizeof(data_));
    if (message_length < PROTOCOL_LIMITS_PACKET_LENGTH) {
      data_.command = PROTOCOL_COMMAND_ERROR;
      return data_;
    }

    memcpy(&data_.command, bytes + PROTOCOL_OFFSETS_COMMAND, PROTOCOL_LIMITS_COMMAND_LENGTH);
    byte command = bytes[PROTOCOL_OFFSETS_COMMAND];
    if (command == PROTOCOL_COMMAND_SET_POSITION_TO) {
      memcpy(&data_.payload.motor.id, bytes + PROTOCOL_OFFSETS_PAYLOAD_MOTOR_ID, PROTOCOL_LIMITS_PAYLOAD_MOTOR_ID_LENGTH);
      memcpy(&data_.payload.motor.position.x, bytes + PROTOCOL_OFFSETS_PAYLOAD_MOTOR_POSITION_X, PROTOCOL_LIMITS_PAYLOAD_MOTOR_POSITION_X_LENGTH);
      memcpy(&data_.payload.motor.position.x, bytes + PROTOCOL_OFFSETS_PAYLOAD_MOTOR_POSITION_Y, PROTOCOL_LIMITS_PAYLOAD_MOTOR_POSITION_Y_LENGTH);
    } else if (command == PROTOCOL_COMMAND_MESSAGE || command == PROTOCOL_COMMAND_ECHO) {
      memcpy(data_.payload.message, bytes + PROTOCOL_OFFSETS_PAYLOAD_MESSAGE, PROTOCOL_LIMITS_PAYLOAD_MESSAGE_LENGTH);
    } else if (command == PROTOCOL_COMMAND_MOVE) {
    } else {
      data_.command = PROTOCOL_COMMAND_ERROR;
    }

    return data_;
  }

  const byte *CreatePacket(char command, const protocol::Payload &payload) {
    memset(bytes_, 0, PROTOCOL_LIMITS_PACKET_LENGTH);
    bytes_[PROTOCOL_OFFSETS_COMMAND] = command;

    if (command == PROTOCOL_COMMAND_MESSAGE) {
      memcpy(bytes_ + PROTOCOL_OFFSETS_PAYLOAD_MESSAGE, payload.message, PROTOCOL_LIMITS_PAYLOAD_MESSAGE_LENGTH);
    }

    return bytes_;
  }

private:
  byte bytes_[PROTOCOL_LIMITS_PACKET_LENGTH];
  protocol::Data data_;
};

#endif  // SERIAL_PROTOCOL_H_
