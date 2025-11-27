#include <stdio.h>

#include "common.h"
#include "compiler.h"
#include "scanner.h"

void compile(const char* source) {
    initScanner(source);
    int line = -1;

    while (true) {
        Token token = scanToken();
        if (token.line != line) {
            printf("%4d", token.line);
            line = token.line;
        }
        else {
            printf("   | ");
        }

        // Usually, you set the output precision—the number of characters to
        // show—by placing a number inside the format string. Using * instead
        // lets you pass the precision as an argument. So that printf() call
        // prints the first token.length characters of the string at
        // token.start.
        printf("%2d '%.*s'\n", token.type, token.length, token.start);

        if (token.type == TOKEN_EOF) break;
    }
}