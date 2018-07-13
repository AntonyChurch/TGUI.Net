/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// TGUI - Texus' Graphical User Interface
// Copyright (C) 2012-2016 Bruno Van de Velde (vdv_b@tgui.eu)
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
using System.Security;
using System.Runtime.InteropServices;
using SFML.Graphics;

namespace TGUI
{
    public class ListBoxRenderer : WidgetRenderer
    {
        public ListBoxRenderer()
            : base(tguiListBoxRenderer_create())
        {
        }

        protected internal ListBoxRenderer(IntPtr cPointer)
            : base(cPointer)
        {
        }

        public ListBoxRenderer(ListBoxRenderer copy)
            : base(tguiListBoxRenderer_copy(copy.CPointer))
        {
        }

        public Outline Borders
        {
            get { return new Outline(tguiListBoxRenderer_getBorders(CPointer)); }
            set { tguiListBoxRenderer_setBorders(CPointer, value.CPointer); }
        }

        public Outline Padding
        {
            get { return new Outline(tguiListBoxRenderer_getPadding(CPointer)); }
            set { tguiListBoxRenderer_setPadding(CPointer, value.CPointer); }
        }

        public Color BackgroundColor
        {
            get { return tguiListBoxRenderer_getBackgroundColor(CPointer); }
            set { tguiListBoxRenderer_setBackgroundColor(CPointer, value); }
        }

        public Color BackgroundColorHover
        {
            get { return tguiListBoxRenderer_getBackgroundColorHover(CPointer); }
            set { tguiListBoxRenderer_setBackgroundColorHover(CPointer, value); }
        }

        public Color SelectedBackgroundColor
        {
            get { return tguiListBoxRenderer_getSelectedBackgroundColor(CPointer); }
            set { tguiListBoxRenderer_setSelectedBackgroundColor(CPointer, value); }
        }

        public Color SelectedBackgroundColorHover
        {
            get { return tguiListBoxRenderer_getSelectedBackgroundColorHover(CPointer); }
            set { tguiListBoxRenderer_setSelectedBackgroundColorHover(CPointer, value); }
        }

        public Color TextColor
        {
            get { return tguiListBoxRenderer_getTextColor(CPointer); }
            set { tguiListBoxRenderer_setTextColor(CPointer, value); }
        }

        public Color TextColorHover
        {
            get { return tguiListBoxRenderer_getTextColorHover(CPointer); }
            set { tguiListBoxRenderer_setTextColorHover(CPointer, value); }
        }

        public Color SelectedTextColor
        {
            get { return tguiListBoxRenderer_getSelectedTextColor(CPointer); }
            set { tguiListBoxRenderer_setSelectedTextColor(CPointer, value); }
        }

        public Color SelectedTextColorHover
        {
            get { return tguiListBoxRenderer_getSelectedTextColorHover(CPointer); }
            set { tguiListBoxRenderer_setSelectedTextColorHover(CPointer, value); }
        }

        public Color BorderColor
        {
            get { return tguiListBoxRenderer_getBorderColor(CPointer); }
            set { tguiListBoxRenderer_setBorderColor(CPointer, value); }
        }

        public Texture TextureBackground
        {
            set { tguiListBoxRenderer_setTextureBackground(CPointer, value.CPointer); }
        }

        public Text.Styles TextStyle
        {
            get { return tguiListBoxRenderer_getTextStyle(CPointer); }
            set { tguiListBoxRenderer_setTextStyle(CPointer, value); }
        }

        public Text.Styles SelectedTextStyle
        {
            get { return tguiListBoxRenderer_getSelectedTextStyle(CPointer); }
            set { tguiListBoxRenderer_setSelectedTextStyle(CPointer, value); }
        }

        public RendererData Scrollbar
        {
            get { return new RendererData(tguiListBoxRenderer_getScrollbar(CPointer)); }
            set { tguiListBoxRenderer_setScrollbar(CPointer, value.CPointer); }
        }

        public float ScrollbarWidth
        {
            get { return tguiListBoxRenderer_getScrollbarWidth(CPointer); }
            set { tguiListBoxRenderer_setScrollbarWidth(CPointer, value); }
        }


        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected IntPtr tguiListBoxRenderer_create();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected IntPtr tguiListBoxRenderer_copy(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setBorders(IntPtr cPointer, IntPtr borders);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected IntPtr tguiListBoxRenderer_getBorders(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setPadding(IntPtr cPointer, IntPtr borders);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected IntPtr tguiListBoxRenderer_getPadding(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setBackgroundColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getBackgroundColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setBackgroundColorHover(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getBackgroundColorHover(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setSelectedBackgroundColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getSelectedBackgroundColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setSelectedBackgroundColorHover(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getSelectedBackgroundColorHover(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setTextColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getTextColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setTextColorHover(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getTextColorHover(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setSelectedTextColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getSelectedTextColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setSelectedTextColorHover(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getSelectedTextColorHover(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setBorderColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Color tguiListBoxRenderer_getBorderColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setTextureBackground(IntPtr cPointer, IntPtr texture);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setTextStyle(IntPtr cPointer, Text.Styles style);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Text.Styles tguiListBoxRenderer_getTextStyle(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setSelectedTextStyle(IntPtr cPointer, Text.Styles style);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected Text.Styles tguiListBoxRenderer_getSelectedTextStyle(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setScrollbar(IntPtr cPointer, IntPtr rendererData);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected IntPtr tguiListBoxRenderer_getScrollbar(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected void tguiListBoxRenderer_setScrollbarWidth(IntPtr cPointer, float width);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern protected float tguiListBoxRenderer_getScrollbarWidth(IntPtr cPointer);

        #endregion
    }
}