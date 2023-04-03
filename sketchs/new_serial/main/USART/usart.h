#pragma once

#ifndef F_CPU
#error F_CPU define require
#endif

/*avr libraries*/
#include "avr/io.h"
#include "avr/interrupt.h"

/*avr standart libraries*/
#include "stdint.h"
#include "string.h"

/*my libraries*/
#include "../INTERRUPT/interrupt.h"
#include "../TIMER/timer.h"
#include "calculator.h"

class USART0 {
};
