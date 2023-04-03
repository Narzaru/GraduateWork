#pragma once

#include <stdint.h>

#include "../USART/initializer.h"

#ifndef USART_CLASS_BUFFER_SIZE
#error usart buffer size is not defined. Use #define USART_CLASS_BUFFER_SIZE <buffer size>
#endif

#ifndef USART_CLASS_ACTION_BUFFER_SIZE
#error usart buffer size is not defined. Use #define USART_CLASS_ACTION_BUFFER_SIZE <buffer size>
#endif

typedef void(*ActionTransmitComplete)(void);

namespace USART {

class Transmitter {
public:
  void Puts(const char* data_pointer, int data_length) {
    strncpy(buffer_, data_pointer, data_length);

    length_of_message_ = data_length;
    index_of_character_ = 0;

    StartTransmit();
  }

  void Putc(char data) {
    buffer_[0] = data;

    length_of_message_ = 1;
    index_of_character_ = 0;
  
    StartTransmit();
  }

  /* not for user use */
  void StartTransmit() {
    is_in_progress_ = true;
    Initializer::EnableTransmit();
    ProcessUsartTXInterrupt();
  }

  void ProcessUsartTXInterrupt() {
    ++index_of_character_;
    if (index_of_character_ > length_of_message_) {
      Initializer::DisableTransmit();
      length_of_message_ = 0;
      index_of_character_ = 0;
      is_in_progress_ = false;
      return;
    }
    UDR0 = buffer_[index_of_character_ - 1];
  }

  bool IsTransmissionInProgress() {
    return is_in_progress_;
  }

private:
  char buffer_[USART_CLASS_BUFFER_SIZE];
  int index_of_character_ = 0;
  int length_of_message_ = 0;
  bool is_in_progress_ = false;

  ActionTransmitComplete action_buffer_[USART_CLASS_ACTION_BUFFER_SIZE];
  int number_of_actions_ = 0;
 
/* for static instance */
public:
  static Transmitter& GetInstance() {
    if (instance_ == nullptr) {
      instance_ = new Transmitter();
    }
    return *instance_;
  }

private:
  Transmitter() {}
  Transmitter(const Transmitter&);
  Transmitter& operator=(Transmitter&);

  inline static Transmitter* instance_ = nullptr;
};

}

ISR(USART_TX_vect) {
  USART::Transmitter::GetInstance().ProcessUsartTXInterrupt();
}
