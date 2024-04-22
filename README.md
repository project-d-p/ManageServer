# Game Lobby Server

This repository contains the server-side implementation for a game lobby system, built with C# and .NET. It utilizes gRPC and Protocol Buffers for server communication and interacts with game engine clients using .NET's built-in API.

## Technology Stack

- **C# & .NET**: Core technologies used for server-side logic.
- **gRPC & Protocol Buffers**: Used for efficient, high-performance server-to-server communication.
- **.NET API**: Manages communication between the server and game engine clients.

## Purpose

The purpose of this server is to handle all necessary functionalities within the game lobby, including player matchmaking, game session management, and player statistics updates.

## Current Features

- **Player Matchmaking System**: Currently implementing a system to match players in the lobby based on various criteria such as skill levels and game preferences.

## How to Run

To run this project, you will need .NET SDK installed on your machine. After cloning the repository, navigate to the project directory and run the following commands:

```bash
dotnet build
dotnet run
