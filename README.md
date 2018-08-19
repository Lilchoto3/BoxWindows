# BoxWindows, a visual engine for console applications written in C#

##### by Zachary Luck

BoxWindows is a visual engine for console applications written in C#, it works by first drawing a box, given specified parameters for X and Y values, relative to the console's display, as well as a height and width to set the size of the box, and then has optional parameters for colors for the foreground and background, as well as a string to give the box a name.

From there, you can write lines or print characters into the box you've drawn, for either text or visual aspects respectively. Drawn boxes' locations are saved and can be recalled at any time, previously drawn boxes can be redrawn, although you'll have to re-enter colors and names, and boxes can also be swapped between, printing lines to multiple box locations.

Text written into the boxes is also saved to a buffer, which removes the oldest line written when the buffer for the box fills up. The buffer is as big as the box is tall on the inside, and the buffer can also be saved and recalled for later, however, printed characters do not use the buffer, and will not be saved.

The engine also makes use of all 16 colors the command prompt allows for, and provides full XML documentation for ease of use within Visual Studio or other C# IDEs as well.