# Match-3-Line-Drawer
![Preview...](https://media.giphy.com/media/oX8T2okAqX26bLBz92/giphy.gif)

Made with Unity 2018.2.18f1.
The repository can be found in https://github.com/chulini/Match-3/

## Software Architecture
Similar to a Model/View/Controller pattern the game architecture is splited in State/View/Logic class types.
The communication between the different class types is using an system of events.

(The event system was made using this tool https://github.com/chulini/unity-quick-events)
### State Classes
State classes stores the current status of the game and sends events when relevant actions occurs.
This classes are controlled only by Logic classes.

* `GameState`: Contains the state if the game like the current score, selected blocks, etc.
* `BlockState`: Contains the state of a block. The current colorID, if is selected, etc.
* `BlockCoordinate`: A struct that stores a (x,y) coordinate in discrete space

### Logic Classes
Logic classes listen to the Views events and controls the State classes.
* `Board`: Listen to pointer events and implements the game logic updating the `GameState`

### View Classes
View classes are in charge of listening to the game events and update the screen/audio. 
Some view classes sends input events.
* `AudioEffects`: Listen to game events and trigger audio
* `Block`: Updates the layout if a block listening to `BlockState` events
* `BlockBgFallAnimation`: Animates a the color of a block when falling to a new block
* `BlockLineRenderer`: Listen to game events and draws a line between selected blocks
* `BlockPointerEvents`: Sends mouse input events on a block
* `UIManager`: Listen to game events and updates the User Interface.
* `UICanvasSwitcher`: To fade between canvases.
* `UIAnimatedText`: To refresh a text making an animation.

### Other Classes
* `Game`: The *Main* class that initializes the hole game.
* `ExtensionMethods`: Helper methods with math functions and shortcuts.

## Scenes
As `UIManager` only listen to the game events and updates the UI elements. All the UI screens are made on an independent scene. 
When game starts `Main` scene loads `UI` scene additively.
