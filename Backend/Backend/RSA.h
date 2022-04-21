#pragma once

// Includes:
#include <math.h>
#include <sstream>
#include <iostream>
#include <stdexcept>
#include <stdio.h>
#include <string>

// Defines:
#define INITIAL_E 3

// Constants: //TODO: CHANGE TO BIG NUMBERS
const double q = 13;
const double p = 11;

// Using:
using std::string;

// RSA Class:
class RSA
{
public:
	// Security Functions:
	static string encrypt(const string& message);
	static string decrypt(const string& cipher);
};

