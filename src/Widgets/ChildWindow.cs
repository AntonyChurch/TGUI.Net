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
using SFML.System;

namespace TGUI
{
	public class ChildWindow : Container
	{
		[Flags]
		public enum TitleButton
		{
			None     = 0,
			Close    = 1 << 0,
			Maximize = 1 << 1,
			Minimize = 1 << 2
		}

		public ChildWindow()
			: base(tguiChildWindow_create())
		{
		}

        public ChildWindow(string title, TitleButton titleButtons = TitleButton.Close)
			: base(tguiChildWindow_create())
		{
		    Title = title;
		    TitleButtons = titleButtons;
		}

		protected internal ChildWindow(IntPtr cPointer)
			: base(cPointer)
		{
		}

		public ChildWindow(ChildWindow copy)
			: base(copy)
		{
		}

		public new ChildWindowRenderer Renderer
		{
			get { return new ChildWindowRenderer(tguiWidget_getRenderer(CPointer)); }
		}

        public new ChildWindowRenderer SharedRenderer
		{
			get { return new ChildWindowRenderer(tguiWidget_getSharedRenderer(CPointer)); }
		}

		public Vector2f MinimumSize
		{
			get { return tguiChildWindow_getMinimumSize(CPointer); }
			set { tguiChildWindow_setMinimumSize(CPointer, value); }
		}

		public Vector2f MaximumSize
		{
			get { return tguiChildWindow_getMaximumSize(CPointer); }
			set { tguiChildWindow_setMaximumSize(CPointer, value); }
		}

		public string Title
		{
			get { return Util.GetStringFromC_UTF32(tguiChildWindow_getTitle(CPointer)); }
			set { tguiChildWindow_setTitle(CPointer, Util.ConvertStringForC_UTF32(value)); }
		}

		public uint TitleTextSize
		{
			get { return tguiChildWindow_getTitleTextSize(CPointer); }
			set { tguiChildWindow_setTitleTextSize(CPointer, value); }
		}

		public HorizontalAlignment TitleAlignment
		{
			get { return tguiChildWindow_getTitleAlignment(CPointer); }
			set { tguiChildWindow_setTitleAlignment(CPointer, value); }
		}

		public TitleButton TitleButtons
		{
			get { return tguiChildWindow_getTitleButtons(CPointer); }
			set { tguiChildWindow_setTitleButtons(CPointer, value); }
		}

		public bool Resizable
		{
			get { return tguiChildWindow_isResizable(CPointer); }
			set { tguiChildWindow_setResizable(CPointer, value); }
		}

		public bool KeepInParent
		{
			get { return tguiChildWindow_isKeptInParent(CPointer); }
			set { tguiChildWindow_setKeepInParent(CPointer, value); }
		}

		protected override void InitSignals()
		{
			base.InitSignals();

            MousePressedCallback = new CallbackAction(ProcessMousePressedSignal);
		    if (tguiWidget_connect(CPointer, Util.ConvertStringForC_ASCII("MousePressed"), MousePressedCallback) == 0)
				throw new TGUIException(Util.GetStringFromC_ASCII(tgui_getLastError()));

            ClosedCallback = new CallbackAction(ProcessClosedSignal);
		    if (tguiWidget_connect(CPointer, Util.ConvertStringForC_ASCII("Closed"), ClosedCallback) == 0)
				throw new TGUIException(Util.GetStringFromC_ASCII(tgui_getLastError()));

            MaximizedCallback = new CallbackAction(ProcessMaximizedSignal);
		    if (tguiWidget_connect(CPointer, Util.ConvertStringForC_ASCII("Maximized"), MaximizedCallback) == 0)
				throw new TGUIException(Util.GetStringFromC_ASCII(tgui_getLastError()));

            MinimizedCallback = new CallbackAction(ProcessMinimizedSignal);
		    if (tguiWidget_connect(CPointer, Util.ConvertStringForC_ASCII("Minimized"), MinimizedCallback) == 0)
				throw new TGUIException(Util.GetStringFromC_ASCII(tgui_getLastError()));
		}

		private void ProcessMousePressedSignal()
		{
            MousePressed?.Invoke(this, EventArgs.Empty);
        }

		private void ProcessClosedSignal()
		{
			if (Closed != null)
				Closed(this, EventArgs.Empty);
			else
			{
				// Actually close the window when no signal handler is connected
				if (!myConnectedSignals.ContainsKey("closed"))
				{
					if (Parent != null)
						Parent.Remove(this);
				}
			}
		}

		private void ProcessMaximizedSignal()
		{
            Maximized?.Invoke(this, EventArgs.Empty);
        }

		private void ProcessMinimizedSignal()
		{
            Minimized?.Invoke(this, EventArgs.Empty);
        }

		/// <summary>Event handler for the MousePressed signal</summary>
		public event EventHandler MousePressed = null;

		/// <summary>Event handler for the Closed signal</summary>
		public event EventHandler Closed = null;

		/// <summary>Event handler for the Maximized signal</summary>
		public event EventHandler Maximized = null;

		/// <summary>Event handler for the Minimized signal</summary>
		public event EventHandler Minimized = null;

        private CallbackAction MousePressedCallback;
        private CallbackAction ClosedCallback;
        private CallbackAction MaximizedCallback;
        private CallbackAction MinimizedCallback;


		#region Imports

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected IntPtr tguiChildWindow_create();

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setTitle(IntPtr cPointer, IntPtr value);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setMaximumSize(IntPtr cPointer, Vector2f maxSize);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected Vector2f tguiChildWindow_getMaximumSize(IntPtr cPointer);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setMinimumSize(IntPtr cPointer, Vector2f minSize);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected Vector2f tguiChildWindow_getMinimumSize(IntPtr cPointer);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected IntPtr tguiChildWindow_getTitle(IntPtr cPointer);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setTitleTextSize(IntPtr cPointer, uint textSize);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected uint tguiChildWindow_getTitleTextSize(IntPtr cPointer);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setTitleAlignment(IntPtr cPointer, HorizontalAlignment alignment);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected HorizontalAlignment tguiChildWindow_getTitleAlignment(IntPtr cPointer);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setTitleButtons(IntPtr cPointer, TitleButton buttons);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected TitleButton tguiChildWindow_getTitleButtons(IntPtr cPointer);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setResizable(IntPtr cPointer, bool resizable);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected bool tguiChildWindow_isResizable(IntPtr cPointer);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected void tguiChildWindow_setKeepInParent(IntPtr cPointer, bool keepInParent);

		[DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
		static extern protected bool tguiChildWindow_isKeptInParent(IntPtr cPointer);

		#endregion
	}
}
