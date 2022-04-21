# Imports:
from Crypto.PublicKey import RSA
from Crypto.Cipher import PKCS1_OAEP
import binascii, sys, pickle

# Constants:
KEY_LENGTH = 3072
MODE_ARGV = 1
MSG_ARGV = 2

def generate_keys():
    # Generating the RSA keys:
    key_pair = RSA.generate(KEY_LENGTH)

    # Writing the public key:
    with open("pub_key.pem", "wb") as file:
        file.write(key_pair.publickey().export_key('PEM'))

    # Writing the private key:
    with open("priv_key.pem", "wb") as file:
        file.write(key_pair.exportKey('PEM'))


def read_keys():
    # Reading the public key:
    with open("pub_key.pem", "rb") as file:
        public_key = RSA.import_key(file.read())

    # Reading the private key:
    with open("priv_key.pem", "rb") as file:
        private_key = RSA.import_key(file.read())

    return public_key, private_key


def encrypt(msg, public_key):
    # Encrypting the RSA:
    encryptor = PKCS1_OAEP.new(public_key)
    enc_msg = encryptor.encrypt(msg)
    return binascii.hexlify(enc_msg).decode()


def decrypt(enc_msg, private_key):
    # Decrypting the RSA:
    decryptor = PKCS1_OAEP.new(private_key)
    dec_msg = decryptor.decrypt(enc_msg)
    return dec_msg.decode()


def main():
    # Reading the RSA keys:
    public_key, private_key = read_keys()
    
    # Condition: RSA encrypt
    if (sys.argv[MODE_ARGV] == "e"):
        print(encrypt(sys.argv[MSG_ARGV].encode(), public_key), end="")
    
    # Condition: RSA decrypt
    elif (sys.argv[MODE_ARGV] == "d"):
        print(decrypt(binascii.unhexlify(sys.argv[MSG_ARGV]), private_key), end="")

if __name__ == "__main__":
    main()