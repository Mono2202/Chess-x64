#include "AES.h"

// Security Functions:

/*
Encrypting the message using AES
Input : message - the message to encrypt
Output: result  - the encrypted message
*/
Buffer AES::encrypt(Buffer& message)
{
	// Condition: get match history response
	if (message[0] == 117)
	{
		// Converting to binary:
		string data(message.begin(), message.end());
		string result = textToBinaryString(data);
		return Buffer(result.begin(), result.end());
	}

	// Converting " to ^:
	for (int i = 0; i < message.size(); i++) {
		if (message[i] == '\"') {
			message[i] = '^';
		}
	}

	// Inits:
	string data(message.begin(), message.end());
	char buffer[SIZE];
	string cmd = "AES.exe e \"" + data + "\"";
	string result = "";

	// Opening the python script:
	FILE* pipe = _popen(cmd.c_str(), "r");

	// Reading from the cmd:
	while (fgets(buffer, sizeof buffer, pipe) != NULL) {
		result += buffer;
	}

	// Closing the python script:
	_pclose(pipe);

	// Converting the text data to binary:
	result = textToBinaryString(result);

	return Buffer(result.begin(), result.end());
}

/*
Decrypting the message using AES
Input : message - the message to decrypt
Output: result  - the decrypted message
*/
Buffer AES::decrypt(const Buffer& cipher)
{

	// Inits:
	string data(cipher.begin(), cipher.end());
	data = binaryStringToText(data);
	char buffer[SIZE];
	string cmd = "AES.exe d \"" + data + "\"";
	string result = "";

	// Opening the python script:
	FILE* pipe = _popen(cmd.c_str(), "r");

	// Reading from the cmd:
	while (fgets(buffer, sizeof buffer, pipe) != NULL) {
		result += buffer;
	}

	// Closing the python script:
	_pclose(pipe);

	// Converting ^ to ":
	for (int i = 0; i < result.size(); i++) {
		if (result[i] == '^') {
			result[i] = '\"';
		}
	}

	return Buffer(result.begin(), result.end());
}


// Helper Methods:

/*
Converting text data to binary
Input : data		 - the data string
Output: binaryString - the binary data
*/
string AES::textToBinaryString(string& data)
{
	// Inits:
	string binaryString = "";

	// Converting the data to a binary string:
	for (char& _char : data) {
		binaryString += std::bitset<8>(_char).to_string();
	}

	return binaryString;
}

/*
Converting binary data to text
Input : binaryString - the data string
Output: text         - the text data
*/
string AES::binaryStringToText(string& binaryString)
{
	// Inits:
	string text = "";
	std::stringstream sstream(binaryString);

	// Converting the binary string to text:
	while (sstream.good())
	{
		std::bitset<8> bits;
		sstream >> bits;
		text += char(bits.to_ulong());
	}

	return text;
}
