#ifndef USART_CLASS_BUFFER_SIZE
#define USART_CLASS_BUFFER_SIZE 128
#endif  // USART_CLASS_BUFFER_SIZE

#ifndef USART_CLASS_ACTION_BUFFER_SIZE
#define USART_CLASS_ACTION_BUFFER_SIZE 8
#endif  // USART_CLASS_ACTION_BUFFER_SIZE

#ifndef TIMER_CLASS_ACTION_BUFFER_SIZE
#define TIMER_CLASS_ACTION_BUFFER_SIZE 8
#endif  // TIMER_CLASS_ACTION_BUFFER_SIZE

#ifndef F_CPU
#define F_CPU 16000000
#endif  // F_CPU

#include "USART/initializer.h"
#include "USART/transmitter.h"
#include "USART/receiver.h"

void ReceiveHandler(const char* message, int size) {
  USART::Transmitter::GetInstance().Puts(message, size);
}

void init() {
  USART::Initializer::Initialize(57600);
  USART::Receiver::GetInstance().Subscribe(ReceiveHandler);
  Timer1::GetInstance().Init();
  Timer1::GetInstance().SetPeriodLength(200);
  PORTD |= 1u << PD7;
}

void loop() {
}

int main() {
  init();
  for (;;) loop();
}
