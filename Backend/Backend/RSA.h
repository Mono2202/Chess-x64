#pragma once

// Includes:
#include <math.h>
#include <sstream>
#include <iostream>
#include <stdexcept>
#include <stdio.h>
#include <string>

// Defines:
#define SIZE 4096

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

