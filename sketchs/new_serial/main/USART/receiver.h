#pragma once

#include "../TIMER/timer.h"
#include "../USART/initializer.h"

#ifndef USART_CLASS_BUFFER_SIZE
#error usart buffer size is not defined. Use #define USART_CLASS_BUFFER_SIZE <buffer size>
#endif

#ifndef USART_CLASS_ACTION_BUFFER_SIZE
#error usart buffer size is not defined. Use #define USART_CLASS_ACTION_BUFFER_SIZE <buffer size>
#endif

/* action function pointer */
typedef void (*ActionReceiveComplete)(const char *message, int size);

/* on timer interrupt handler declaration */
void OnTimer();

namespace USART {

class Receiver {
public:
  void Subscribe(ActionReceiveComplete action) {
    action_buffer_[number_of_actions_] = action;
    ++number_of_actions_;
  }

  void ProcessUsartRXInterrupt() {
    Timer1::GetInstance().Start();
    char data = UDR0;
    int index = index_of_character_;
    buffer_[index] = data;
    ++index_of_character_;
  }

  void ProcessTimerInterrupt() {
    USART::Initializer::DisableReceive();
    for (int i = 0; i < number_of_actions_; ++i) {
      action_buffer_[i](buffer_, index_of_character_);
    }
    index_of_character_ = 0;
    USART::Initializer::EnableReceive();
  }

private:
  char buffer_[USART_CLASS_BUFFER_SIZE];
  int index_of_character_ = 0;

  ActionReceiveComplete action_buffer_[USART_CLASS_ACTION_BUFFER_SIZE];
  int number_of_actions_ = 0;

/* for static instance */
public:
  static Receiver& GetInstance() {
    if (instance_ == nullptr) {
      instance_ = new Receiver();
    }
    return *instance_;
  }

private:
  Receiver() {
    Timer1::GetInstance().Subscribe(OnTimer);
  }
  Receiver(const Receiver&);
  Receiver& operator=(Receiver&);

  inline static Receiver* instance_ = nullptr;
};

}

void OnTimer() {
  USART::Receiver::GetInstance().ProcessTimerInterrupt();
}

ISR(USART_RX_vect) {
  USART::Receiver::GetInstance().ProcessUsartRXInterrupt();
}
