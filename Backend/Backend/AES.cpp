#include "AES.h"

// Security Functions:

/*
Encrypting the message using AES
Input : message - the message to encrypt
Output: result  - the encrypted message
*/
Buffer AES::encrypt(Buffer& message)
{
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
