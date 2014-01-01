/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// TGUI.Net - Texus's Graphical User Interface for .Net languages
// Copyright (C) 2013-2014 Bruno Van de Velde (vdv_b@tgui.eu)
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented;
//    you must not claim that you wrote the original software.
//    If you use this software in a product, an acknowledgment
//    in the product documentation would be appreciated but is not required.
//
// 2. Altered source versions must be plainly marked as such,
//    and must not be misrepresented as being the original software.
//
// 3. This notice may not be removed or altered from any source distribution.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace TGUI
{
    public class Borders
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Default constructor
        /// </summary>
        ///
        /// <param name="left">Left border width</param>
        /// <param name="top">Top border height</param>
        /// <param name="right">Right border width</param>
        /// <param name="bottom">Bottom border height</param>
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Borders(uint left = 0, uint top = 0, uint right = 0, uint bottom = 0)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public uint Left;
        public uint Top;
        public uint Right;
        public uint Bottom;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public interface WidgetBorders
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Borders of the widget
        /// </summary>
        ///
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Borders Borders
        {
            get;
            set;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
