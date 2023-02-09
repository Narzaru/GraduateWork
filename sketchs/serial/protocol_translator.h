/// The Protocol Translation class contains a method for
/// translating an array of bytes into a protocol data structure.

#ifndef SERIAL_PROTOCOL_H_
#define SERIAL_PROTOCOL_H_

#include <Arduino.h>
#include <HardwareSerial.h>

#include "protocol_defines.h"

namespace protocol {
class ProtocolTranslator {
 public:
  const byte *ToBytes(const protocol::Data &data) {
    memset(bytes_, 0, sizeof(bytes_));
    bytes_[PROTOCOL_OFFSET_COMMAND] = data.Command;

    if (data.Command == PROTOCOL_COMMAND_ECHO) {
      memcpy(bytes_, &data.Payload.Message, PROTOCOL_SIZE_MESSAGE);
    } else if (data.Command == PROTOCOL_COMMAND_MOVE) {
      memcpy(bytes_, &data.Payload.Points.First.Position.X, PROTOCOL_SIZE_FIRST_POINT_X);
      memcpy(bytes_, &data.Payload.Points.First.Position.Y, PROTOCOL_SIZE_FIRST_POINT_Y);
      memcpy(bytes_, &data.Payload.Points.Second.Position.X, PROTOCOL_SIZE_SECOND_POINT_X);
      memcpy(bytes_, &data.Payload.Points.Second.Position.Y, PROTOCOL_SIZE_SECOND_POINT_Y);
    }
    return bytes_;
  }

  const protocol::Data &FromBytes(const byte *bytes, ptrdiff_t message_length) {
    memset(&data_, 0, sizeof(data_));
    // Error handling
    if (message_length != PROTOCOL_SIZE_PACKET) {
      data_.Command = PROTOCOL_COMMAND_MESSAGE_ERROR;
      return data_;
    }

    // Fill data from byte array
    memcpy(&data_.Command, bytes + PROTOCOL_OFFSET_COMMAND, PROTOCOL_SIZE_COMMAND);
    byte command = bytes[PROTOCOL_OFFSET_COMMAND];

    if (command == PROTOCOL_COMMAND_ECHO || command == PROTOCOL_COMMAND_MESSAGE) {
      memcpy(&data_.Payload.Message, bytes + PROTOCOL_OFFSET_PAYLOAD_MESSAGE, PROTOCOL_SIZE_MESSAGE);
    }

    if (command == PROTOCOL_COMMAND_SET_POINTS) {
      memcpy(&data_.Payload.Points.First.Position.X, bytes + PROTOCOL_OFFSET_PAYLOAD_POINTS_FIRST_POSITION_X, PROTOCOL_SIZE_FIRST_POINT_X);
      memcpy(&data_.Payload.Points.First.Position.Y, bytes + PROTOCOL_OFFSET_PAYLOAD_POINTS_FIRST_POSITION_Y, PROTOCOL_SIZE_FIRST_POINT_Y);
      memcpy(&data_.Payload.Points.Second.Position.X, bytes + PROTOCOL_OFFSET_PAYLOAD_POINTS_SECOND_POSITION_X, PROTOCOL_SIZE_SECOND_POINT_X);
      memcpy(&data_.Payload.Points.Second.Position.Y, bytes + PROTOCOL_OFFSET_PAYLOAD_POINTS_SECOND_POSITION_Y, PROTOCOL_SIZE_SECOND_POINT_Y);
    }

    return data_;
  }

  const byte *CreatePacket(char command, const protocol::ProtocolPayload &payload) {
    memset(bytes_, 0, PROTOCOL_SIZE_PACKET);
    bytes_[PROTOCOL_OFFSET_COMMAND] = command;

    if (command == PROTOCOL_COMMAND_MESSAGE || command == PROTOCOL_COMMAND_ECHO) {
      memcpy(bytes_ + PROTOCOL_OFFSET_PAYLOAD_MESSAGE, payload.Message, PROTOCOL_SIZE_MESSAGE);
    }

    return bytes_;
  }

 private:
  byte bytes_[PROTOCOL_SIZE_PACKET];
  protocol::Data data_;
};
} // namespace protocol

#endif  // SERIAL_PROTOCOL_H_
