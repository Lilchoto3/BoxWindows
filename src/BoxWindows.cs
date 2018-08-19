using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoxWindowsLibrary
{
    /// <summary>
    /// BoxWindows Engine for C# console output, can draw and store a number of boxes to be printed to the console.
    /// <para>Has options for coloration of the boxes with up to 16 options for color, which follows a binary style (red = 1, green = 2, blue = 4, darken = 8).</para>
    /// <para>Has two styles of boxes, single (false) and double lined (true).</para>
    /// <para>Includes a built in database that has dimensions of boxes saved to it whenever a non-recalling version of DrawBox is called. </para>
    /// <para>Database does not hold data for the color, name, or text inside the box. </para>
    /// <para>Please make account for the dimensions of your console so as to not draw boxes outside of the console, which will throw an exception. </para>
    /// <para>Version 1.4</para>
    /// </summary>
    public class BoxWindows
    {
        char[] boxChar;
        int[,] savedBoxes;  //[index, val] val works as such; 0: x coord, 1: y coord, 2: width, 3: height
        string[][] savedBuffers; //[index][lines] index is not the same as savedBoxes, buffers are seperate from boxes
        int[][] savedForeColors; //[index][lines] same as above
        int[][] savedBackColors; //[index][lines] same as above
        int mod, textX, textY, textW, textH;
        int savedIndex;
        List<string> stringBuffer = new List<string>();
        List<int> foreColorBuffer = new List<int>();
        List<int> backColorBuffer = new List<int>();
        
        /// <summary>
        /// Clears text buffers.
        /// </summary>
        public void ClearBuffer ()
        {
            stringBuffer.Clear();
            foreColorBuffer.Clear();
            backColorBuffer.Clear();
        }

        /// <summary>
        /// Saves the current buffer to storage at the index. Use WriteLine to clear buffer.
        /// </summary>
        /// <param name="index">Index specified to save to (0-31)</param>
        public void SaveBuffer (int index)
        {
            savedBuffers[index] = new string[stringBuffer.Count];
            savedForeColors[index] = new int[foreColorBuffer.Count];
            savedBackColors[index] = new int[backColorBuffer.Count];
            for (int i = 0; i < stringBuffer.Count; i++)
            {
                savedBuffers[index][i] = stringBuffer[i];
                savedForeColors[index][i] = foreColorBuffer[i];
                savedBackColors[index][i] = backColorBuffer[i];
            }
        }

        /// <summary>
        /// Gets a buffer from the given index, retrieves all lines. Clears text buffers before loading new ones.
        /// </summary>
        /// <param name="index">The index to retrieve the saved buffer.</param>
        public void GetBuffer(int index)
        {
            stringBuffer.Clear();
            foreColorBuffer.Clear();
            backColorBuffer.Clear();

            for (int i = 0; i < savedBuffers[index].Length; i++)
            {
                stringBuffer.Insert(i, savedBuffers[index][i]);
                foreColorBuffer.Insert(i, savedForeColors[index][i]);
                backColorBuffer.Insert(i, savedBackColors[index][i]);
            }
        }

        /// <summary>
        /// Writes new lines to the buffer. Wraps words within the current box. Option to clear the buffer. 
        /// </summary>
        /// <param name="input">Text to be inputted.</param>
        /// <param name="clearBox">Specify whether or not to clear the buffer.</param>
        public void WriteLine(string input, bool clearBuffer)
        {
            if (clearBuffer)
            {
                stringBuffer.Clear();
                foreColorBuffer.Clear();
                backColorBuffer.Clear();
            }
            this.ClearBox();
            int breakpoint = 0;
            int linecount = stringBuffer.Count;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ' ')
                {
                    breakpoint = i;
                }
                if (i >= textW)
                {
                    stringBuffer.Insert(linecount, input.Substring(0, breakpoint));
                    foreColorBuffer.Insert(linecount, GetColor(false));
                    backColorBuffer.Insert(linecount, GetColor(true));
                    input = input.Substring(breakpoint + 1);
                    linecount++;
                    breakpoint = 0;
                    i = 0;
                }
                if (input[i] == '\n')
                {
                    stringBuffer.Insert(linecount, input.Substring(0, i));
                    foreColorBuffer.Insert(linecount, GetColor(false));
                    backColorBuffer.Insert(linecount, GetColor(true));
                    input = input.Substring(i + 1);
                    linecount++;
                    breakpoint = 0;
                    i = 0;
                }
                if (linecount == textH - 1)
                {
                    stringBuffer.RemoveAt(0);
                    foreColorBuffer.RemoveAt(0);
                    backColorBuffer.RemoveAt(0);
                    linecount--;
                }
                if (i == input.Length - 1)
                {
                    stringBuffer.Insert(linecount, input);
                    foreColorBuffer.Insert(linecount, GetColor(false));
                    backColorBuffer.Insert(linecount, GetColor(true));
                }
            }
            for (int i = 0; i < stringBuffer.Count; i++)
            {
                Console.SetCursorPosition(textX, textY + i);
                SetColor(foreColorBuffer[i], false);
                SetColor(backColorBuffer[i], true);
                Console.Write(stringBuffer[i]);
            }
            Console.SetCursorPosition(textX, textY + textH - 1);
        }

        /// <summary>
        /// Writes new lines to the buffer. Wraps words within the current box. Option to clear the buffer. 
        /// </summary>
        /// <param name="input">Text to be inputted.</param>
        /// <param name="clearBox">Specify whether or not to clear bot the text buffer and two color buffers.</param>
        /// <param name="color">Specifies the color to use for the foreground or background.</param>
        /// <param name="colorBackground">Specifies whether or not to color the background </param>
        public void WriteLine (string input, bool clearBuffer, int color, bool colorBackground)
        {
            if (clearBuffer)
            {
                stringBuffer.Clear();
                foreColorBuffer.Clear();
                backColorBuffer.Clear();
            }
            this.ClearBox();
            int breakpoint = 0;
            int linecount = stringBuffer.Count;
            for (int i=0;i<input.Length;i++)
            {
                if (input[i] == ' ')
                {
                    breakpoint = i;
                }
                if (i >= textW)
                {
                    stringBuffer.Insert(linecount, input.Substring(0, breakpoint));
                    if (colorBackground)
                    {
                        backColorBuffer.Insert(linecount, color);
                        foreColorBuffer.Insert(linecount, GetColor(false));
                    }
                    else
                    {
                        foreColorBuffer.Insert(linecount, color);
                        backColorBuffer.Insert(linecount, GetColor(true));
                    }
                    input = input.Substring(breakpoint + 1);
                    linecount++;
                    breakpoint = 0;
                    i = 0;
                }
                if (input[i] == '\n')
                {
                    stringBuffer.Insert(linecount, input.Substring(0, i));
                    if (colorBackground)
                    {
                        backColorBuffer.Insert(linecount, color);
                        foreColorBuffer.Insert(linecount, GetColor(false));
                    }
                    else
                    {
                        foreColorBuffer.Insert(linecount, color);
                        backColorBuffer.Insert(linecount, GetColor(true));
                    }
                    input = input.Substring(i + 1);
                    linecount++;
                    breakpoint = 0;
                    i = 0;
                }
                if (linecount == textH - 1)
                {
                    stringBuffer.RemoveAt(0);
                    foreColorBuffer.RemoveAt(0);
                    backColorBuffer.RemoveAt(0);
                    linecount--;
                }
                if (i == input.Length - 1)
                    stringBuffer.Insert(linecount, input);
                if (colorBackground)
                {
                    backColorBuffer.Insert(linecount, color);
                    foreColorBuffer.Insert(linecount, GetColor(false));
                }
                else
                {
                    foreColorBuffer.Insert(linecount, color);
                    backColorBuffer.Insert(linecount, GetColor(true));
                }
            }
            for (int i = 0; i < stringBuffer.Count; i++)
            {
                Console.SetCursorPosition(textX, textY + i);
                SetColor(foreColorBuffer[i], false);
                SetColor(backColorBuffer[i], true);
                Console.Write(stringBuffer[i]);
            }
            Console.SetCursorPosition(textX, textY + textH - 1);
        }
        
        /// <summary>
        /// Clears the box of the currently selected index of any text.
        /// </summary>
        public void ClearBox ()
        {
            for (int i=0;i<textH;i++)
            {
                for (int j=0;j<textW;j++)
                {
                    Console.SetCursorPosition(textX + j, textY + i);
                    Console.Write(" ");
                }
            }
        }

        /// <summary>
        /// Clears the box of the inputted index of any text.
        /// </summary>
        /// <param name="index">Index of the box to clear of text.</param>
        public void ClearBox(int index)
        {
            this.SetCurrentBox(index);
            for (int i = 0; i < textH; i++)
            {
                for (int j = 0; j < textW; j++)
                {
                    Console.SetCursorPosition(textX + j, textY + i);
                    Console.Write(" ");
                }
            }
        }

        /// <summary>
        /// Prints a single character inside of the current box, uses last used colors and local space. Clamps within box dimensions.
        /// </summary>
        /// <example>Top left corner inside box is (0,0)).</example>
        /// <param name="x">The X coordinate within the box, local space.</param>
        /// <param name="y">The Y coordinate within the box, local space.</param>
        /// <param name="c">The character to print.</param>
        public void PrintChar (int x, int y, char c)
        {
            if (x > textW)
                x = textW;
            else if (x < 0)
                x = 0;
            if (y > textH)
                y = textH;
            else if (y < 0)
                y = 0;
            Console.SetCursorPosition(textX + x, textY + y);
            Console.Write(c);
        }

        /// <summary>
        /// Prints a single character inside of the current box, changes the specified foreground or background color. Clamps within box dimensions.
        /// </summary>
        /// <example>Top left corner inside box is (0,0)).</example>
        /// <param name="x">The X coordinate within the box, local space.</param>
        /// <param name="y">The Y coordinate within the box, local space.</param>
        /// <param name="c">The character to print.</param>
        /// <param name="color">Specifies the color to use for the foreground or background.</param>
        /// <param name="colorBackground">Specifies whether or not to color the background </param>
        public void PrintChar(int x, int y, char c, int color, bool colorBackground)
        {
            SetColor(color, colorBackground);

            if (x > textW)
                x = textW;
            else if (x < 0)
                x = 0;
            if (y > textH)
                y = textH;
            else if (y < 0)
                y = 0;
            Console.SetCursorPosition(textX + x, textY + y);
            Console.Write(c);
        }

        /// <summary>
        /// Prints a single character inside of the current box, changes the foreground and background color. Clamps within box dimensions.
        /// </summary>
        /// <example>Top left corner inside box is (0,0)).</example>
        /// <param name="x">The X coordinate within the box, local space.</param>
        /// <param name="y">The Y coordinate within the box, local space.</param>
        /// <param name="c">The character to print.</param>
        /// <param name="foreColor">Specifies what color is used for the foreground color.</param>
        /// <param name="backColor">Specifies what color is used for the background color.</param>
        public void PrintChar(int x, int y, char c, int foreColor, int backColor)
        {
            SetColor(foreColor, false);
            SetColor(backColor, true);

            if (x > textW)
                x = textW;
            else if (x < 0)
                x = 0;
            if (y > textH)
                y = textH;
            else if (y < 0)
                y = 0;
            Console.SetCursorPosition(textX + x, textY + y);
            Console.Write(c);
        }

        /// <summary>
        /// Sets the current box to print text to without redrawing the box.
        /// </summary>
        /// <param name="index">The index of the saved box to grab dimensions from.</param>
        public void SetCurrentBox (int index)
        {
            textX = savedBoxes[index, 0] + 1;
            textY = savedBoxes[index, 1] + 1;
            textW = savedBoxes[index, 2] - 2;
            textH = savedBoxes[index, 3] - 2;
            Console.SetCursorPosition(textX, textY);
        }

        /// <summary>
        /// Gets the X or Y coordinate of a corner of the box at a specific index in storage.
        /// </summary>
        /// <param name="index">The index of the box to find the coordinates from.</param>
        /// <param name="corner">0: top-left, 1: top-right, 2: bottom-left, 3: bottom-right.</param>
        /// <param name="coord">false: get x coord, true: get y coord.</param>
        /// <returns>The X or Y coordinate of the corner of the box.</returns>
        public int GetCorner (int index, int corner, bool coord)
        {
            if (coord)
            {
                switch (corner)
                {
                    case 0:
                    case 1:
                        return savedBoxes[index, 1];
                    case 2:
                    case 3:
                        return savedBoxes[index, 1] + savedBoxes[index, 3]-1;
                    default:
                        break;
                }
            }
            else
            {
                switch (corner)
                {
                    case 0:
                    case 2:
                        return savedBoxes[index, 0];
                    case 1:
                    case 3:
                        return savedBoxes[index, 0] + savedBoxes[index, 2]-1;
                    default:
                        break;
                }
            }
            return -1;
        }

        /// <summary>
        /// Draws a box with the specified dimensions and style, using the last colors specified to color the box.
        /// </summary>
        /// <param name="width">Specifies the width of the whole box.</param>
        /// <param name="height">Specifies the height of the whole box.</param>
        /// <param name="x">Specifies the x-coordinate of the top-left corner of the box.</param>
        /// <param name="y">Specifies the y-coordinate of the top-left corner of the box.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        public void DrawBox(int width, int height, int x, int y, int style)
        {
            mod = style * 11;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.SetCursorPosition(x + j, y + i);
                    if (j == 0 && i == 0)
                    {
                        Console.Write(boxChar[2 + mod]);
                        continue;
                    }
                    else if (j == 0 && i == height - 1)
                    {
                        Console.Write(boxChar[4 + mod]);
                        continue;
                    }
                    else if (j == width - 1 && i == height - 1)
                    {
                        Console.Write(boxChar[5 + mod]);
                        continue;
                    }
                    else if (j == width - 1 && i == 0)
                    {
                        Console.Write(boxChar[3 + mod]);
                        continue;
                    }
                    else if (j == 0 || j == width - 1)
                    {
                        Console.Write(boxChar[1 + mod]);
                        continue;
                    }
                    else if (i == 0 || i == height - 1)
                    {
                        Console.Write(boxChar[0 + mod]);
                        continue;
                    }
                    else
                        Console.Write(" ");
                }
            }
            textX = x + 1;
            textY = y + 1;
            textW = width - 2;
            textH = height - 2;

            SaveCurrentBox(x, y, width, height);

        }

        /// <summary>
        /// Draws a box with the specified dimensions and style, using the last colors specified to color the box, and the given name to print at the top.
        /// </summary>
        /// <param name="width">Specifies the width of the whole box.</param>
        /// <param name="height">Specifies the height of the whole box.</param>
        /// <param name="x">Specifies the x-coordinate of the top-left corner of the box.</param>
        /// <param name="y">Specifies the y-coordinate of the top-left corner of the box.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="name">Specifies the name printed at the top left corner of the box.</param>
        public void DrawBox(int width, int height, int x, int y, int style, string name)
        {
            DrawBox(width, height, x, y, style);

            Console.SetCursorPosition(x + 1, y);
            Console.Write("[");
            for (int i = 0; i < width - 4; i++)
            {
                if (i == name.Length)
                    break;
                Console.Write(name[i]);
            }
            Console.Write("]");
        }

        /// <summary>
        /// Draws a box with the specified dimensions and style, changing the specifed foreground or background color.
        /// </summary>
        /// <param name="width">Specifies the width of the whole box.</param>
        /// <param name="height">Specifies the height of the whole box.</param>
        /// <param name="x">Specifies the x-coordinate of the top-left corner of the box.</param>
        /// <param name="y">Specifies the y-coordinate of the top-left corner of the box.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="color">Specifies the color to use for the foreground or background.</param>
        /// <param name="colorBackground">Specifies whether or not to color the background </param>
        public void DrawBox(int width, int height, int x, int y, int style, int color, bool colorBackground)
        {
            SetColor(color, colorBackground);

            DrawBox(width, height, x, y, style);
        }

        /// <summary>
        /// Draws a box with the specified dimensions and style, changing the specifed foreground or background color, and prints the given name to print at the top.
        /// </summary>
        /// <param name="width">Specifies the width of the whole box.</param>
        /// <param name="height">Specifies the height of the whole box.</param>
        /// <param name="x">Specifies the x-coordinate of the top-left corner of the box.</param>
        /// <param name="y">Specifies the y-coordinate of the top-left corner of the box.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="color">Specifies the color to use for the foreground or background.</param>
        /// <param name="colorBackground">Specifies whether or not to color the background </param>
        /// <param name="name">Specifies the name printed at the top left corner of the box.</param>
        public void DrawBox(int width, int height, int x, int y, int style, int color, bool colorBackground, string name)
        {
            SetColor(color, colorBackground);

            DrawBox(width, height, x, y, style);

            Console.SetCursorPosition(x + 1, y);
            Console.Write("[");
            for (int i = 0; i < width - 4; i++)
            {
                if (i == name.Length)
                    break;
                Console.Write(name[i]);
            }
            Console.Write("]");
        }

        /// <summary>
        /// Draws a box with the specified dimensions and style, changing the specifed foreground and background colors.
        /// </summary>
        /// <param name="width">Specifies the width of the whole box.</param>
        /// <param name="height">Specifies the height of the whole box.</param>
        /// <param name="x">Specifies the x-coordinate of the top-left corner of the box.</param>
        /// <param name="y">Specifies the y-coordinate of the top-left corner of the box.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="foreColor">Specifies what color is used for the foreground color.</param>
        /// <param name="backColor">Specifies what color is used for the background color.</param>
        public void DrawBox(int width, int height, int x, int y, int style, int foreColor, int backColor)
        {
            SetColor(foreColor, false);
            SetColor(backColor, true);

            DrawBox(width, height, x, y, style);
        }

        /// <summary>
        /// Draws a box with the specified dimensions and style, changing the specifed foreground and background colors, and prints the given name to print at the top.
        /// </summary>
        /// <param name="width">Specifies the width of the whole box.</param>
        /// <param name="height">Specifies the height of the whole box.</param>
        /// <param name="x">Specifies the x-coordinate of the top-left corner of the box.</param>
        /// <param name="y">Specifies the y-coordinate of the top-left corner of the box.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="foreColor">Specifies what color is used for the foreground color.</param>
        /// <param name="backColor">Specifies what color is used for the background color.</param>
        /// <param name="name">Specifies the name printed at the top left corner of the box.</param>
        public void DrawBox(int width, int height, int x, int y, int style, int foreColor, int backColor, string name)
        {
            SetColor(foreColor, false);
            SetColor(backColor, true);

            DrawBox(width, height, x, y, style);

            Console.SetCursorPosition(x + 1, y);
            Console.Write("[");
            for (int i = 0; i < width - 4; i++)
            {
                if (i == name.Length)
                    break;
                Console.Write(name[i]);
            }
            Console.Write("]");
        }

        /// <summary>
        /// Redraws a previously drawn box from stored dimentions, using the last colors used. Does not save a new box to storage.
        /// </summary>
        /// <param name="index">Index of the saved box to redraw from.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        public void DrawBox(int index, int style)
        {
            mod = style * 11;

            for (int i = 0; i < savedBoxes[index, 3]; i++)
            {
                for (int j = 0; j < savedBoxes[index, 2]; j++)
                {
                    Console.SetCursorPosition(savedBoxes[index, 0] + j, savedBoxes[index, 1] + i);
                    if (j == 0 && i == 0)
                    {
                        Console.Write(boxChar[2 + mod]);
                        continue;
                    }
                    else if (j == 0 && i == savedBoxes[index, 3] - 1)
                    {
                        Console.Write(boxChar[4 + mod]);
                        continue;
                    }
                    else if (j == savedBoxes[index, 2] - 1 && i == savedBoxes[index, 3] - 1)
                    {
                        Console.Write(boxChar[5 + mod]);
                        continue;
                    }
                    else if (j == savedBoxes[index, 2] - 1 && i == 0)
                    {
                        Console.Write(boxChar[3 + mod]);
                        continue;
                    }
                    else if (j == 0 || j == savedBoxes[index, 2] - 1)
                    {
                        Console.Write(boxChar[1 + mod]);
                        continue;
                    }
                    else if (i == 0 || i == savedBoxes[index, 3] - 1)
                    {
                        Console.Write(boxChar[0 + mod]);
                        continue;
                    }
                    else
                        Console.Write(" ");
                }
            }
            textX = savedBoxes[index, 0] + 1;
            textY = savedBoxes[index, 1] + 1;
            textW = savedBoxes[index, 2] - 2;
            textH = savedBoxes[index, 3] - 2;
        }

        /// <summary>
        /// Redraws a previously drawn box from stored dimentions, using the last colors used and printing the given name to print at the top. Does not save a new box to storage.
        /// </summary>
        /// <param name="index">Index of the saved box to redraw from.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="name">Specifies the name printed at the top left corner of the box.</param>
        public void DrawBox(int index, int style, string name)
        {
            DrawBox(index, style);

            Console.SetCursorPosition(savedBoxes[index, 0] + 1, savedBoxes[index, 1]);
            Console.Write("[");
            for (int i = 0; i < savedBoxes[index, 2] - 4; i++)
            {
                if (i == name.Length)
                    break;
                Console.Write(name[i]);
            }
            Console.Write("]");
        }

        /// <summary>
        /// Redraws a previously drawn box from stored dimentions, using the foreground or background color given. Does not save a new box to storage.
        /// </summary>
        /// <param name="index">Index of the saved box to redraw from.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="color">Specifies the color to use for the foreground or background.</param>
        /// <param name="colorBackground">Specifies whether or not to color the background </param>
        public void DrawBox(int index, int style, int color, bool colorBackground)
        {
            SetColor(color, colorBackground);

            DrawBox(index, style);
        }

        /// <summary>
        /// Redraws a previously drawn box from stored dimentions, using the foreground or background color given and printing the given name to print at the top. Does not save a new box to storage.
        /// </summary>
        /// <param name="index">Index of the saved box to redraw from.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="color">Specifies the color to use for the foreground or background.</param>
        /// <param name="colorBackground">Specifies whether or not to color the background </param>
        /// <param name="name">Specifies the name printed at the top left corner of the box.</param>
        public void DrawBox(int index, int style, int color, bool colorBackground, string name)
        {
            SetColor(color, colorBackground);

            DrawBox(index, style);

            Console.SetCursorPosition(savedBoxes[index, 0] + 1, savedBoxes[index, 1]);
            Console.Write("[");
            for (int i = 0; i < savedBoxes[index, 2] - 4; i++)
            {
                if (i == name.Length)
                    break;
                Console.Write(name[i]);
            }
            Console.Write("]");
        }

        /// <summary>
        /// Redraws a previously drawn box from stored dimentions, using the foreground and background colors specified. Does not save a new box to storage.
        /// </summary>
        /// <param name="index">Index of the saved box to redraw from.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="foreColor">Specifies what color is used for the foreground color.</param>
        /// <param name="backColor">Specifies what color is used for the background color.</param>
        public void DrawBox(int index, int style, int foreColor, int backColor)
        {
            SetColor(foreColor, false);
            SetColor(backColor, true);

            DrawBox(index, style);
        }

        /// <summary>
        /// Redraws a previously drawn box from stored dimentions, using the foreground and background colors specified and printing the given name to print at the top. Does not save a new box to storage.
        /// </summary>
        /// <param name="index">Index of the saved box to redraw from.</param>
        /// <param name="style">Specifies the style of border of the box: <code>false</code>, single lined border,<code>true</code>, double lined border</param>
        /// <param name="foreColor">Specifies what color is used for the foreground color.</param>
        /// <param name="backColor">Specifies what color is used for the background color.</param>
        /// <param name="name">Specifies the name printed at the top left corner of the box.</param>
        public void DrawBox(int index, int style, int foreColor, int backColor, string name)
        {
            SetColor(foreColor, false);
            SetColor(backColor, true);

            DrawBox(index, style);

            Console.SetCursorPosition(savedBoxes[index, 0] + 1, savedBoxes[index, 1]);
            Console.Write("[");
            for (int i = 0; i < savedBoxes[index, 2] - 4; i++)
            {
                if (i == name.Length)
                    break;
                Console.Write(name[i]);
            }
            Console.Write("]");
        }
        
        private void SaveCurrentBox(int x, int y, int width, int height)
        {
            savedBoxes[savedIndex, 0] = x;
            savedBoxes[savedIndex, 1] = y;
            savedBoxes[savedIndex, 2] = width;
            savedBoxes[savedIndex, 3] = height;
            savedIndex++;
        }

        private int GetColor(bool colorBackground)
        {
            if (colorBackground)
            {
                switch (Console.BackgroundColor) {
                    case ConsoleColor.Black:
                        return 0;
                    case ConsoleColor.Red:
                        return 1;
                    case ConsoleColor.Green:
                        return 2;
                    case ConsoleColor.Yellow:
                        return 3;
                    case ConsoleColor.Blue:
                        return 4;
                    case ConsoleColor.Magenta:
                        return 5;
                    case ConsoleColor.Cyan:
                        return 6;
                    case ConsoleColor.White:
                        return 7;
                    case ConsoleColor.DarkGray:
                        return 8;
                    case ConsoleColor.DarkRed:
                        return 9;
                    case ConsoleColor.DarkGreen:
                        return 10;
                    case ConsoleColor.DarkYellow:
                        return 11;
                    case ConsoleColor.DarkBlue:
                        return 12;
                    case ConsoleColor.DarkMagenta:
                        return 13;
                    case ConsoleColor.DarkCyan:
                        return 14;
                    case ConsoleColor.Gray:
                        return 15;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (Console.ForegroundColor)
                {
                    case ConsoleColor.Black:
                        return 0;
                    case ConsoleColor.Red:
                        return 1;
                    case ConsoleColor.Green:
                        return 2;
                    case ConsoleColor.Yellow:
                        return 3;
                    case ConsoleColor.Blue:
                        return 4;
                    case ConsoleColor.Magenta:
                        return 5;
                    case ConsoleColor.Cyan:
                        return 6;
                    case ConsoleColor.White:
                        return 7;
                    case ConsoleColor.DarkGray:
                        return 8;
                    case ConsoleColor.DarkRed:
                        return 9;
                    case ConsoleColor.DarkGreen:
                        return 10;
                    case ConsoleColor.DarkYellow:
                        return 11;
                    case ConsoleColor.DarkBlue:
                        return 12;
                    case ConsoleColor.DarkMagenta:
                        return 13;
                    case ConsoleColor.DarkCyan:
                        return 14;
                    case ConsoleColor.Gray:
                        return 15;
                    default:
                        return 0;
                }
            }
        }

        private void SetColor(int index, bool colorBackground)
        {
            if (colorBackground)
            {
                switch (index)
                {
                    case 0:
                        Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    case 1:
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    case 2:
                        Console.BackgroundColor = ConsoleColor.Green;
                        break;
                    case 3:
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        break;
                    case 4:
                        Console.BackgroundColor = ConsoleColor.Blue;
                        break;
                    case 5:
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        break;
                    case 6:
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        break;
                    case 7:
                        Console.BackgroundColor = ConsoleColor.White;
                        break;
                    case 8:
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        break;
                    case 9:
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        break;
                    case 10:
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;
                    case 11:
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        break;
                    case 12:
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        break;
                    case 13:
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case 14:
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        break;
                    case 15:
                        Console.BackgroundColor = ConsoleColor.Gray;
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case 6:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case 7:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case 8:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case 9:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case 10:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case 11:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case 12:
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                    case 13:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case 14:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case 15:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }
            }
        }

        public void TestShowAllChars()
        {
            for (int i = 0; i < boxChar.Length; i++)
            {
                Console.Write(boxChar[i]);
            }
        }

        /// <summary>
        /// Generic instantiator for BoxWindows, will keep the size of the console at instantiation
        /// </summary>

        public BoxWindows()
        {
            boxChar = new char[44];
            boxChar[0] = '─';
            boxChar[1] = '│';
            boxChar[2] = '┌';
            boxChar[3] = '┐';
            boxChar[4] = '└';
            boxChar[5] = '┘';
            boxChar[6] = '├';
            boxChar[7] = '┤';
            boxChar[8] = '┬';
            boxChar[9] = '┴';
            boxChar[10] = '┼';
            boxChar[11] = '═';
            boxChar[12] = '║';
            boxChar[13] = '╔';
            boxChar[14] = '╗';
            boxChar[15] = '╚';
            boxChar[16] = '╝';
            boxChar[17] = '╠';
            boxChar[18] = '╣';
            boxChar[19] = '╦';
            boxChar[20] = '╩';
            boxChar[21] = '╬';
            boxChar[22] = '█';
            boxChar[23] = '█';
            boxChar[24] = '█';
            boxChar[25] = '█';
            boxChar[26] = '█';
            boxChar[27] = '█';
            boxChar[28] = '█';
            boxChar[29] = '█';
            boxChar[30] = '█';
            boxChar[31] = '█';
            boxChar[32] = '█';
            boxChar[33] = '-';
            boxChar[34] = '|';
            boxChar[35] = '+';
            boxChar[36] = '+';
            boxChar[37] = '+';
            boxChar[38] = '+';
            boxChar[39] = '+';
            boxChar[40] = '+';
            boxChar[41] = '+';
            boxChar[42] = '+';
            boxChar[43] = '+';
            savedIndex = 0;
            savedBoxes = new int[128, 4];
            savedBuffers = new string[32][];
            savedForeColors = new int[32][];
            savedBackColors = new int[32][];
        }

        /// <summary>
        /// Instantiator with width and height values
        /// </summary>
        /// <param name="width">width of the console</param>
        /// <param name="height">height of the console</param>

        public BoxWindows(int width, int height) : this ()
        {
            bool init = true;

            while (init)
            {
                try
                {
                    Console.SetWindowSize(width, height);

                    init = false;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    --width;
                    --height;
                }
            }
        }
    }
}
