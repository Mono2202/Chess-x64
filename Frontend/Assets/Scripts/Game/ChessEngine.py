#!/usr/bin/python

import chess, chess.engine, sys

ENGINE_PATH = r"c:\Users\User\Desktop\GIT\Chess-x64\Frontend\Assets\Scripts\Game\Stockfish.exe"
BOARD_ARGV = 2
TURN_ARGV = 3
CASTLE_ARGV = 4

def main():
    
    board = chess.Board(sys.argv[BOARD_ARGV] + " " + sys.argv[TURN_ARGV] + " " + sys.argv[CASTLE_ARGV] + " - 0 1")
    engine = chess.engine.SimpleEngine.popen_uci(ENGINE_PATH)
    suggested_move = engine.play(board, chess.engine.Limit(time=0.1))
    
    with open("SuggestedMove.txt", "w") as file:
        file.write(str(suggested_move.move))

if __name__ == "__main__":
    main()