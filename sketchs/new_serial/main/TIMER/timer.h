#pragma once

#include <stdint.h>

#include "timer_calculator.h"
#include "../INTERRUPT/interrupt.h"

#ifndef TIMER_CLASS_ACTION_BUFFER_SIZE
#error usart buffer size is not defined. Use #define TIMER_CLASS_ACTION_BUFFER_SIZE <buffer size>
#endif

typedef void (*ActionTimer)(void);

class Timer1 {
public:
  void Init() {
    /* disable global interrupt */
    InterruptController::DisableGlobalInterrupt();
    /* set CTC mode */
    TCCR1B |= 1u << WGM12;
    /* enable global interrupt */
    InterruptController::EnableGlobalInterrupt();
  }

  void SetPeriodLength(int msec) {
    /* compare value */
    OCR1A = TimerCalculator::OCRFromTime(F_CPU, 1024, msec);
  }

  void Start() {
    /* start timer with a  1024 prescaler */
    TCCR1B |= (5u << CS10);
    ClearTimer();
    EnableCTCInterrupt();
  }

  void Stop() {
    /* stop timer */
    TCCR1B &= ~(5u << CS10);
    DisableCTCINterrupt();
  }

  void Subscribe(ActionTimer action) {
    actions_[number_of_actions_] = action;
    ++number_of_actions_;
  }

  /* not for user use */
  void EnableCTCInterrupt() {
    /* enable CTC interrupt */
    TIMSK1 |= (1u << OCIE1A);
  }

  void DisableCTCINterrupt() {
    /* disable CTC interrupt */
    TIMSK1 &= ~(1u << OCIE1A);
    /* clear interrupt flag */
    TIFR1 |= 1 << OCF1A;
  }

  inline void ClearTimer() {
    /* clear interrupt flag */
    TIFR1 |= 1 << OCF1A;
    /* clear counter register */
    TCNT1 = 0;
  }

  inline void OnTimerInterrupt() {
    for (int i = 0; i < number_of_actions_; ++i) {
      actions_[i]();
    }
  }

private:
  ActionTimer actions_[TIMER_CLASS_ACTION_BUFFER_SIZE];
  int number_of_actions_ = 0;

/* for static instance */
public:
  static Timer1& GetInstance() {
    if (instance_ == nullptr) {
      instance_ = new Timer1();
    }
    return *instance_;
  }

private:
  Timer1() {}
  Timer1(const Timer1&);
  Timer1& operator=(Timer1&);

  inline static Timer1* instance_ = nullptr;
};

ISR(TIMER1_COMPA_vect) {
  Timer1::GetInstance().Stop();
  Timer1::GetInstance().OnTimerInterrupt();
}
