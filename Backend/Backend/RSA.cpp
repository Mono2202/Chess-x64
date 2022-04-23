#include "RSA.h"

// Security Functions:

/*
Encrypting the message using RSA
Input : message - the message to encrypt
Output: result  - the encrypted message
*/
string RSA::encrypt(const string& message)
{
	// Inits:
	char buffer[SIZE];
	string cmd = "python ./RSA.py e " + message;
	std::string result = "";
	
	// Opening the python script:
	FILE* pipe = _popen(cmd.c_str(), "r");

	// Reading from the cmd:
	while (fgets(buffer, sizeof buffer, pipe) != NULL) {
		result += buffer;
	}

	// Closing the python script:
	_pclose(pipe);

	return result;
}

/*
Decrypting the message using RSA
Input : message - the message to decrypt
Output: result  - the decrypted message
*/
string RSA::decrypt(const string& cipher)
{
	// Inits:
	char buffer[SIZE];
	string cmd = "python ./RSA.py d " + cipher;
	std::string result = "";

	// Opening the python script:
	FILE* pipe = _popen(cmd.c_str(), "r");

	// Reading from the cmd:
	while (fgets(buffer, sizeof buffer, pipe) != NULL) {
		result += buffer;
	}

	// Closing the python script:
	_pclose(pipe);

	return result;
}
