#pragma once

#include "../INTERRUPT/interrupt.h"
#include "../USART/calculator.h"

namespace USART {

class Initializer {
public:
  inline static void Initialize(uint32_t baud_rate) {
    /* clear flag for a global interrupt */
    InterruptController::DisableGlobalInterrupt();
    /* disable a receive and transmit usart 0 interrupts */
    UCSR0B &= ~((1u << TXCIE0) | (1u << RXCIE0));
    /* set UBRR register for a baud rate */
    UBRR0 = USARTCalculator::ForAsynchronousNormalMode(F_CPU, baud_rate);
    /* enable transmit and receive */
    UCSR0B |= (1u << RXEN0) | (1u << TXEN0);
    /* set a 2 stop bit and a 8 bit data size */
    UCSR0C |= (1u << USBS0) | (3u << UCSZ00);
    /* enable a receive and transmit usart 0 interrupts */
    UCSR0B |= (1u << TXCIE0) | (1u << RXCIE0);
    /* enable global interrupts */
    InterruptController::EnableGlobalInterrupt();
  }

  inline static void EnableReceive() {
    UCSR0B |= 1u << RXEN0;
    UCSR0B |= 1u << RXCIE0;
  }

  inline static void DisableReceive() {
    UCSR0B &= ~(1u << RXEN0);
    UCSR0B &= ~(1u << RXCIE0);
  }

  inline static void EnableTransmit() {
    UCSR0B |= 1u << TXEN0;
    UCSR0B |= 1u << TXCIE0;
  }

  inline static void DisableTransmit() {
    UCSR0B &= ~(1u << TXEN0);
    UCSR0B &= ~(1u << TXCIE0);
  }
};

}