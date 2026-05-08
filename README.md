## Team
Darcy Mazloum (student ID # 2545876), Felis MacLellan (student ID #2245053)

## Brief Description
This is a climbing 2D platformer. Going as high as possible is the goal. Reach a checkpoint and fall? You'll be safely brought back up. Collect items along the way to speed you up or increase your score.

## Two major systems
This game implemented a camera movement system, a slot based inventory system, and a checkpoint system.

## Event Approach
For the most part, this project uses C# Events. Some of the UI elements use Unity Events, but for the most part, its C# Events.
C# Events were chosen because events were generally connected through getting components in code and not through inspector drag and drop.

## Save System
Go into a level, press P, then press Save and Quit. The game will remember the previous checkpoint and bring you back up if the save data indicates that you has passed a checkpoint.

## Known issues
I feel that the esthetics are an issue even if there is no requirement for the assignment to look nice >.< Ok but seriously, items are not able to be to be entered into the save data. If you look at Assets/Scripts/Player/PlayerSaveData.cs you'll see that I have coded it to accept items but it doesn't work. I would need a bit of time to figure it out. In addition, I have not implemented remembering which items have been collected after having been collected and saved.

## Thank you
Thanks again for the wonderful class this semester!
