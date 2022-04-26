#pragma once

// Includes:
#include <math.h>
#include <sstream>
#include <iostream>
#include <stdexcept>
#include <stdio.h>
#include <string>
#include <vector>
#include <bitset>
#include <sstream>

// Defines:
#define SIZE 4096

// Using:
using std::string;
using std::vector;

// Typedef:
typedef vector<unsigned char> Buffer;

// AES Class:
class AES
{
public:
	// Security Functions:
	static Buffer encrypt(Buffer& message);
	static Buffer decrypt(const Buffer& cipher);

private:
	// Helper Functions:
	static string textToBinaryString(string& data);
	static string binaryStringToText(string& binaryString);
};
