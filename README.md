<h1 align="center"> Chess x64 ♟️</h1>

<p align="center"> 
  <img src="https://user-images.githubusercontent.com/69368659/166502144-59a33aca-6eea-4890-9f75-385fd6f3441c.png" alt="Chess x64 Logo" height="300" width="300">
</p>

<br></br>



<!-- TABLE OF CONTENTS -->
<h2 id="table-of-contents"> 🐘 Table of Contents</h2>

<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project"> ➤ About The Project</a></li>
    <li><a href="#overview"> ➤ Overview</a></li>
    <li><a href="#built-with"> ➤ Built With</a></li>
    <li>
      <a href="#getting-started"> ➤ Getting Started</a>
      <ol>
      <li>
        <a href="#dependencies"> ➤ Dependencies</a>
      </li>
      <li>
        <a href="#installation"> ➤ Installation</a>
      </li>
      </ol>
    </li>
    <li>
      <a href="#usage"> ➤ Usage </a>
      <ol>
      <li>
        <a href="#login"> ➤ Login</a>
      </li>
      <li>
        <a href="#register"> ➤ Register</a>
      </li>
      <li>
        <a href="#menu"> ➤ Menu</a>
      </li>
      <li>
        <a href="#play"> ➤ Play</a>
      </li>
      <li>
        <a href="#private-room"> ➤ Private Room</a>
      </li>
      <li>
        <a href="#game-room"> ➤ Game Room</a>
      </li>
      <li>
        <a href="#other-scenes"> ➤ Other Scenes</a>
      </li>
      </ol>
    </li>
    <li><a href="#roadmap"> ➤ Roadmap</a></li>
    <li><a href="#references"> ➤ References</a></li>
    <li><a href="#credits"> ➤ Credits</a></li>
  </ol>
</details>



![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- ABOUT THE PROJECT -->
<h2 id="about-the-project"> 🐎 About The Project</h2>
<p align="justify"> 
  Chess is a board game played between two players, is an abstract strategy game and involves no hidden information. It is played on a square chessboard with 64 squares arranged in an eight-by-eight grid. At the start, each player (one controlling the white pieces, the other controlling the black pieces) controls sixteen pieces: one king, one queen, two rooks, two bishops, two knights, and eight pawns. The object of the game is to checkmate the opponent's king, whereby the king is under immediate attack (in "check") and there is no way for it to escape. There are also several ways a game can end in a draw.
</p>



![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- OVERVIEW -->
<h2 id="overview"> 🦅 Overview</h2>
<p align="justify">
    The project is a fullstack project, with a server and a client. A player can play with his friends and with random players, and even study chess lines in the board editor. The server is a multi-threaded server, and the communication between the clients and the server is encrypted with AES. Data stored in the SQL database is encrypted with RSA.
</p>



![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- BUILT WITH -->
<h2 id="built-with"> 🐫 Built With</h2>

Backend:
* [C++](https://isocpp.org/)
* [C](https://www.gnu.org/software/gnu-c-manual/)
* [Python](https://www.python.org/)
* [SQLite3](https://www.sqlite.org/index.html)
* [nlohmann/json](https://github.com/nlohmann/json)

Frontend:
* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
* [WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/?view=netdesktop-5.0)
* [Newtonsoft.Json](https://www.newtonsoft.com/json)


![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- GETTING STARTED -->
<h2 id="getting-started"> 🐤 Getting Started</h2>


<h3 id="dependencies"> Dependencies</h3>

* Unity 2020.3.23f1:
  ```sh
  install @https://unity3d.com/get-unity/download
  ```
* Visual Studio 2022 Preview:
  ```sh
  install @https://visualstudio.microsoft.com/vs/
  ```
* Python 3.9:
  ```sh
  install @https://www.python.org/downloads/
  ```
* pycrypto:
  ```py
  pip install pycrypto
  ```


<h3 id="installation"> Installation</h3>

1. Clone the repo:
  ```sh
  git clone https://github.com/Mono2202/Chess-x64.git
  ```
2. Change ```config.txt``` in ```Backend/Backend``` and in ```Frontend/Assets/StreamingAssets```:
  ```sh
  -ip:127.0.0.1
  -port:54321
  -listenerPort:12345
  ```
3. Open ```Backend/Backend/Backend.sln``` in Visual Studio 2022 Preview
4. Run the server
5. Open ```Frontend``` folder in Unity
6. Build the client by pressing ```CTRL + B```
7. From the ```Builds``` folder, start multiple Clients (```.exe``` file)



![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- USAGE -->
<h2 id="usage"> 🐁 Usage</h2>


<h3 id="login"> Login</h3>
<p align="justify">
    User should input his username and password. Username consists of 
    6 to 15 characters, only english letters, digits and an underline. Password
    consists of 8 to 20 characters, at least one sign, one capital letter, one 
    lowercase letter and one digit:
    <p align="center"> 
      <img src="https://user-images.githubusercontent.com/69368659/166525641-7c11f2bc-d642-442f-8462-415ddeef1bef.png" width="50%">
    </p>
</p>


<h3 id="register"> Register</h3>
<p align="justify">
    User should input new username, password and email. Email should be a valid 
    email address:
    <p align="center"> 
      <img src="https://user-images.githubusercontent.com/69368659/166527593-16e93895-1563-4a40-8217-5b06a25933de.png" width="50%">
    </p>
</p>


<h3 id="menu"> Menu</h3>
<p align="justify">
    User can access to other scenes:
    <p align="center"> 
      <img src="https://user-images.githubusercontent.com/69368659/166529956-43351533-b637-4c8c-ba49-718c42461e90.png" width="50%">
    </p>
</p>


<h3 id="play"> Play</h3>
<p align="justify">
    User can join a private room, create a private room or join a random room:
    <p align="center"> 
      <img src="https://user-images.githubusercontent.com/69368659/166531416-73d27a8a-e896-418b-89bd-41c54d9426f5.png" width="50%">
    </p>
</p>


<h3 id="private-room"> Private Room</h3>
<p align="justify">
    User can copy to his clipboard the room code, and wait for a friend to join 
    the room:
    <p align="center"> 
      <img src="https://user-images.githubusercontent.com/69368659/166533109-e716689a-42e7-496f-a99a-c32a8ae53a84.png" width="50%">
    </p>
</p>


<h3 id="game-room"> Game Room</h3>
<p align="justify">
    Chess game between 2 players occurs:
    <p align="center"> 
      <img src="https://user-images.githubusercontent.com/69368659/166534951-13f95fdb-5385-4219-a24e-655c2d2fd7d9.png" width="50%">
    </p>
</p>


<h3 id="other-scenes"> Other Scenes</h3>
<p align="justify">
    There are more scenes in the project, for example: the board editor scene, user statistics scene, 
    match history scene, search player scene etc.
</p>



![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- ROADMAP -->
<h2 id="roadmap"> 🦌 Roadmap</h2>

- [ ] Fix lag
- [ ] Improve board editor to show full engine lines
- [ ] Add chess board customization
- [ ] Ability for user to change engine depth
- [ ] Add option to play vs. AI @ different levels



![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- REFERENCES -->
<h2 id="references"> 🦉 References</h2>

* Wikipedia.org, "Unity (game engine)". [Online]:
https://en.wikipedia.org/wiki/Unity_(game_engine)

* Wikipedia.org, "Advanced Encryption Standard". [Online]:
https://en.wikipedia.org/wiki/Advanced_Encryption_Standard

* Wikipedia.org, "RSA (cryptosystem)". [Online]:
https://en.wikipedia.org/wiki/RSA_(cryptosystem)

* Wikipedia.org, "Chess". [Online]:
https://en.wikipedia.org/wiki/Chess



![-----------------------------------------------------](https://raw.githubusercontent.com/andreasbm/readme/master/assets/lines/rainbow.png)

<!-- CREDITS -->
<h2 id="credits"> 🐆 Credits</h2>
Ron Monosevich
<br></br>

[![GitHub Badge](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Mono2202)
[![LinkedIn Badge](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/ron-monosevich-214754220/)
