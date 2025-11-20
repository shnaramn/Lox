cmake_minimum_required(VERSION 3.20)
project(CLox)

file(GLOB clox_SRC
    "*.h"
    "*.c"
)

add_executable(CLox ${clox_SRC})
