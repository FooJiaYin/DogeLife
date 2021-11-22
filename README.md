# Networking API
Reference: https://mirror-networking.gitbook.io/docs/

## Setup
Reference: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
```CSharp
Using ...;
Using ...;
Using Mirror;

// public class Example : MonoBehaviour
public class Example : NetworkBehaviour
```
## Player
- When a new client is connected to the network, a new player instance is spawned on all connected clients.
- There will be many player game objects in the scene.
- In `player.cs`
    ```CSharp
    public class player : NetworkBehaviour
    {
        void Example() {
            // Code applied to the local player and all other players
            if (isLocalPlayer) {
                // Code applied to the local player only
            }
        }
    }
    ```
- Since the player is now spawned dynamically, any game object reference (such as ScoreDisplay) should be assigned with code (not dragged from the scene)

## Spawning GameObject
Example: `SpawnFood() in GameManager.cs`
```CSharp
/* When we want to spawn a game object */

// Spawn only at the server
if (!isServer) return;

var newObject = Instantiate(prefab, transform.position, transform.rotation);

// Tell other clients to spawn the game object
NetworkServer.Spawn(newObject);

```

- Game object to be synced between all clients must be spawned **only at the server** and then synced to the clients
- Game object should spawned from prefab, not GameObject instance from the scene
  - There should not be any game object instance in the scene because the instance will be independent in each client (not synced)
- **Never** spawn game object as child to other game object, because `NetworkServer.Spawn()` takes its local transform as global transform.
  - If there is a need, we will have to write a custom spawn function