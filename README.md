# Tag Area

This repository contains the source code for a Unity-based multiplayer game utilizing **Unity Netcode for GameObjects**, **Unity Relay**, **Unity Lobby** to provide a seamless online multiplayer experience.

## Requirements

- **Unity 6000.0.28f1**.
- **Netcode for GameObjects** package.
- **Unity Relay** package.
- **Unity Lobby** package.
- **Unity Account** and **Project setup** on the Unity Dashboard.

## How to run

To get started, clone this repository and open it in Unity:

```bash
git clone https://github.com/reed-jim/Tag-Area.git
```

1. **Host (Create a Lobby):**
   - One player should click the **Create** button to create a new lobby.
   - Once the lobby is created successfully, the screen will automatically switch to the **Lobby Room**.

2. **Joining the Lobby:**
   - Other players should click the **Refresh** button to load the list of available lobbies.
   - From the list, choose an available lobby and join.

3. **Start the Game:**
   - The host should wait for all players to connect to the server.
   - Once everyone is connected, the host can click **Start Game** to start the game.

## Multiplayer

This project demonstrates how to set up a multiplayer game in Unity using **Netcode for GameObjects**, **Unity Relay**, **Unity Lobby**. Players can create lobbies, join existing ones, and connect to each other using the **Relay** service for reliable peer-to-peer communication.

- **Netcode for GameObjects**: Handles network synchronization and client-server architecture.
- **Unity Relay**: Facilitates secure peer-to-peer connections through Unity’s Relay service.
- **Unity Lobby**: Provides functionality for creating, listing, and joining lobbies.
