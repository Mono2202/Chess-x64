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

